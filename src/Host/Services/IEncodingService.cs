namespace Host.Services
{
    public interface IEncodingService
    {
        int StartEncodingAsync(string text);
        string StopEncodingAsync();
    }
}
