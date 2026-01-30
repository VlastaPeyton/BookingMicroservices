using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.MediatorBehaviours.Logging
{
    // MediatR pipeline behavior koji loguje svaki request i njegov response
    // Takodje meri vreme izvrsavanja i upozorava ako request traje predugo (>3 sekunde)
    public class LoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
        where TResponse : notnull
    {
        private readonly ILogger<LoggingBehaviour<TRequest, TResponse>> _logger;

        public LoggingBehaviour(ILogger<LoggingBehaviour<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            var responseName = typeof(TResponse).Name;

            // Log pocetak request-a
            _logger.LogInformation("[MediatR] Handling {RequestName} -> {ResponseName}", requestName, responseName);

            // Startuj timer za merenje vremena
            var stopwatch = Stopwatch.StartNew();

            var response = await next();

            stopwatch.Stop();

            // Ako request traje duze od 3 sekunde, loguj warning
            if (stopwatch.Elapsed.TotalSeconds > 3)
            {
                _logger.LogWarning("[MediatR] SLOW REQUEST: {RequestName} took {ElapsedSeconds} seconds",
                    requestName, stopwatch.Elapsed.TotalSeconds);
            }

            // Log zavrsetak request-a
            _logger.LogInformation("[MediatR] Handled {RequestName} in {ElapsedMs}ms",
                requestName, stopwatch.ElapsedMilliseconds);

            return response;
        }
    }
}
