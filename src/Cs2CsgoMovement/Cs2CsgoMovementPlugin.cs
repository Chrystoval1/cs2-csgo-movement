using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Commands;

namespace Cs2CsgoMovement;

/// <summary>
/// Applies a CS:GO-like movement profile to a CS2 server and exposes console commands to
/// switch, toggle and inspect the active preset. Everything is server-side: the cvars are
/// replicated to the client, which predicts movement with them. The client is never touched.
/// </summary>
public sealed class Cs2CsgoMovementPlugin : BasePlugin
{
    public override string ModuleName => "cs2-csgo-movement";
    public override string ModuleVersion => "0.1.0";
    public override string ModuleAuthor => "Chrystoval1";
    public override string ModuleDescription =>
        "Server-side CS:GO-like movement profiles for CS2.";

    private MovementProfile _active = MovementProfiles.Default;

    public override void Load(bool hotReload)
    {
        // Cvars can reset between maps, so reapply the active profile on every map start.
        RegisterListener<Listeners.OnMapStart>(_ => Apply(_active));

        AddCommand("csgomove_apply", $"Apply a movement preset: {MovementProfiles.Names}", OnApply);
        AddCommand("csgomove_toggle", "Toggle between the csgo and vanilla presets", OnToggle);
        AddCommand("csgomove_status", "Print the active movement preset", OnStatus);

        if (hotReload)
            Apply(_active);
    }

    private void OnApply(CCSPlayerController? player, CommandInfo command)
    {
        var name = command.ArgCount > 1 ? command.GetArg(1) : string.Empty;

        if (!MovementProfiles.TryGet(name, out var profile))
        {
            ReplyTo(player, $"Unknown preset '{name}'. Available: {MovementProfiles.Names}.");
            return;
        }

        Apply(profile);
    }

    private void OnToggle(CCSPlayerController? player, CommandInfo command)
    {
        var next = string.Equals(_active.Name, MovementProfiles.Csgo.Name, StringComparison.OrdinalIgnoreCase)
            ? MovementProfiles.Vanilla
            : MovementProfiles.Csgo;

        Apply(next);
    }

    private void OnStatus(CCSPlayerController? player, CommandInfo command) =>
        ReplyTo(player, $"Active movement preset: {_active.Name}.");

    private void Apply(MovementProfile profile)
    {
        _active = profile;

        foreach (var cvar in profile.ConVars)
            Server.ExecuteCommand($"{cvar.Name} {cvar.Value}");

        Server.PrintToChatAll($"[cs2-csgo-movement] applied preset: {profile.Name}");
    }

    private static void ReplyTo(CCSPlayerController? player, string message)
    {
        if (player is { IsValid: true })
            player.PrintToChat($"[cs2-csgo-movement] {message}");
        else
            Server.PrintToConsole($"[cs2-csgo-movement] {message}");
    }
}
