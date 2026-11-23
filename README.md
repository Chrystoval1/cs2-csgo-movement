# cs2-csgo-movement

A server-side [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) plugin
that brings a **CS:GO-like movement feel** to a Counter-Strike 2 community server.

- ✅ 100% server-side — the client is never modified.
- ✅ No memory reading, no injection, no VAC bypass.
- ✅ Switchable presets, with a one-command A/B toggle to feel the difference.

> **Honest scope.** This makes CS2 movement feel *closer to* CS:GO. It does **not** turn CS2
> into CS:GO, and it does **not** remove subtick (that is impossible server-side). What it
> does is temper subtick's effect on movement using two official cvars, and ship sane
> movement values alongside them. See [docs/why-it-works.md](docs/why-it-works.md) for the
> reverse-engineering-backed explanation.

## How it works in one line

Movement cvars are server-side and **replicated** to the client; the client **predicts**
movement with them. Change the server, and the player feels it instantly — no client mod
needed.

## Presets

| Preset | What it does |
|---|---|
| `csgo` | CS:GO-like feel: subtick tempered (`sv_quantize_movement_input 1`, `sv_subtick_movement_view_angles 0`) + standard competitive movement values. **Applied on load.** |
| `vanilla` | Stock CS2 subtick behaviour. Movement values are identical to `csgo`, so toggling isolates the subtick levers for a clean A/B test. |
| `surf` | Experimental: high air acceleration + bunnyhopping for surf maps. Starting point, tune to your maps. |

## Requirements

- A CS2 **dedicated server** launched with `-insecure` (no VAC in the loop).
- [Metamod:Source](https://www.sourcemm.net) (CS2 build).
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) ("with runtime" release).

## Install

1. Build the plugin (see [Build](#build)) or grab a release.
2. Copy the output into your server:
   ```
   game/csgo/addons/counterstrikesharp/plugins/Cs2CsgoMovement/
   ```
3. Start (or restart) the server. The `csgo` preset is applied automatically on load and on
   every map start.

### Prefer plain configs?

If you do not want the plugin, the same profiles exist as standalone configs in
[`cfg/`](cfg/). Copy one into `game/csgo/cfg/` and run `exec csgo` (or `surf`, `vanilla`)
from the server console.

## Commands

Run these from the server console (or RCON):

| Command | Description |
|---|---|
| `csgomove_apply <preset>` | Apply a preset: `csgo`, `surf`, or `vanilla`. |
| `csgomove_toggle` | Toggle between the `csgo` and `vanilla` presets. |
| `csgomove_status` | Print the active preset. |

### Feel the difference

1. Join the server and move around on the `csgo` preset.
2. Run `csgomove_toggle` to switch to `vanilla` and move again.
3. Because both presets share the same movement numbers, anything you feel is **purely the
   subtick levers**.

## Build

Requires the .NET 8 SDK.

```bash
dotnet build -c Release src/Cs2CsgoMovement/Cs2CsgoMovement.csproj
```

The plugin DLL is produced under `src/Cs2CsgoMovement/bin/Release/net8.0/`.

> The `CounterStrikeSharp.API` version is pinned in the `.csproj`. Bump it to match the
> CounterStrikeSharp runtime installed on your server if they differ.

## Tuning

Every numeric value is a **starting point**, not gospel. Edit the presets in
[`src/Cs2CsgoMovement/MovementProfiles.cs`](src/Cs2CsgoMovement/MovementProfiles.cs) (or the
`cfg/` files), rebuild, and re-test. CS2 game updates can change cvar behaviour, so re-check
the feel after updating your server.

## License

[MIT](LICENSE).
