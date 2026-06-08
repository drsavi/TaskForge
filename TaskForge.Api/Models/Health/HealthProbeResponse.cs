namespace TaskForge.Api.Models.Health
{
    /// <summary>
    /// Resposta padronizada dos endpoints de health check.
    /// </summary>
    public record HealthProbeResponse(
        string Endpoint,
        string Purpose,
        string Description,
        string Status,
        string Summary,
        IReadOnlyList<HealthDependencyDetail>? Dependencies,
        DateTimeOffset CheckedAtUtc
    );

    public record HealthDependencyDetail(
        string Name,
        string Purpose,
        string Status,
        string? Error
    );
}
