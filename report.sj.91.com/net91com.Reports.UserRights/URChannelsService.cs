using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core.Web;

namespace net91com.Reports.UserRights
{
    /// <summary>
    /// 渠道相关方法
    /// </summary>
    public class URChannelsService
    {
        private URLoginService loginService = new URLoginService();

        #region 获取有权限的渠道ID列表(未登录)(GetChannelIds)

        /// <summary>
        /// 获取指定软件指定渠道绑定的渠道ID列表(未登录)
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="channelType"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        public List<int> GetChannelIds(int softId, ChannelTypeOptions channelType, int[] channels)
        {
            return DAChannelsHelper.GetChannelIds(softId, channelType, channels);
        }

        #endregion

        #region 获取指定产品的渠道列表(GetChannels)

        /// <summary>
        /// 获取指定产品的渠道列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="includeChannelIds">是否包含渠道ID</param>
        /// <returns></returns>
        public List<Channel> GetChannels(int softId, bool includeChannelIds)
        {
            if (loginService.LoginUser.AccountType == UserTypeOptions.Channel
                || loginService.LoginUser.AccountType == UserTypeOptions.ChannelPartner)
                throw new NotRightException();

            if ((loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                 || loginService.LoginUser.AccountType == UserTypeOptions.General)
                && !loginService.AvailableSofts.Exists(a => a.ID == softId))
                throw new NotRightException();

            return DAChannelsHelper.GetChannels(softId, includeChannelIds);
        }

        /// <summary>
        /// 获取指定产品的渠道列表(KEY则为渠道类型加ID值)
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="includeChannelIds"></param>
        /// <returns></returns>
        public Dictionary<string, Channel> GetChannelDict(int softId, bool includeChannelIds)
        {
            string cacheKey = string.Format("net91com.Reports.UserRights.URChannelsService.GetChannelDict_{0}_{1}",
                                            softId, includeChannelIds);
            if (CacheHelper.Contains(cacheKey))
                return CacheHelper.Get<Dictionary<string, Channel>>(cacheKey);
            Dictionary<string, Channel> channelDict = new Dictionary<string, Channel>();
            List<Channel> channels = DAChannelsHelper.GetChannels(softId, includeChannelIds);
            foreach (Channel channel in channels)
            {
                string key = channel.ChannelType.ToString() + channel.ID.ToString();
                channelDict[key] = channel;
            }
            CacheHelper.Set<Dictionary<string, Channel>>(cacheKey, channelDict, Core.CacheTimeOption.TenMinutes,
                                                         Core.CacheExpirationOption.AbsoluteExpiration);
            return channelDict;
        }

        /// <summary>
        /// 获取指定渠道的子渠道列表(KEY则为渠道类型加ID值)
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="channelType"></param>
        /// <param name="channelId"></param>
        /// <param name="includeChannelIds"></param>
        /// <returns></returns>
        public Dictionary<string, Channel> GetSubChannelDict(int softId, ChannelTypeOptions channelType, int channelId,
                                                             bool includeChannelIds)
        {
            Dictionary<string, Channel> channelDict = GetChannelDict(softId, includeChannelIds);
            Dictionary<string, Channel> subChannelDict = new Dictionary<string, Channel>();
            foreach (Channel channel in channelDict.Values)
            {
                if (channel.ChannelType == channelType && channel.ID == channelId) continue;
                string parentKey = channel.ParentChannelType.ToString() + channel.ParentID.ToString();
                Channel parentChannel;
                while (channelDict.ContainsKey(parentKey))
                {
                    parentChannel = channelDict[parentKey];
                    if (parentChannel.ChannelType == channelType && parentChannel.ID == channelId)
                    {
                        subChannelDict[channel.ChannelType.ToString() + channel.ID.ToString()] = channel;
                        break;
                    }
                    parentKey = parentChannel.ParentChannelType.ToString() + parentChannel.ParentID.ToString();
                }
            }
            return subChannelDict;
        }

