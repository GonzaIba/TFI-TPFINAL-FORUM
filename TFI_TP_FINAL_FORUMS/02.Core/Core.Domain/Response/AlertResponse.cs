using System;
using System.Text.Json.Serialization;

namespace Core.Domain.Response
{
    public sealed class AlertResponse
    {
        public string Id { get; set; } = string.Empty;
        public string? DedupeKey { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Severity { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool? Sticky { get; set; }
        public AlertCtaResponse? Cta { get; set; }
        public string? ChannelSuggested { get; set; }
        public object? Data { get; set; }
    }

    public sealed class AlertCtaResponse
    {
        public string Label { get; set; } = string.Empty;
        public string Href { get; set; } = string.Empty;
    }
}
