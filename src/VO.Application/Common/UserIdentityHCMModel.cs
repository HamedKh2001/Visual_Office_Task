using SharedKernel.Common;
using SharedKernel.Common.CacheModels;

namespace VO.Application.Common
{
    public class UserIdentityHCMModel : UserIdentitySharedModel
    {
        public UserCacheModel UserInfo { get; set; }
    }
}
