namespace SomoTaskManagement.Cache
{
    public interface ICacheService
    {
        Task<string> GetCacheResponseAsynce(string cacheKey);
        Task SetCacheResponseAsync(string cacheKey, object response, TimeSpan timeOut);
    }
}