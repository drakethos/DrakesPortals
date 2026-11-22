using System;
using System.Collections.Generic;

namespace DrakesPortals.Core;

/// <summary>Name to resolver lookup. Other mods can add resolvers at startup.</summary>
public static class ResolverRegistry
{
    private static readonly Dictionary<string, IDestinationResolver> Resolvers =
        new Dictionary<string, IDestinationResolver>(StringComparer.OrdinalIgnoreCase);

    public static void Register(IDestinationResolver resolver)
    {
        if (resolver == null || string.IsNullOrWhiteSpace(resolver.Name))
            return;
        Resolvers[resolver.Name] = resolver;
    }

    public static void RegisterBuiltIns() => Register(new FixedDestinationResolver());

    /// <summary>Falls back to Fixed when the name is unknown, so a typo in the config never strands a player.</summary>
    public static IDestinationResolver? Get(string? name)
    {
        if (!string.IsNullOrWhiteSpace(name) && Resolvers.TryGetValue(name!.Trim(), out var found))
            return found;
        return Resolvers.TryGetValue(FixedDestinationResolver.ResolverName, out var fallback) ? fallback : null;
    }
}
