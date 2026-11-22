namespace DrakesPortals.Core;

/// <summary>Which way a linked portal works. Stored on the portal's ZDO, so every client sees it.</summary>
public enum PortalDirection
{
    /// <summary>A normal portal: both ends work.</summary>
    TwoWay = 0,

    /// <summary>One-way: players step in here and arrive at the linked portal.</summary>
    Entry = 1,

    /// <summary>One-way: players arrive here. Stepping into it does nothing.</summary>
    Exit = 2,
}
