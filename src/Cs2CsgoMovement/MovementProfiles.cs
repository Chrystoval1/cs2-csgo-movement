namespace Cs2CsgoMovement;

/// <summary>
/// The built-in movement presets.
/// </summary>
/// <remarks>
/// Key design note: <see cref="Csgo"/> and <see cref="Vanilla"/> share the exact same
/// ground/air movement values. The only difference between them is the two subtick levers
/// (<c>sv_quantize_movement_input</c> and <c>sv_subtick_movement_view_angles</c>). That is
/// deliberate and honest: CS:GO and CS2 share most movement cvar defaults, so the felt
/// difference comes from subtick, not from the acceleration numbers. See docs/why-it-works.md.
///
/// All numeric values are a sane competitive starting point, not gospel. Tune them on your
/// own server to taste.
/// </remarks>
public static class MovementProfiles
{
    // Shared ground/air model used by both csgo and vanilla (identical on purpose).
    private static readonly ConVarSetting[] SharedMovement =
    [
        new("sv_accelerate", "5.5"),
        new("sv_airaccelerate", "12"),
        new("sv_air_max_wishspeed", "30"),
        new("sv_friction", "5.2"),
        new("sv_maxspeed", "320"),
        new("sv_gravity", "800"),
        new("sv_enablebunnyhopping", "0"),
        new("sv_autobunnyhopping", "0"),
    ];

    /// <summary>
    /// CS:GO-like feel: subtick movement tempered (digital-style input, no subtick view
    /// angles), with standard competitive movement values.
    /// </summary>
    public static readonly MovementProfile Csgo = new("csgo",
    [
        new("sv_quantize_movement_input", "1"),
        new("sv_subtick_movement_view_angles", "0"),
        .. SharedMovement,
    ]);

    /// <summary>
    /// Stock CS2 feel: subtick fully enabled. Same movement numbers as <see cref="Csgo"/>,
    /// so toggling between the two isolates the subtick levers for A/B comparison.
    /// </summary>
    public static readonly MovementProfile Vanilla = new("vanilla",
    [
        new("sv_quantize_movement_input", "0"),
        new("sv_subtick_movement_view_angles", "1"),
        .. SharedMovement,
    ]);

    /// <summary>
    /// Experimental surf preset: high air acceleration and bunnyhopping enabled.
    /// Starting point only — surf maps vary a lot; adjust to the maps you run.
    /// </summary>
    public static readonly MovementProfile Surf = new("surf",
    [
        new("sv_quantize_movement_input", "1"),
        new("sv_subtick_movement_view_angles", "0"),
        new("sv_accelerate", "10"),
        new("sv_airaccelerate", "150"),
        new("sv_air_max_wishspeed", "30"),
        new("sv_friction", "4"),
        new("sv_maxspeed", "320"),
        new("sv_gravity", "800"),
        new("sv_enablebunnyhopping", "1"),
        new("sv_autobunnyhopping", "1"),
    ]);

    /// <summary>The preset applied when the plugin loads.</summary>
    public static MovementProfile Default => Csgo;

    private static readonly IReadOnlyDictionary<string, MovementProfile> ByName =
        new Dictionary<string, MovementProfile>(StringComparer.OrdinalIgnoreCase)
        {
            [Csgo.Name] = Csgo,
            [Vanilla.Name] = Vanilla,
            [Surf.Name] = Surf,
        };

    /// <summary>Comma-separated list of available preset names, for help text.</summary>
    public static string Names => string.Join(", ", ByName.Keys);

    /// <summary>Look up a preset by name, case-insensitively.</summary>
    public static bool TryGet(string name, out MovementProfile profile) =>
        ByName.TryGetValue(name ?? string.Empty, out profile!);
}
