using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using TaskForge.Api.Models.Health;

namespace TaskForge.Api.Controllers
{
    /// <summary>
    /// Endpoints operacionais para monitoramento da API (liveness e readiness).
    /// Não exigem autenticação — usados por Docker, Kubernetes e ferramentas de observabilidade.
    /// </summary>
    [ApiController]
    [Route("health")]
    [AllowAnonymous]
    [Tags("Health")]
    [Produces("application/json")]
    public class HealthController(HealthCheckService healthCheckService) : ControllerBase
    {
        private readonly HealthCheckService _healthCheckService = healthCheckService;

        /// <summary>
        /// Liveness — o processo da API está em execução?
        /// </summary>
        /// <remarks>
        /// **Função:** probe de *liveness* para orquestradores (Docker, Kubernetes).
        ///
        /// **O que verifica:** apenas se a aplicação responde a requisições HTTP.
        ///
        /// **O que NÃO verifica:** banco de dados ou outros serviços externos.
        ///
        /// **Quando usar:** o container deve ser reiniciado se este endpoint falhar.
        ///
        /// **HTTP 200** = processo saudável.
        /// </remarks>
        [HttpGet("live")]
        [ProducesResponseType(typeof(HealthProbeResponse), StatusCodes.Status200OK)]
        public ActionResult<HealthProbeResponse> Live()
        {
            var response = new HealthProbeResponse(
                Endpoint: "live",
                Purpose: "Liveness",
                Description: "Confirma que o processo da API está em execução e aceitando requisições.",
                Status: HealthStatus.Healthy.ToString(),
                Summary: "A aplicação está viva. Nenhuma dependência externa é verificada neste endpoint.",
                Dependencies: null,
                CheckedAtUtc: DateTimeOffset.UtcNow
            );

            return Ok(response);
        }

        /// <summary>
        /// Readiness — a API está pronta para receber tráfego?
        /// </summary>
        /// <remarks>
        /// **Função:** probe de *readiness* para orquestradores e balanceadores.
        ///
        /// **O que verifica:** conectividade com o PostgreSQL (operações de leitura/escrita).
        ///
        /// **Quando usar:** tráfego só deve ser encaminhado quando este endpoint retorna saudável.
        ///
        /// **HTTP 200** = pronta para receber requisições.
        ///
        /// **HTTP 503** = dependência indisponível; aguarde ou investigue o banco.
        /// </remarks>
        [HttpGet("ready")]
        [ProducesResponseType(typeof(HealthProbeResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(HealthProbeResponse), StatusCodes.Status503ServiceUnavailable)]
        public async Task<ActionResult<HealthProbeResponse>> Ready(CancellationToken cancellationToken)
        {
            var report = await _healthCheckService.CheckHealthAsync(
                registration => registration.Tags.Contains("ready"),
                cancellationToken);

            var dependencies = report.Entries.Select(entry => new HealthDependencyDetail(
                Name: entry.Key,
                Purpose: entry.Key switch
                {
                    "postgresql" => "Verifica conexão e consulta básica ao PostgreSQL.",
                    _ => "Verificação de dependência registrada no health check."
                },
                Status: entry.Value.Status.ToString(),
                Error: entry.Value.Exception?.Message
            )).ToList();

            var isHealthy = report.Status == HealthStatus.Healthy;

            var response = new HealthProbeResponse(
                Endpoint: "ready",
                Purpose: "Readiness",
                Description: "Confirma que a API e suas dependências críticas estão prontas para atender requisições.",
                Status: report.Status.ToString(),
                Summary: isHealthy
                    ? "A aplicação está pronta. Todas as dependências verificadas estão saudáveis."
                    : "A aplicação não está pronta. Uma ou mais dependências falharam.",
                Dependencies: dependencies,
                CheckedAtUtc: DateTimeOffset.UtcNow
            );

            return isHealthy ? Ok(response) : StatusCode(StatusCodes.Status503ServiceUnavailable, response);
        }
    }
}
