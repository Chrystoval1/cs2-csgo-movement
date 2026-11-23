# Why it works — subtick, honestly

This document explains *what this plugin actually does* and *why*, so you can judge it
instead of trusting a marketing claim. It is backed by static reverse engineering of the
CS2 server binary. Where something is confirmed from the binary it is marked **confirmed**;
where it is reasoned, it is marked **inferred**.

No Valve code or binaries are redistributed here — only a conceptual description and the
public console-variable names.

## The one-paragraph version

CS2's "subtick" is **not** something a server can switch off — it is structural. What a
server *can* do is **temper how subtick affects movement** using two official cvars. This
plugin bundles those two levers with sane movement values into named presets. It makes the
movement feel *closer to* CS:GO; it does **not** turn CS2 into CS:GO, and it does not touch
how shooting is timed.

## What subtick is (confirmed)

In CS:GO, a server tick processes one input state: whatever you were holding at the tick
boundary. In CS2, each input carries a **sub-tick timestamp** — the fraction of the tick at
which it happened.

The CS2 server movement routine (`CPlayer_MovementServices::DoMovement`) walks an ordered
list of "subtick move steps" within a single tick. Each step carries:

- a fraction in `[0, 1]` — *when* inside the tick the input happened;
- a movement button/axis code;
- a pressed/released flag.

For each step the engine computes an absolute time of roughly `(tick + fraction) / 64` and
**re-integrates movement for that fractional slice**. In other words, a single tick is
chopped into sub-slices at the exact instants you pressed or released a key.

## Why that changes the feel (inferred, consistent with the above)

CS:GO movement is integrated **once per tick** from a single input state → deterministic.
CS2 integrates movement **per fractional slice**, so the result of a precise action — a
counter-strafe stop, a peek — depends on *when within the tick* you released the key. That
variability is what long-time CS:GO players read as "less crisp". It is not a bug; it is the
direct consequence of the per-slice integration.

## The two levers (confirmed to exist; effect to be measured)

CS2 exposes two server cvars that reduce subtick's grip on movement:

| Cvar | What the binary says it does |
|---|---|
| `sv_quantize_movement_input` | *"Quantize movement input values. Enabling this restricts players from using analog input to move at fractional speeds normally impossible with digital button input."* (verbatim from the binary.) A dedicated `CSMovementVelocityQuantizer_t` subsystem snaps the resulting velocity onto discrete values — i.e. back to CS:GO-style digital speeds. |
| `sv_subtick_movement_view_angles` | *"Whether or not subtick view angles are taken into account during movement."* (verbatim.) Set to `0` to drop subtick view-angle influence during movement. |

Setting `sv_quantize_movement_input 1` and `sv_subtick_movement_view_angles 0` is the entire
"subtick" part of the `csgo` preset. Everything else in the preset is standard competitive
movement values.

## Honest limits

- **Subtick is not removed.** These cvars temper the *movement* side only. Shooting stays
  subtick-timestamped — and that part is genuinely fairer, so you would not want to remove it
  even if you could.
- **This is an approximation.** Source 2 ≠ Source 1. The goal is "as close as a server can
  get", not a 1:1 copy.
- **The magnitude is empirical.** The reverse confirms these levers *exist* and what they are
  *for*; it does not prove exactly how strong the effect feels. Use `csgomove_toggle` to A/B
  the `csgo` and `vanilla` presets (identical movement values, only the subtick levers differ)
  and judge for yourself.
- **CS2 updates can shift cvar behaviour.** Re-test after a game update.

## Reproduce the analysis yourself

- Counter-Strike 2 ships the server logic in `server.dll` (Windows) / `.so` (Linux).
- Open it in a disassembler (IDA Pro, Ghidra) and search the strings `sv_quantize_movement_input`,
  `CSMovementVelocityQuantizer_t`, and `CPlayer_MovementServices::DoMovement::Subtick`.
- The cvar descriptions are stored as plain strings next to their registration; the movement
  loop is the function referencing the `DoMovement::Subtick` profiling scope.
