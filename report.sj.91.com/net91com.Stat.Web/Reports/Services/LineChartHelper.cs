using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using net91com.Stat.Services.Entity;

namespace net91com.Stat.Web.Reports.Services
{
    /// <summary>
    /// 曲线辅助类
    /// </summary>
    internal class LineChartHelper
    {
        #region 生成留存率曲线

        /// <summary>
        /// 生成留存率曲线
        /// </summary>
        /// <param name="users"></param>
        /// <param name="period"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="xJson"></param>
        /// <returns></returns>
        public static string BuildStatRetainedUsersLine(List<net91com.Reports.Entities.DataBaseEntity.Sjqd_StatRetainedUsers> users, net91com.Stat.Core.PeriodOptions period, DateTime startDate, DateTime endDate, out string xJson)
        {
            DateTime xBeginDate = users.Count > 0 ? users.Min(a => a.OriginalDate) : startDate;
            DateTime xEndDate = users.Count > 0 ? users.Max(a => a.StatDate) : endDate;
            LineChart chart = new LineChart(xBeginDate, xEndDate);
            chart.Period = period;
            var usersGrpByOrgDate = users.GroupBy(a => a.OriginalDate);
            for (int i = 0; i < usersGrpByOrgDate.Count(); i++)
            {
                var temp = usersGrpByOrgDate.ElementAt(i);
                List<LineChartPoint> points = temp.Select(
                    a => new LineChartPoint
                    {
                        XValue = a.StatDate,
                        YValue = a.OriginalNewUserCount == 0 ? 100 : (a.RetainedUserCount / (double)a.OriginalNewUserCount) * 100,
                        DataContext = a,
                        Denominator = a.OriginalNewUserCount,
                        Percent = a.OriginalNewUserCount == 0 ? "100.00" : ((a.RetainedUserCount / (double)a.OriginalNewUserCount) * 100).ToString("0.00") + "%"
                    }).ToList();
                points.Insert(0, new LineChartPoint { XValue = temp.Key, YValue = 100, Percent = "100%", Denominator = temp.ElementAt(0).OriginalNewUserCount });
                LineChartLine line = new LineChartLine
                {
                    Name = temp.Key.ToString("yyyy-MM-dd"),
                    Show = true,
                    XIntervalDays = 0,
                    Points = points
                };
                chart.Y.Add(line);
            }
            xJson = string.Format("{{{0},labels:{{align:'left',tickLength:80,tickPixelInterval:140,rotation:-45,x:-30,y:45,step:{1}}}}}"
                , chart.GetXJson()
                , chart.Step);

            return chart.GetYJson(
                delegate(LineChartPoint point)
                {
                    net91com.Reports.Entities.DataBaseEntity.Sjqd_StatRetainedUsers user = (net91com.Reports.Entities.DataBaseEntity.Sjqd_StatRetainedUsers)point.DataContext;
                    string temp = null;
                    switch (period)
                    {
                        case net91com.Stat.Core.PeriodOptions.Daily:
                            temp = user == null ? "当天" : "第" + user.StatDate.Subtract(user.OriginalDate).Days.ToString() + "天";
                            break;
                        case net91com.Stat.Core.PeriodOptions.Weekly:
                            temp = user == null ? "本周" : "第" + (user.StatDate.Subtract(user.OriginalDate).Days / 7).ToString() + "周";
                            break;
                        case net91com.Stat.Core.PeriodOptions.Monthly:
                        case net91com.Stat.Core.PeriodOptions.NaturalMonth:
                            temp = user == null ? "本月" : "第" + (user.StatDate.Subtract(user.OriginalDate).Days / 28).ToString() + "月";
                            break;
                    }
                    return string.Format(",\"growth\":\"{0}\",\"Denominator\":{1},\"Other\":\"{2}\"", point.Percent, point.Denominator, temp);
                });
        }

        #endregion
    }
}