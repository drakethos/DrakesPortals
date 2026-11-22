using System;
using System.Reflection;
using BepInEx.Configuration;
using HarmonyLib;

namespace DrakesPortals.Ui;

/// <summary>
/// The few settings the UI reads. Replaces the Libs integration config so this folder has no outside dependency.
/// Call <see cref="Bind"/> once from the plugin's Awake; without it the defaults below apply.
/// </summary>
public static class UiSettings
{
    private static ConfigEntry<bool>? _useToolkitUi;

    /// <summary>
    /// Draw menus, confirm and prompt dialogs in the new look (UI Toolkit) instead of the classic wood panels.
    /// Per-client; applies the next time a dialog opens.
    /// </summary>
    public static bool UseToolkitUi => _useToolkitUi?.Value ?? true;

    /// <summary>When set and available, forces that tab id as the default open tab. Not configurable here.</summary>
    public static string? ForceDefaultTabId => null;

    /// <summary>When true, tabs that "claim" an item are not auto-selected. Not configurable here.</summary>
    public static bool DisableClaimDefault => false;

    /// <summary>Per-tab priority override. This mod has none; the tab's own priority is used.</summary>
    public static bool TryGetPriorityOverride(string tabId, out int priority)
    {
        priority = 0;
        return false;
    }

    public static void Bind(ConfigFile config)
    {
        if (config == null || _useToolkitUi != null)
            return;
        _useToolkitUi = config.Bind(
            "Ui",
            "UseToolkitUi",
            true,
            "Draw dialogs in the new look (UI Toolkit). Turn off for the classic wood panels. Per client.");
    }
}

/// <summary>
/// Valheim 1.0+: <c>Character.Message(type, msg, amount, icon, log)</c>. Older reference assemblies only have the
/// 4-argument overload, so the call is bound at runtime.
/// </summary>
internal static class CharacterMessageInvoker
{
    private static readonly MethodInfo? Message5 = AccessTools.Method(
        typeof(Character),
        nameof(Character.Message),
        new[] { typeof(MessageHud.MessageType), typeof(string), typeof(int), typeof(UnityEngine.Sprite), typeof(bool) });

    private static readonly MethodInfo? Message4 = AccessTools.Method(
        typeof(Character),
        nameof(Character.Message),
        new[] { typeof(MessageHud.MessageType), typeof(string), typeof(int), typeof(UnityEngine.Sprite) });

    internal static void Show(Character character, MessageHud.MessageType type, string msg, int amount, UnityEngine.Sprite? icon)
    {
        if (!character)
            return;

        try
        {
            if (Message5 != null)
            {
                Message5.Invoke(character, new object?[] { type, msg, amount, icon, false });
                return;
            }

            Message4?.Invoke(character, new object?[] { type, msg, amount, icon });
        }
        catch (Exception)
        {
            // Never break the HUD over a message.
        }
    }
}
