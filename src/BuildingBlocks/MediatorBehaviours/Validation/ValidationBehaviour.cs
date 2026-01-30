using BuildingBlocks.Core.CQRS;
using FluentValidation;
using MediatR;

namespace BuildingBlocks.MediatorBehaviours.Validation
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : ICommand<TResponse>
        where TResponse : notnull                                                                                
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;
        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var context = new ValidationContext<TRequest>(request);

            // Call ValidateAsync method for each Handle method - ide kroz CommandValidator i pokrece svaki RuleFor 
            var validationResults = await Task.WhenAll(_validators.Select(v => v.ValidateAsync(context, cancellationToken)));
            // Task.WhenAll = start all validators for same Command object at the same time (ako imam ugnedenu validaciju ili vise validatora za isti objekat -nemam nista od ovoga)

            // Check for any erro in validationResults 
            var failures = validationResults.Where(r => r.Errors.Any()).SelectMany(r => r.Errors).ToList();

            // If any error occured, throw ValidationException 
            if (failures.Any())
                throw new ValidationException(failures); // ValidationException je built-in. Propagira uzvodno do prvog catch koji se nalazi u GlobaleExceptionHandlingMiddleware

            // next() will run next MediatR pipeline behaviour (ako postoji) registrovan nakon ValidationBehaviour u Program.cs (a to je LoggingBehaviour)
            return await next();
        }
    }
}
