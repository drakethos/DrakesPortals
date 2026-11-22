using System;
using System.Reflection;

namespace DrakesPortals.Integration;

/// <summary>
/// Soft link to DrakeModsLibs' piece-interaction claims, by reflection (no reference, no dependency). Mods that also want
/// Shift+E on pieces (RenameIt's piece rename) leave portals alone when this is registered.
/// </summary>
internal static class LibsClaims
{
    private static bool _done;

    public static void TryRegister()
    {
        if (_done)
            return;
        _done = true;
        try
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                var type = assembly.GetType("DrakeModsLibs.API.PieceInteractionClaims", false);
                var register = type?.GetMethod("Register", BindingFlags.Public | BindingFlags.Static,
                    null, new[] { typeof(string), typeof(Func<Piece, bool>) }, null);
                if (register == null)
                    continue;
                register.Invoke(null, new object[] { DrakesPortals.ModName, new Func<Piece, bool>(IsPortalPiece) });
                DrakesPortals.Log?.LogInfo("Claimed portal pieces for Shift+E (DrakeModsLibs found).");
                return;
            }
        }
        catch (Exception ex)
        {
            DrakesPortals.Log?.LogDebug($"Piece claim skipped: {ex.Message}");
        }
    }

    private static bool IsPortalPiece(Piece piece) => piece && piece.GetComponent<TeleportWorld>() != null;
}
