using UnityEngine;

namespace DrakesPortals.Core;

/// <summary>Who is stepping through which gate. Kept small; add fields as resolvers need them.</summary>
public readonly struct ResolveContext
{
    public ResolveContext(Player player, PortalRule rule, Vector3 gatePosition)
    {
        Player = player;
        Rule = rule;
        GatePosition = gatePosition;
    }

    public Player Player { get; }
    public PortalRule Rule { get; }
    public Vector3 GatePosition { get; }
}

public readonly struct ResolveResult
{
    private ResolveResult(bool ok, Vector3 position, Quaternion rotation, string? refusal)
    {
        Ok = ok;
        Position = position;
        Rotation = rotation;
        Refusal = refusal;
    }

    public bool Ok { get; }
    public Vector3 Position { get; }
    public Quaternion Rotation { get; }

    /// <summary>Shown to the player when <see cref="Ok"/> is false.</summary>
    public string? Refusal { get; }

    public static ResolveResult Go(Vector3 position, Quaternion rotation) => new ResolveResult(true, position, rotation, null);
    public static ResolveResult Refuse(string message) => new ResolveResult(false, Vector3.zero, Quaternion.identity, message);
}

/// <summary>
/// Decides where a gate sends a player. 0.1.0 ships one (Fixed). Item, team, condition and puzzle resolvers are
/// added later without touching the portal code.
/// </summary>
public interface IDestinationResolver
{
    string Name { get; }

    ResolveResult Resolve(ResolveContext context);
}
