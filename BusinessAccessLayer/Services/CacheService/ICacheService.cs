using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessAccessLayer.Services.CacheService
{
    public interface ICacheService
    {
        void SetCache<T>(string key, T value, TimeSpan expiration);
        T GetCache<T>(string key);
    }
}
