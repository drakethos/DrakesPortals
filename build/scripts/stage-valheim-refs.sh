#!/usr/bin/env bash
# Builds a fake Valheim install tree (ci_valheim/) from the Pfhoenix NuGet package, so the mod compiles without a
# game install. Same steps as the "Stage game DLLs" step in .github/workflows/release.yml.
set -euo pipefail
ROOT="${1:-$(pwd)}"
cd "$ROOT"
PFHOENIX_LIB="$(ls -d packages/Pfhoenix.Valheim.ModProjectReferences.*/lib/net46 2>/dev/null | head -1)"
[ -n "$PFHOENIX_LIB" ] || { echo "Pfhoenix package not found; run nuget restore first" >&2; exit 1; }

MANAGED="ci_valheim/valheim_Data/Managed"
PUBLICIZED="$MANAGED/publicized_assemblies"
BEPINEX="ci_valheim/BepInEx/core"
mkdir -p "$MANAGED" "$PUBLICIZED" "$BEPINEX"
cp "$PFHOENIX_LIB/"*.dll "$MANAGED/"
cp "$PFHOENIX_LIB/assembly_valheim.dll" "$PUBLICIZED/assembly_valheim_publicized.dll"
cp "$PFHOENIX_LIB/assembly_utils.dll"   "$PUBLICIZED/assembly_utils_publicized.dll"
cp "$PFHOENIX_LIB/gui_framework.dll"    "$PUBLICIZED/gui_framework_publicized.dll"
for src in assembly_googleanalytics assembly_guiutils assembly_lux assembly_postprocessing \
           assembly_simplemeshcombine assembly_sunshafts SoftReferenceableAssets; do
  if [ -f "$PFHOENIX_LIB/$src.dll" ]; then cp "$PFHOENIX_LIB/$src.dll" "$PUBLICIZED/${src}_publicized.dll"; fi
done
cp "$PFHOENIX_LIB/0Harmony.dll" "$BEPINEX/"
cp "$PFHOENIX_LIB/bepinex.dll"  "$BEPINEX/BepInEx.dll"
echo "Staged game references in $ROOT/ci_valheim"
