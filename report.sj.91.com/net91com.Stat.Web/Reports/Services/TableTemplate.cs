using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace net91com.Stat.Web.Reports.Services
{
    /// <summary>
    /// 表格模板类
    /// </summary>
    public class TableTemplate
    {
        #region 有效的列信息节点(ColumnNode)

        /// <summary>
        /// 有效的列信息节点
        /// </summary>
        public class ColumnNode
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }

            /// <summary>
            /// 名称，根据这个绑定数据
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// 对应的XML节点
            /// </summary>
            public XmlNode CorrespondXmlNode { get; set; }

            /// <summary>
            /// 子节点
            /// </summary>
            public List<object> Children { get; set; }

            /// <summary>
            /// 包含的列数
            /// </summary>
            public int ColSpan { get; set; }

            /// <summary>
            /// 层级
            /// </summary>
            public int Level { get; set; }
        }

        #endregion

        #region 根据节点信息构造表格的头部(BuildTableHead)

        /// <summary>
        /// 根据节点信息构造表格的头部，并返回数据绑定列
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="IsTrue"></param>
        /// <param name="condition"></param>
        /// <param name="dataColumns"></param>
        /// <returns></returns>
        public static string BuildTableHead(ColumnNode rootNode,IsTrueHandler IsTrue, NodeCondition condition, out List<ColumnNode> dataColumns)
        {            
            //遍历模板树，构造一棵符合条件的树，用于生成表
            TraversalTree(rootNode, IsTrue, condition);
            //根据树得到头部列表
            List<List<ColumnNode>> headRows = new List<List<ColumnNode>>();
            dataColumns = new List<ColumnNode>();
            ConvertTreeToList(rootNode, headRows, dataColumns);
            StringBuilder htmlBuilder = new StringBuilder();
            htmlBuilder.Append("<thead>");
            for (int i = 1; i < headRows.Count; i++)
            {
                htmlBuilder.Append("<tr style=\"text-align:center;\">");
                for (int j = 0; j < headRows[i].Count; j++)
                {
                    //如果没有子节点，行数=总行数-当前的层级
                    int rowSpan = headRows[i][j].Children.Count == 0 ? headRows.Count - headRows[i][j].Level : 1;
                    htmlBuilder.AppendFormat("<th colspan=\"{0}\" rowspan=\"{1}\">{2}</th>", headRows[i][j].ColSpan, rowSpan, headRows[i][j].Title);
                }
                htmlBuilder.Append("</tr>");
            }
            htmlBuilder.Append("</thead>");
            return htmlBuilder.ToString();
        }

        /// <summary>
        /// 遍历条件类
        /// </summary>
        public class NodeCondition
        {
        }

        /// <summary>
        /// 条件是否为true
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public delegate bool IsTrueHandler(NodeCondition condition, XmlNode node);

        /// <summary>
        /// 遍历树
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="IsTrue"></param>
        /// <param name="condition"></param>
        public static void TraversalTree(ColumnNode rootNode, IsTrueHandler IsTrue, NodeCondition condition)
        {
            rootNode.Children = new List<object>();
            foreach (XmlNode node in rootNode.CorrespondXmlNode.ChildNodes)
            {
                if (IsTrue(condition, node))
                {
                    rootNode.Children.Add(
                        new ColumnNode
                        {
                            CorrespondXmlNode = node,
                            Title = node.Attributes["title"].Value,
                            Name = node.Attributes["name"].Value,
                            Level = rootNode.Level + 1
                        });
                }
            }
            foreach (object node in rootNode.Children)
            {
                TraversalTree((ColumnNode)node, IsTrue, condition);
            }
            //如果只有一个子节点，则去除该节点，去除节点的子节点上移
            if (rootNode.Children.Count == 1)
            {
                rootNode.Children = ((ColumnNode)rootNode.Children[0]).Children;
                foreach (object node in rootNode.Children)
                {
                    ((ColumnNode)node).Level = rootNode.Level + 1;
                    TraversalTree((ColumnNode)node, IsTrue, condition);
                }
            }
            foreach (object node in rootNode.Children)
            {
                rootNode.ColSpan += ((ColumnNode)node).ColSpan;
            }
            rootNode.ColSpan = rootNode.ColSpan == 0 ? 1 : rootNode.ColSpan;
        }

        /// <summary>
        /// 将树转换为列表
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="headRows"></param>
        /// <param name="dataColumns"></param>
        public static void ConvertTreeToList(ColumnNode rootNode, List<List<ColumnNode>> headRows, List<ColumnNode> dataColumns)
        {
            while (headRows.Count < rootNode.Level + 1)
                headRows.Add(new List<ColumnNode>());
            headRows[rootNode.Level].Add(rootNode);
            //叶节点作为数据绑定列
            if (rootNode.Children.Count == 0)
            {
                dataColumns.Add(rootNode);
            }
            //遍历子节点
            foreach (ColumnNode childNode in rootNode.Children)
            {
                ConvertTreeToList(childNode, headRows, dataColumns);
            }            
        }

        #endregion

        #region XML属性值逻辑判断

        /// <summary>
        /// 根据XML属性值与传入的BOOL值比较是否相等
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="boolValue"></param>
        /// <returns></returns>
        public static bool CompareBool(XmlAttribute attr, bool boolValue)
        {
            if (attr != null && !string.IsNullOrEmpty(attr.Value)
                && bool.Parse(attr.Value) != boolValue)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据XML属性值，判断其是否包含传入的整型值
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static bool IncludeInt(XmlAttribute attr, int intValue)
        {
            if (attr != null)
            {
                string[] values = attr.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length > 0)
                {
                    if (values.Length == 1 && (values[0].StartsWith(">") || values[0].StartsWith("<")))
                    {
                        int value = int.Parse(values[0].Substring(1, values[0].Length - 1));
                        if (values[0].StartsWith(">") && intValue <= value) return false;
                        if (values[0].StartsWith("<") && intValue >= value) return false;
                    }
                    else if (!values.Contains(intValue.ToString()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 根据XML属性值，判断其是否不包含传入的整型值
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="intValue"></param>
        /// <returns></returns>
        public static bool ExcludeInt(XmlAttribute attr, int intValue)
        {
            if (attr != null)
            {
                string[] values = attr.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (values.Length > 0)
                {
                    if (values.Length == 1 && (values[0].StartsWith(">") || values[0].StartsWith("<")))
                    {
                        int value = int.Parse(values[0].Substring(1, values[0].Length - 1));
                        if (values[0].StartsWith(">") && intValue > value) return false;
                        if (values[0].StartsWith("<") && intValue < value) return false;
                    }
                    else if (values.Contains(intValue.ToString()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 根据XML属性值，判断其是否包含传入的字符串
        /// </summary>
        /// <param name="attr"></param>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static bool IncludeString(XmlAttribute attr, string stringValue)
        {
            if (attr != null
                && !string.IsNullOrEmpty(attr.Value)
                && !stringValue.Contains(attr.Value))
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}