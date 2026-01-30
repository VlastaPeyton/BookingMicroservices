using System.Text.Json;
using BuildingBlocks.Core.CQRS;
using EasyCaching.Core;
using MediatR;

namespace BuildingBlocks.MediatorBehaviours.Caching
{   
    // MediatR caching behaviour sme samo za Query, nikako za Command !
    // Cache-aside pattern with only auto self-invalidatin cache => ne treba mi InvalidateCacheBehaviour 
    public class CachingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull, IQuery<TResponse>
        where TResponse : notnull
    {
        private readonly IEasyCachingProvider _cachingProvider;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromHours(1);

        public CachingBehaviour(IEasyCachingProviderFactory cachingProviderFactory)
        {
            _cachingProvider = cachingProviderFactory.GetCachingProvider("redis"); // Prvo ga registrujem ovim imenom u Program.cs
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var cacheKey = GenerateCacheKey(request); 

            var cachedResponse = await _cachingProvider.GetAsync<TResponse>(cacheKey, cancellationToken);
            if (cachedResponse.HasValue)
                return cachedResponse.Value;

            var response = await next(); // Nastavi kroz pipeline 

            await _cachingProvider.SetAsync(cacheKey, response, _defaultExpiration, cancellationToken);

            return response; 
        }

        private string GenerateCacheKey(TRequest request)
        {
            return $"{typeof(TRequest).FullName}:{JsonSerializer.Serialize(request)}";
        }
    }
}
