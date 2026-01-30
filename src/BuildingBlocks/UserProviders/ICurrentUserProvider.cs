namespace BuildingBlocks.UserProviders
{
    public interface ICurrentUserProvider
    {
        string? GetCurrentUserId();
    }
}
