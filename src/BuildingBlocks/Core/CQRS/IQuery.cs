using MediatR;

namespace BuildingBlocks.Core.CQRS
{
    // Nema IQuery koji ne vraca nista klijentu kao ICommand sto ima, jer Endpoint za citanje iz baze uvek sale klijentu nesto iz baze
    public interface IQuery<out TResponse> : IRequest<TResponse> where TResponse : notnull { }
}
