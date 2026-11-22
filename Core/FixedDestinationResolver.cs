using System.Globalization;
using UnityEngine;

namespace DrakesPortals.Core;

/// <summary>Always sends the player to the configured "x,y,z" position.</summary>
public sealed class FixedDestinationResolver : IDestinationResolver
{
    public const string ResolverName = "Fixed";

    public string Name => ResolverName;

    public ResolveResult Resolve(ResolveContext context)
    {
        if (!TryParse(DrakesPortalsConfig.FixedDestination, out var position))
            return ResolveResult.Refuse("This gate has no valid destination.");
        return ResolveResult.Go(position, Quaternion.identity);
    }

    public static bool TryParse(string? text, out Vector3 position)
    {
        position = Vector3.zero;
        if (string.IsNullOrWhiteSpace(text))
            return false;
        var parts = text!.Split(',');
        if (parts.Length != 3)
            return false;
        var values = new float[3];
        for (var i = 0; i < 3; i++)
        {
            if (!float.TryParse(parts[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out values[i]))
                return false;
        }
        position = new Vector3(values[0], values[1], values[2]);
        return true;
    }
}
