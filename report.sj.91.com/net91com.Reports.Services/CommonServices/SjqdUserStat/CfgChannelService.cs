using net91com.Reports.DataAccess.SjqdUserStat;
using net91com.Reports.Entities.DataBaseEntity;

namespace net91com.Reports.Services.CommonServices.SjqdUserStat
{
    /// <summary>
    ///     渠道商信息业务接口类
    /// </summary>
    public class CfgChannelService
    {
        /// <summary>
        ///     获取渠道商对象根据渠道商id(包括准确的cateid，softid)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Sjqd_ChannelCustomers GetChannelCustomer(int id)
        {
            return SjqdChannelCustomers_DataAccess.Instance.GetChannelCustomer(id);
        }

        /// <summary>
        ///     根据渠道分类ID获取分类信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Sjqd_ChannelCategories GetChannelCategory(int id)
        {
            return SjqdChannelCategories_DataAccess.Instance.GetChannelCategory(id);
        }
    }
}