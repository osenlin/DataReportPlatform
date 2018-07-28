using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace net91com.Stat.Web.Reports.Services
{
    public class SeriesJsonModel
    {
        public string name { get; set; }
        public List<object> data = new List<object>();
        public bool visible = true;

    }

    public class DataLabels
    {
        public SmallDataLabels dataLabels { get; set; }
        public double y { get; set; }
       
        ///通用的百分比都是这个属性表示
        public string growth = "0.000%";
        
        ///下面这个都是作为分母，分子就是y
        public long Denominator;

        /// <summary>
        /// 显示时间(对比时间的时候才用)
        /// </summary>
        public string Datestr = "";

        public MarkerBase marker;

        public string Other;

    }

    public class SmallDataLabels
    {
        public bool enabled = false;
        //public string align = "left";
        //public int x = 0;
        //public int y = -6;
        //public int rotation = -45;
    }

    public class ColumnModel
    {
        /// <summary>
        /// 柱形名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 数据列表
        /// </summary>
        public List<int> data { get; set; }
        /// <summary>
        /// 是否分组累加
        /// </summary>
        public string stack { get; set; }



    }

    public class MarkerBase
    {
        public string marks;
    }

    public class MarkerForForecast : MarkerBase
    {
        public bool enabled;
        public string fillColor;
        public string lineColor;
        public int radius;
        public double lineWidth;
        public string symbol;
        /// <summary>
        /// 是否是预测点，若不是预测点则是特别点
        /// </summary>
        /// <param name="Forecast"></param>
        public MarkerForForecast()
        {
            
                this.enabled = true;
                ///填充空白
                this.fillColor = "#FFFFFF";
                this.lineColor = "#FF0000";
                this.lineWidth = 0.5;
                this.radius = 6;

        }

    }
    public class MarkerForSuper : MarkerBase
    { 
        public string symbol; 
        public MarkerForSuper()
        {
            this.symbol = "url(/Images/sun.png)";
        }

    }

    ///字符串显示格式如下
    //[{
    //       name:"熊猫看书";
    //      {
    //    data: [29.9, 71.5, 106.4, 129.2, 144.0, 176.0, 135.6, 148.5, 216.4, 194.1, 95.6, {
    //        dataLabels: {
    //            enabled: true,
    //            align: 'left',
    //            x: 10,
    //            y: 4,
    //            style: {
    //                fontWeight: 'bold'
    //            },
            //       marker: {
            //    enabled:true,
            //    fillColor: '#FFFFFF',
            //    lineWidth: 2,
            //    lineColor: '#FF0000', // inherit from series
            //    radius: 4
            //} 
    //        },
    //        y: 54.4
    //    }
    //
    //}]
}