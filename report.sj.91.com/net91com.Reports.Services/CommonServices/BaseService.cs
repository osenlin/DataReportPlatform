using System.Text;

namespace net91com.Reports.Services.CommonServices
{
    public abstract class BaseService
    {
        protected string _cachePreviousKey;

        protected string BuildCacheKey(params object[] args)
        {
            var sbCacheKey = new StringBuilder(_cachePreviousKey);
            foreach (object obj in args)
            {
                sbCacheKey.Append("_");
                sbCacheKey.Append(obj);
            }
            return sbCacheKey.ToString();
        }
    }
}