        /// <summary>
        /// 获取指定产品的渠道列表(如果是渠道ID,KEY则为平台加渠道ID;如果渠道商或分类,KEY则为渠道类型加ID值)
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="includeChannelIds"></param>
        /// <returns></returns>
        public Dictionary<string, Channel> GetChannelDict2(int softId, bool includeChannelIds)
        {
            string cacheKey = string.Format("net91com.Reports.UserRights.URChannelsService.GetChannelDict2_{0}_{1}",
                                            softId, includeChannelIds);
            if (CacheHelper.Contains(cacheKey))
                return CacheHelper.Get<Dictionary<string, Channel>>(cacheKey);
            Dictionary<string, Channel> channelDict = new Dictionary<string, Channel>();
            List<Channel> channels = DAChannelsHelper.GetChannels(softId, includeChannelIds);
            foreach (Channel channel in channels)
            {
                string key = channel.ChannelType == ChannelTypeOptions.ChannelID
                                 ? channel.Platform.ToString() + channel.Name.ToString()
                                 : channel.ChannelType.ToString() + channel.ID.ToString();
                channelDict[key] = channel;
            }
            CacheHelper.Set<Dictionary<string, Channel>>(cacheKey, channelDict, Core.CacheTimeOption.TenMinutes,
                                                         Core.CacheExpirationOption.AbsoluteExpiration);
            return channelDict;
        }

        #endregion

        #region 获取指定渠道有权限的用户列表(GetUserIds)

        /// <summary>
        /// 获取指定渠道有权限的用户列表
        /// </summary>
        /// <param name="softId"></param>
        /// <param name="channelRight"></param>
        /// <returns></returns>
        public List<int> GetUserIds(int softId, ChannelRight channelRight)
        {
            loginService.HaveAdminRight(DACommonHelper.REPORT_SYS_ID);

            if (loginService.LoginUser.AccountType == UserTypeOptions.ProductAdmin
                && !loginService.AvailableSofts.Exists(a => a.ID == softId))
                throw new NotRightException();

            return DAChannelsHelper.GetUserIds(softId, channelRight);
        }

        #endregion

        #region 权限相关方法

        /// <summary>
        /// 授予用户特定渠道权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <param name="channelRights"></param>
        public void AddUserChannelRights(int userId, int softId, List<ChannelRight> channelRights)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrantSoft(userId, softId);

            DAChannelsHelper.AddUserChannelRights(userId, softId, channelRights);

            //记录登录日志
            loginService.AddLog(
                "AddUserChannelRights",
                string.Format("添加用户渠道权限(UserID={0},SoftID={1})", userId, softId));
        }

        /// <summary>
        /// 授予用户特定渠道权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <param name="channelRight"></param>
        public void AddUserChannelRight(int userId, int softId, ChannelRight channelRight)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrantSoft(userId, softId);

            DAChannelsHelper.AddUserChannelRight(userId, softId, channelRight);

            //记录登录日志
            loginService.AddLog(
                "AddUserChannelRight",
                string.Format("添加用户渠道权限(UserID={0},SoftID={1},ChannelType={2},ChannelID={3})", userId, softId,
                              channelRight.ChannelType, channelRight.ChannelID));
        }

        /// <summary>
        /// 授予用户特定渠道权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <param name="channelRight"></param>
        public void DeleteUserChannelRight(int userId, int softId, ChannelRight channelRight)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrantSoft(userId, softId);

            DAChannelsHelper.DeleteUserChannelRight(userId, softId, channelRight);

            //记录登录日志
            loginService.AddLog(
                "DeleteUserChannelRight",
                string.Format("删除用户渠道权限(UserID={0},SoftID={1},ChannelType={2},ChannelID={3})", userId, softId,
                              channelRight.ChannelType, channelRight.ChannelID));
        }

        /// <summary>
        /// 获取用户资源权限
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="softId"></param>
        /// <returns></returns>
        public List<ChannelRight> GetUserChannelRights(int userId, int softId)
        {
            //权限判断
            loginService.HaveAdminRightForUserGrantSoft(userId, softId);

            return DAChannelsHelper.GetUserChannelRights(userId, softId);
        }

        #endregion
    }
}