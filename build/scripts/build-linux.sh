#!/usr/bin/env bash
# Linux build for DrakesPortals: the same steps CI runs (.github/workflows/release.yml), runnable locally.
#   build/scripts/build-linux.sh [Debug|Release]
# Needs: dotnet SDK 8, nuget (or mono's nuget), git, zip, python3, jq.
# Compiles against the game DLLs from the Pfhoenix NuGet package (no Valheim install needed), and against
# DrakeModsLibs built from the tag pinned in mod.package.props.
set -euo pipefail

CONFIG="${1:-Release}"
ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
cd "$ROOT"

VERSION="$(sed -n 's/.*<Version>\([^<]*\)<\/Version>.*/\1/p' mod.package.props | head -1 | xargs)"
LIBS_VERSION="$(sed -n 's/.*DrakeMods-DrakeModsLibs-\([0-9.]*\).*/\1/p' mod.package.props | head -1)"
NET48_VER="1.0.3"
export CI_NET48_FWPATH="$ROOT/packages/Microsoft.NETFramework.ReferenceAssemblies.net48.$NET48_VER/build"
export CI_SKIP_POST_BUILD=true

echo "== restore"
nuget restore DrakesPortals.csproj -PackagesDirectory packages

echo "== stage game DLLs"
"$ROOT/build/scripts/stage-valheim-refs.sh" "$ROOT"
CI_ROOT="$ROOT/ci_valheim"
MANAGED="$CI_ROOT/valheim_Data/Managed"
export VALHEIM_INSTALL="$CI_ROOT"

echo "== DrakeModsLibs v$LIBS_VERSION"
if [ -z "${WORKSHOP_LIBS_DLL:-}" ]; then
  if [ ! -d ci_libs/DrakeModsLibsSrc ]; then
    git clone --depth 1 --branch "v$LIBS_VERSION" https://github.com/drakethos/DrakesWorkshopLibs.git ci_libs/DrakeModsLibsSrc
  fi
  nuget restore ci_libs/DrakeModsLibsSrc/DrakeModsLibs.csproj -PackagesDirectory ci_libs/DrakeModsLibsSrc/packages
  dotnet msbuild ci_libs/DrakeModsLibsSrc/DrakeModsLibs.csproj \
    /p:Configuration=Release /p:TargetFrameworkVersion=v4.8 \
    /p:BepInExPath="$CI_ROOT/BepInEx" /p:CorlibPath="$MANAGED" \
    /p:PublicizedAssembliesPath="$MANAGED/publicized_assemblies" \
    /p:CI_NET48_FWPATH="$CI_NET48_FWPATH" /p:CI_SKIP_POST_BUILD=true /t:Build /v:minimal
  WORKSHOP_LIBS_DLL="$(find ci_libs/DrakeModsLibsSrc -path '*/bin/Release/DrakeModsLibs.dll' | head -1)"
  WORKSHOP_LIBS_DLL="$ROOT/$WORKSHOP_LIBS_DLL"
fi
[ -f "$WORKSHOP_LIBS_DLL" ] || { echo "DrakeModsLibs.dll not found" >&2; exit 1; }

echo "== build DrakesPortals $VERSION ($CONFIG)"
dotnet msbuild DrakesPortals.csproj \
  /p:Configuration="$CONFIG" /p:TargetFrameworkVersion=v4.8 \
  /p:BepInExPath="$CI_ROOT/BepInEx" /p:CorlibPath="$MANAGED" \
  /p:PublicizedAssembliesPath="$MANAGED/publicized_assemblies" \
  /p:WorkshopLibsDll="$WORKSHOP_LIBS_DLL" /p:Version="$VERSION" /t:Build /v:minimal

echo "== package"
PKG="DrakeMods-DrakesPortals-$VERSION"
rm -rf "dist/$PKG" "dist/$PKG.zip"
mkdir -p "dist/$PKG/plugins"
cp "bin/$CONFIG/DrakesPortals.dll" "dist/$PKG/plugins/"
cp README.md CHANGELOG.md "dist/$PKG/"
[ -f icon.png ] && cp icon.png "dist/$PKG/"
dotnet msbuild DrakesPortals.csproj /p:Version="$VERSION" /p:ThunderstoreManifestDir="$ROOT/dist/$PKG" /t:WriteThunderstoreManifest /v:minimal
(cd "dist/$PKG" && zip -qr "../$PKG.zip" .)
python3 .github/scripts/check-gale-layout.py "dist/$PKG.zip" "DrakeMods-DrakesPortals" --expect "DrakesPortals.dll"
echo "OK: dist/$PKG.zip"
