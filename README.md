# cs2-csgo-movement

A server-side [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) plugin
that makes Counter-Strike 2 movement feel closer to CS:GO. It runs entirely on the server —
the client is never modified and there is no VAC bypass.

It does not turn CS2 into CS:GO, and it cannot remove subtick (no server can). What it does
is temper subtick's effect on movement with two official cvars and ship them, alongside
standard movement values, as named presets. The reasoning is in
[docs/why-it-works.md](docs/why-it-works.md).

## How it works

Movement cvars are server-side and replicated to the client, which predicts movement with
them. Change the server and the player feels it immediately, with no client-side mod.

## Presets

- `csgo` — subtick tempered (`sv_quantize_movement_input 1`, `sv_subtick_movement_view_angles 0`)
  with standard competitive movement values. Applied on load.
- `vanilla` — stock CS2 subtick. Same movement values as `csgo`, so switching between the two
  isolates the subtick levers.
- `surf` — experimental: high air acceleration and bunnyhopping for surf maps.

## Requirements

- A CS2 dedicated server launched with `-insecure`.
- [Metamod:Source](https://www.sourcemm.net) (CS2 build).
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp) (with-runtime release).

## Install

Build the plugin (see [Build](#build)) and copy the output into:

```
game/csgo/addons/counterstrikesharp/plugins/Cs2CsgoMovement/
```

Restart the server. The `csgo` preset is applied on load and on every map change.

If you would rather not run the plugin, the same profiles live in [`cfg/`](cfg/). Copy one
into `game/csgo/cfg/` and run `exec csgo` (or `surf`, `vanilla`) from the console.

## Commands

- `csgomove_apply <csgo|surf|vanilla>` — apply a preset.
- `csgomove_toggle` — switch between `csgo` and `vanilla`.
- `csgomove_status` — print the active preset.

Because `csgo` and `vanilla` share the same movement values, toggling between them is a clean
A/B test of the subtick levers.

## Build

Requires the .NET 8 SDK.

```
dotnet build -c Release src/Cs2CsgoMovement/Cs2CsgoMovement.csproj
```

The DLL lands in `src/Cs2CsgoMovement/bin/Release/net8.0/`. The `CounterStrikeSharp.API`
version is pinned in the csproj; bump it to match the runtime on your server.

## Tuning

The preset values are a starting point, not fixed constants. Edit
[`src/Cs2CsgoMovement/MovementProfiles.cs`](src/Cs2CsgoMovement/MovementProfiles.cs) or the
`cfg/` files, rebuild, and re-test. CS2 updates can change cvar behaviour, so re-check after
updating the server.

## License

[MIT](LICENSE).
