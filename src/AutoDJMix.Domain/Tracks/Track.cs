namespace AutoDJMix.Domain.Tracks;

public sealed record Track(
    TrackId Id,
    string SourcePath,
    string DisplayName,
    TimeSpan Duration,
    string ContentHashSha256,
    RoleAssignment RoleAssignment);
