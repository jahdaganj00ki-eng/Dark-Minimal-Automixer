namespace AutoDJMix.Domain.Tracks;

public enum RoleSource
{
    AnalysisAuto = 0,
    UserOverride = 1
}

public sealed record RoleAssignment(TrackRole Role, RoleSource Source, bool IsLocked, TrackRole? WeakHint);
