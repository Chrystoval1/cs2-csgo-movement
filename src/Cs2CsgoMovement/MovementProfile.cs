namespace Cs2CsgoMovement;

/// <summary>
/// An immutable, named set of movement console variables to apply to the server.
/// Each entry is a <c>(cvar, value)</c> pair applied verbatim via the server console.
/// </summary>
public sealed record MovementProfile(string Name, IReadOnlyList<ConVarSetting> ConVars);

/// <summary>A single console variable assignment.</summary>
public sealed record ConVarSetting(string Name, string Value);
