using System;
using System.Collections.Generic;
using System.Linq;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Reports.Entities.B_Other;

namespace net91com.Reports.Services.CommonServices.B_BaseTool
{
    public class B_BaseToolService : BaseService
    {
        private static B_BaseToolService instance = null;
        private static readonly object obj = new object();

        public static B_BaseToolService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new B_BaseToolService();
                            instance._cachePreviousKey = "net91com_Reports_Services_CommonServices_B_BaseTool_B_BaseToolService";
                        }
                    }
                }
                return instance;
            }
        }

        #region 获取地区基础数据
        /// <summary>
        /// 获取地区的信息
        /// </summary>
        /// <returns></returns>
        public List<B_AreaEntity> GetAreaCache()
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_AreaEntity>>
                    (BuildCacheKey("GetArea"), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetArea());
        }
        #endregion

        /// <summary>
        /// 获取所有国家
        /// </summary>
        /// <returns></returns>
        public List<B_AreaEntity> GetCountriesCache(int flag = 0)
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_AreaEntity>>
                    (BuildCacheKey("GetCountries",flag), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetCountries(flag));
        }

        /// <summary>
        /// 获取所有省
        /// </summary>
        /// <returns></returns>
        public List<B_AreaEntity> GetProvincesCache(int flag=0)
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_AreaEntity>>
                    (BuildCacheKey("GetProvinces",flag), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetProvinces(flag));
        }

        /// <summary>
        /// 获取所有市
        /// </summary>
        /// <returns></returns>
        public List<B_AreaEntity> GetCitiesCache(int flag = 0)
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_AreaEntity>>
                    (BuildCacheKey("GetCities",flag), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetCities());
        }

        /// <summary>
        /// 获取地区信息按ID集合
        /// </summary>
        /// <param name="lst"></param>
        /// <returns></returns>
        public List<B_AreaEntity> GetAreabyidCache(List<int> lst)
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_AreaEntity>>
                    (BuildCacheKey("GetAreabyidCache", String.Join(",", lst.Select(p => p.ToString()).ToArray())), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetAreabyid(lst)).ToList();
        }

        #region 获取资源类型 大小分类基础数据
        /// <summary>
        /// 获取分类相关的大小分类信息
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<B_ResCateEntity> GetResCateCache(int type)
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_ResCateEntity>>
                    (BuildCacheKey("GetResCateCache", type), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetResCate(type));
        }

        #endregion

        #region 获取版本基础数据
        /// <summary>
        /// 获取版本信息
        /// </summary>
        /// <param name="softid"></param>
        /// <param name="platform"></param>
        /// <returns></returns>
        public List<B_VersionEntity> GetVersionCache(int softid, int platform)
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_VersionEntity>>
                    (BuildCacheKey("GetVersionCache", softid, platform), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetVersions()).Where(p => p.SoftID == softid && p.Platform == platform).ToList();
        }

        public List<B_VersionEntity> GetVersionCache(List<int> lst )
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_VersionEntity>>
                    (BuildCacheKey("GetVersionCache", String.Join(",",lst.Select(p=>p.ToString()).ToArray())), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetVersions(lst)).ToList();
        }




        

        public List<B_AuthorEntity> GetAuthorEntityCache()
        {
            return net91com.Core.Web.CacheHelper.Get<List<B_AuthorEntity>>
                    (BuildCacheKey("GetAuthorEntityCache"), Core.CacheTimeOption.TenMinutes,
                     () => new B_BaseTool_DataAccess().GetAuthorEntity()).ToList();
        }

        #endregion


        #region 通用工具类
        /// <summary>
        /// 获取所要周的日期
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="weekday"></param>
        /// <param name="Number">-1表示上一周，0当前周，1下一周 以此类推</param>
        /// <returns>所要周的日期</returns>
        public static DateTime GetWeekUpOfDate(DateTime dt, DayOfWeek weekday, int Number)
        {
            int wd1 = (int)weekday;
            int wd2 = (int)dt.DayOfWeek;
            return wd2 == wd1 ? dt.AddDays(7 * Number) : dt.AddDays(7 * Number - wd2 + wd1);
        }
        #endregion

        public List<Dictionary<string, string>> getExtendResAttr(int restype)
        {
            return net91com.Core.Web.CacheHelper.Get<List<Dictionary<string,string>>>
                           (BuildCacheKey("getExtendResAttr", restype), Core.CacheTimeOption.TenMinutes,
                            () => new B_BaseTool_DataAccess().getExtendResAttr(restype));
        }

        public List<B_ResInfo> GetResInfo(int restype, int areatype,List<string>  resindetifierlst,bool isresid=false)
        {
            if (restype==1)
            {
                isresid = false;
            }
            else
            {
                isresid = true;
            }
            return new B_BaseTool_DataAccess().GetResInfo(restype, areatype, resindetifierlst, isresid);
        }

        public List<B_ResInfo> GetResInfo2(int restype, int areatype, List<string> resindetifierlst, bool isresid = false)
        {
            return new B_BaseTool_DataAccess().GetResInfo2(restype, areatype, resindetifierlst, isresid);
        }
    }
}
