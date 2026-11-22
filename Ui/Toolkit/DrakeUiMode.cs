using System;

namespace DrakesPortals.Ui.Toolkit;

/// <summary>
/// Which look the shared dialogs use: the new UI Toolkit one (default) or the classic wood panels. One per-player switch
/// (<c>UseToolkitUi</c> in this mod's config) covers every mod that uses the shared menu, confirm and prompt components.
/// If the new look ever fails to build (a missing shader, say) it turns itself off for the session and the classic panels take over.
/// </summary>
public static class DrakeUiMode
{
    static bool _failed;

    /// <summary>True when shared dialogs should draw in the new look.</summary>
    public static bool Toolkit => !_failed && UiSettings.UseToolkitUi;

    /// <summary>Called when the new look could not be created; the classic one is used from now on.</summary>
    public static void ReportFailure(Exception ex)
    {
        if (_failed)
            return;
        _failed = true;
        Jotunn.Logger.LogError($"[DrakesPortals] The new UI could not be created, using the classic panels instead: {ex}");
    }
}
