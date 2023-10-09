using Microsoft.Extensions.Caching.Memory;
using System.Text;

namespace Host.Services
{
    public class EncodingService : IEncodingService
    {
        private readonly ILogger<EncodingService> _logger;
        private readonly IMemoryCache _memoryCache;

        public EncodingService(ILogger<EncodingService> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public int StartEncodingAsync(string inputText)
        {
            try
            {
                // Encode the entire input string into Base64
                byte[] inputBytes = Encoding.UTF8.GetBytes(inputText);
                string encodedString = Convert.ToBase64String(inputBytes);

                var encodedChars = encodedString.ToList();

                _memoryCache.Set("EncodedCharacters", encodedChars, TimeSpan.FromMinutes(30));

                // Encoding completed successfully
                return encodedString.Length;
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "An error occurred during encoding.");
                throw new OperationCanceledException("Processing canceled.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred during encoding.");
                throw new Exception("An error occurred during encoding.");

            }
        }

        public string StopEncodingAsync()
        {
            const string cancelOperationInfo = "Encoding canceled";
            _memoryCache.Set("CancelOperation", true, TimeSpan.FromMilliseconds(5000));
            _memoryCache.Remove("EncodedCharacters");
            _logger.LogInformation(cancelOperationInfo);
            return cancelOperationInfo;
        }
    }
}
