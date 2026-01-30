using MediatR;

namespace BuildingBlocks.Core.CQRS
{   
    // Za Endpoint koji vraca nesto klijentu nakon izmene u bazi
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Unit>
     where TCommand : ICommand<Unit>
    {
    }

    // Za Endpoint koji ne vraca nista klijentu nakon izmene u bazi
    public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
        where TResponse : notnull
    {
    }
}
