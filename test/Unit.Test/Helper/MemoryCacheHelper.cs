using Microsoft.Extensions.Caching.Memory;

namespace Unit.Test.Helper;

public class MemoryCacheHelper
{
    //helper method to mock IMemoryCache
    public Mock<IMemoryCache> MockCache()
    {
        var cache = new MemoryCache(new MemoryCacheOptions());
        var memoryCacheMock = new Mock<IMemoryCache>();
        memoryCacheMock.Setup(x => x.CreateEntry(It.IsAny<object>()))
            .Returns((object key) => cache.CreateEntry(key))
            .Verifiable();
        memoryCacheMock.Setup(x => x.TryGetValue(It.IsAny<object>(), out It.Ref<object>.IsAny))
            .Returns((object key, out object value) => cache.TryGetValue(key, out value))
            .Verifiable();
        memoryCacheMock.Setup(x => x.Remove(It.IsAny<object>()))
            .Callback((object key) => cache.Remove(key))
            .Verifiable();
        return memoryCacheMock;
    }
}
