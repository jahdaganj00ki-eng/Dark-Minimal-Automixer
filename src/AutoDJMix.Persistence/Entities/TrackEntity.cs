using System.ComponentModel.DataAnnotations;

namespace AutoDJMix.Persistence.Entities;

public sealed class TrackEntity
{
    [Key]
    public Guid Id { get; set; }

    [Required]
    public string SourcePath { get; set; } = string.Empty;

    [Required]
    public string DisplayName { get; set; } = string.Empty;

    public long DurationMs { get; set; }

    [Required]
    public string ContentHashSha256 { get; set; } = string.Empty;
}
