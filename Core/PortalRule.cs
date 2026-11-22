namespace DrakesPortals.Core;

/// <summary>
/// One gate's rules. The direction is not a field on purpose: every gate is one way. A gate sends a player to a
/// destination chosen by <see cref="ResolverName"/> and never offers a way back through itself.
/// </summary>
public sealed class PortalRule
{
    /// <summary>Stable id for the gate (stored on the piece, never the display name).</summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>Name registered in <see cref="ResolverRegistry"/>.</summary>
    public string ResolverName { get; set; } = "Fixed";

    // Later: who may enter (team / global key), use limits per player, cooldown, cost.
}
