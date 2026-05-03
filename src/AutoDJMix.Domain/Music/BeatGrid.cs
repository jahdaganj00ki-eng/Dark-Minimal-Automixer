namespace AutoDJMix.Domain.Music;

public sealed record BeatGrid(TimeSpan StartTime, TimeSpan BeatInterval, double DriftPpm);
