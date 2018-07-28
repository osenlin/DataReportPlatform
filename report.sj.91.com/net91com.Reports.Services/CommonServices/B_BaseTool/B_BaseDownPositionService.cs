using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net91com.Core;
using net91com.Reports.DataAccess.B_Basic;
using net91com.Reports.Entities.B_Other;
using net91com.Reports.UserRights;

namespace net91com.Reports.Services.CommonServices.B_BaseTool
{
    public class B_BaseDownPositionService : BaseService
    {
        private static B_BaseDownPositionService instance = null;
        private static readonly object obj = new object();

        public static B_BaseDownPositionService Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (obj)
                    {
                        if (instance == null)
                        {
                            instance = new B_BaseDownPositionService();
                            instance._cachePreviousKey = "net91com_Reports_Services_CommonServices_B_BaseTool_B_BaseDownPositionService";
                        }
                    }
                }
                return instance;
            }
        }

        public List<B_DownPositionEntity> GetB_DownPositionListByCache(ProjectSourceTypeOptions projectSourceType, int downtype, int istag,
                                                                            int restype, int project, string position,
                                                                            string name, string pagename, string pagetype, int beginsize,
                                                                            int pagesize,int softid2, out int count)
        {

            //return net91com.Core.Web.CacheHelper.Get<List<B_ResCateEntity>>
            //        (BuildCacheKey("GetResCateCache", type), Core.CacheTimeOption.TenMinutes,
            //         () => new B_ResCate_DataAccess().GetResCate(type));
            //string key = BuildCacheKey("GetB_DownPositionListByCache", projectSourceType, restype, project, position,
            //                           name, pagename, beginsize, pagesize);
            return new B_DownPosition_DataAccess().GetB_DownPositionList(projectSourceType, downtype, istag, restype, project, position,
                                                                         name, pagename, pagetype, beginsize, pagesize,softid2, out count);
        }


        public List<B_DownPositionEntity> GetB_DownPositionListByCache(ProjectSourceTypeOptions projectSourceType,
                                                                    int restype, int project,int softid=-1)
        {

            return  new B_DownPosition_DataAccess().GetB_DownPositionListByCache(projectSourceType, restype, project, softid);
        }

        public List<B_DownPositionEntity> GetB_DownAdPositionListByCache(int softid, int platform, int restype,int areatype,int pagetype)
        {
            string key = BuildCacheKey("GetB_DownAdPositionListByCache", softid, platform, restype, areatype, pagetype);
            return net91com.Core.Web.CacheHelper.Get<List<B_DownPositionEntity>>
                    (key, Core.CacheTimeOption.TenMinutes,
                     () => new B_DownPosition_DataAccess().GetAdPosition(softid, platform, restype, areatype, pagetype));
        }


        public B_DownPositionEntity GetB_DownPosition(ProjectSourceTypeOptions projectsourcetype, int restype, int projectsource, string positionid)
        {
            return new B_DownPosition_DataAccess().GetB_DownPosition(projectsourcetype, restype, projectsource, positionid);
        }

        public static int UpdatePosition2MySql(B_DownPositionEntity b_DownPositionEntity)
        {
            return new B_DownPosition_DataAccess().UpdatePosition2MySql(b_DownPositionEntity);
        }

        internal static int AddPosition2MySql(B_DownPositionEntity b_DownPositionEntity)
        {
            return new B_DownPosition_DataAccess().AddPosition2MySql(b_DownPositionEntity);
        }

        public static int BatchEditPositionName(B_DownPositionEntity bDownPositionEntity, string idnamelist)
        {
            return new B_DownPosition_DataAccess().BatchEditPositionName( bDownPositionEntity, idnamelist);
        }
    }
}
