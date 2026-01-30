using MediatR;

namespace BuildingBlocks.Core.CQRS
{   
    // Za Endpoint koji vraca nesto klijentu nakon izmene u bazi
    public interface ICommand<out TResponse> : IRequest<TResponse> where TResponse: notnull;

    // Za Endpoint koji ne vraca nista klijentu nakon izmene u bazi
    public interface ICommand : ICommand<Unit>; 
}
