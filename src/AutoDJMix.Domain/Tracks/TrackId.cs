namespace AutoDJMix.Domain.Tracks;

public readonly record struct TrackId(Guid Value)
{
    public static TrackId New() => new(Guid.NewGuid());
}
