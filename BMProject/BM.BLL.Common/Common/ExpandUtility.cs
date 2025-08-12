using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace BM.BLL.Common.Common
{
    public static class ExpandUtility
    {
        #region DataTable扩展

        public static IList<T> FillEntityFromReader<T>(SqlDataReader reader)
        {
            List<T> list = new List<T>();
            List<PropertyInfo> pinfoList = new List<PropertyInfo>();
            Boolean checkedPropertyInfo = false;
            while (reader.Read())
            {
                T instance = Activator.CreateInstance<T>();

                #region 检查数据库字段与实体类属性的匹配情况
                if (!checkedPropertyInfo)
                {
                    foreach (PropertyInfo pinfo in typeof(T).GetProperties())
                    {
                        //判断reader是否有此Property中的BoundFieldAttribute所对应需要绑定的字段
                        if (reader.GetSchemaTable().Select("ColumnName='" + pinfo.Name + "'").Length > 0)
                        {
                            pinfoList.Add(pinfo);
                        }
                    }
                }
                #endregion

                //查检完成
                checkedPropertyInfo = true;

                //开始赋值
                foreach (PropertyInfo info in pinfoList)
                {
                    int index = reader.GetOrdinal(info.Name);
                    if (reader.GetValue(index) != DBNull.Value)
                    {
                        if (!info.PropertyType.IsEnum)
                        {
                            info.SetValue(instance, Convert.ChangeType(reader.GetValue(index), info.PropertyType), null);
                        }
                        else
                        {
                            info.SetValue(instance, Enum.Parse(info.PropertyType, reader.GetValue(index).ToString()), null);
                        }
                    }
                }

                list.Add(instance);
            }
            reader.Close();
            return list;
        }

        /// <summary>
        /// 此方法适应与数据表结构字母大小写和实体类属性名称相同的情况下
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> FillEntityFromDatatable<T>(this DataTable dt) where T : new()
        {
            var list = new List<T>();
            if (dt == null) return list;
            var len = dt.Rows.Count;

            for (var i = 0; i < len; i++)
            {
                var info = new T();
                foreach (DataColumn dc in dt.Rows[i].Table.Columns)
                {
                    var value = dt.Rows[i][dc.ColumnName];
                    if (value == null || string.IsNullOrEmpty(value.ToString())) continue;



                    var p = info.GetType().GetProperty(dc.ColumnName);


                    try
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            p.SetValue(info, value.ToString(), null);
                        }
                        else if (p.PropertyType == typeof(int))
                        {
                            p.SetValue(info, int.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(bool))
                        {
                            p.SetValue(info, bool.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(DateTime))
                        {
                            p.SetValue(info, DateTime.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(float))
                        {
                            p.SetValue(info, float.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(double))
                        {
                            p.SetValue(info, double.Parse(value.ToString()), null);
                        }
                        else
                        {
                            p.SetValue(info, value.ToString(), null);
                        }
                    }
                    catch (Exception)
                    {
                        //p.SetValue(info, ex.Message, null);
                    }
                }
                list.Add(info);
            }
            dt.Dispose(); dt = null;
            return list;
        }

        /// <summary>
        /// 此方法适应与数据表结构字母大小写和实体类属性名称相同的情况下
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> FillEntityFromDatatable<T>(this DataTable dt, out int count) where T : new()
        {

            var list = new List<T>();
            count = 0;
            if (dt == null) return list;
            var len = dt.Rows.Count;

            for (var i = 0; i < len; i++)
            {
                var info = new T();
                foreach (DataColumn dc in dt.Rows[i].Table.Columns)
                {
                    var value = dt.Rows[i][dc.ColumnName];
                    if (value == null || string.IsNullOrEmpty(value.ToString())) continue;



                    var p = info.GetType().GetProperty(dc.ColumnName);




                    try
                    {

                        if (dc.ColumnName.ToLower() == "totalcount")
                        {
                            count = Int32.Parse(value.ToString());
                        }

                        if (p.PropertyType == typeof(string))
                        {
                            p.SetValue(info, value.ToString(), null);
                        }
                        else if (p.PropertyType == typeof(int))
                        {
                            p.SetValue(info, int.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(bool))
                        {
                            p.SetValue(info, bool.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(DateTime))
                        {
                            p.SetValue(info, DateTime.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(float))
                        {
                            p.SetValue(info, float.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(double))
                        {
                            p.SetValue(info, double.Parse(value.ToString()), null);
                        }
                        else
                        {
                            p.SetValue(info, value.ToString(), null);
                        }
                    }
                    catch (Exception)
                    {
                        //p.SetValue(info, ex.Message, null);
                    }
                }
                list.Add(info);
            }
            dt.Dispose(); dt = null;
            return list;
        }


        /// <summary>
        /// table转实体对象扩展方法，不区分数据表列名称大小写
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ToList<T>(this DataTable dt) where T : new()
        {
            var list = new List<T>();
            if (dt == null) return list;
            var len = dt.Rows.Count;

            for (var i = 0; i < len; i++)
            {
                var info = new T();

                for (int j = 0; j < dt.Rows[i].Table.Columns.Count; j++)
                {
                    DataColumn dc = dt.Rows[i].Table.Columns[j];

                    //数据表列名称
                    var CName = dc.ColumnName.ToLower();
                    var value = dt.Rows[i][CName];
                    if (value == null || string.IsNullOrEmpty(value.ToString())) continue;
                    var p = info.GetType().GetProperty(CName);
                    var ps = info.GetType().GetProperties();

                    for (int k = 0; k < ps.Length; k++)
                    {
                        //实体类属性名称
                        var Mname = ps[k].Name.ToLower();
                        if (CName == Mname)
                        {
                            p = ps[k];
                            break;
                        }
                    }
                    try
                    {
                        if (p.PropertyType == typeof(string))
                        {
                            p.SetValue(info, value.ToString(), null);
                        }
                        else if (p.PropertyType == typeof(int))
                        {
                            p.SetValue(info, int.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(bool))
                        {
                            p.SetValue(info, bool.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(DateTime))
                        {
                            p.SetValue(info, DateTime.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(float))
                        {
                            p.SetValue(info, float.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(double))
                        {
                            p.SetValue(info, double.Parse(value.ToString()), null);
                        }
                        else if (p.PropertyType == typeof(decimal))
                        {
                            p.SetValue(info, decimal.Parse(value.ToString()), null);
                        }
                        else
                        {
                            p.SetValue(info, value.ToString(), null);
                        }
                    }
                    catch (Exception)
                    {
                        //p.SetValue(info, ex.Message, null);
                    }


                }
                list.Add(info);
            }
            dt.Dispose(); dt = null;
            return list;
        }

        /// <summary>
        /// 按照属性顺序的列名集合
        /// </summary>
        public static IList<string> GetColumnNames(this DataTable dt)
        {
            DataColumnCollection dcc = dt.Columns;
            //由于集合中的元素是确定的，所以可以指定元素的个数，系统就不会分配多余的空间，效率会高点
            IList<string> list = new List<string>(dcc.Count);
            foreach (DataColumn dc in dcc)
            {
                list.Add(dc.ColumnName);
            }
            return list;
        }

        /// <summary>
        /// 根据列名称获得数据表中第一行中的指定列值
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colName">列名称</param>
        /// <returns></returns>
        public static object RowVal(this DataTable dt, string colName)
        {
            try
            {
                return dt.Rows[0][colName];
            }
            catch
            {
                return string.Empty;

            }
        }
        /// <summary>
        /// 根据列索引称获得数据表中第一行中的指定列值
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="colName">列索引</param>
        /// <returns></returns>
        public static object RowVal(this DataTable dt, int colIndex)
        {
            try
            {
                return dt.Rows[0][colIndex];
            }
            catch
            {
                return string.Empty;

            }
        }

        /// <summary>
        /// 获得数据表中指定行和指定列的值
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="colName">列名称</param>
        /// <returns></returns>
        public static object RowVal(this DataTable dt, int rowIndex, string colName)
        {
            try
            {
                return dt.Rows[rowIndex][colName];
            }
            catch
            {
                return string.Empty;

            }
        }

        /// <summary>
        /// 获得数据表中指定行和指定列的值
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="rowIndex">行索引</param>
        /// <param name="colIndex">列索引</param>
        /// <returns></returns>
        public static object RowVal(this DataTable dt, int rowIndex, int colIndex)
        {
            try
            {
                return dt.Rows[rowIndex][colIndex];
            }
            catch
            {
                return string.Empty;

            }
        }

        #endregion

        #region String扩展

        /// <summary>
        /// 扩展方法String转换int
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Int32 ToInt(this string s)
        {
            Int32 id;
            Int32.TryParse(s, out id);//这里当转换失败时返回的id为0
            return id;
        }

        /// <summary>
        /// 扩展方法String转换Double
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Double ToDouble(this string s)
        {
            Double id;
            Double.TryParse(s, out id);//这里当转换失败时返回的id为0
            return id;
        }

        /// <summary>
        /// 扩展方法String转换Decimal
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Decimal ToDecimal(this string s)
        {
            Decimal id;
            Decimal.TryParse(s, out id);//这里当转换失败时返回的id为0
            return id;
        }

        /// <summary>
        /// 扩展方法String转换DateTime
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this string s)
        {
            DateTime dt;
            DateTime.TryParse(s, out dt);
            return dt;
        }

        /// <summary>
        /// 扩展方法String转换Boolean
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Boolean ToBool(this string s)
        {
            Boolean bl;
            Boolean.TryParse(s, out bl);
            return bl;
        }

        /// <summary>
        /// 判断字符串是否为空
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        /// <summary>
        /// 判断是否为正整数
        /// </summary>
        /// <param name="inString"></param>
        /// <returns></returns>
        public static bool IsInt(this string inString)
        {
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[0-9]*[1-9][0-9]*$");
            return regex.IsMatch(inString.Trim());
        }


        /// <summary>
        /// 如果字符串是文件路径或者url可以使用此扩展方法
        /// </summary>
        /// <param name="s"></param>
        public static string Open(this string s)
        {
            System.Diagnostics.Process.Start(s);
            return s;
        }

        /// <summary>
        /// 截取字符串，超出部分显示...
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxlength"></param>
        /// <returns></returns>
        public static string CutStr(this string str, int maxlength)
        {
            if (str.Length > maxlength)
                return str.Substring(0, maxlength) + "...";
            else return str;
        }

        ///   <summary>
        ///   给一个字符串进行MD5加密
        ///   </summary>
        ///   <param   name="input">待加密字符串</param>
        ///   <returns>加密后的字符串(大写)</returns>
        public static string ToMD5(this string input)
        {
            using (MD5 md5 = MD5.Create())
            {
                StringBuilder sb = new StringBuilder();
                foreach (byte b in md5.ComputeHash(Encoding.UTF8.GetBytes(input)))
                {
                    //toString()方法后可以传参，传入X2表示转换为数字和英文
                    //如果传入d1就表示一位数，d2表示两位数
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string ToDateTimeStr(this string s)
        {
            return s.ToDateTime().ToString("yyyy-MM-dd");//兼容win7 xp 
        }

        #endregion

        #region DataRow扩展

        /// <summary>
        /// 扩展方法String转换int
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Int32 ToInt(this object obj)
        {
            Int32 id;
            Int32.TryParse(obj.ToString(), out id);
            return id;
        }

        /// <summary>
        /// 扩展方法String转换Double
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Double ToDouble(this object obj)
        {
            Double id;
            Double.TryParse(obj.ToString(), out id);//这里当转换失败时返回的id为0
            return id;
        }

        /// <summary>
        /// 扩展方法String转换Decimal
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Decimal ToDecimal(this object obj)
        {
            Decimal id;
            Decimal.TryParse(obj.ToString(), out id);//这里当转换失败时返回的id为0
            return id;
        }

        /// <summary>
        /// 扩展方法String转换DateTime
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object obj)
        {
            DateTime dt;
            DateTime.TryParse(obj.ToString(), out dt);
            return dt;
        }

        /// <summary>
        /// 扩展方法String转换Boolean
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static Boolean ToBool(this object obj)
        {
            Boolean bl;
            Boolean.TryParse(obj.ToString(), out bl);
            return bl;
        }

        /// <summary>
        /// 扩展方法将将真假值转换成文本字符“是否”
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToBoolText(this object s)
        {
            if (s == null) { return String.Empty; }

            Boolean mark;
            Boolean.TryParse(s.ToString(), out mark);
            if (mark)
            {
                return "是";
            }
            else
            {
                return "否";
            }

        }

        /// <summary>
        /// 性别转换
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToSexText(this object obj)
        {
            if (obj != null)
            {
                string[] sex = new string[] { "未指定", "男", "女" };
                return sex[obj.ToInt()];
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 将文件路径格式化成超链接
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToFileLocText(this object obj)
        {
            if (obj == null) { return String.Empty; }
            return String.Format("<a target='_blank' href='{0}'>下载</a>", obj);
        }

        /// <summary>
        /// 替换null为DBNull
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static object ToDBNull(this object obj)
        {
            if (obj == null)
            {
                return DBNull.Value;
            }
            else
            {
                return obj;
            }

        }
        #endregion

        #region Html扩展
        /// <summary>
        /// 扩展方法String转换Html
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string ToHtml(this string str)
        {
            str = str.Replace("&", "&amp;");
            str = str.Replace("<", "&lt;");
            str = str.Replace(">", "&gt;");
            str = str.Replace("\"", "&quot;");
            str = str.Replace("'", "&apos;");
            str = str.Replace(" ", "&nbsp;");
            str = str.Replace("\n", "<br/>");
            return str;
        }
        #endregion

        #region Xml扩展
        /// <summary>
        /// 扩展方法Object转换String(转义Xml字符)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToXml(this object obj)
        {
            return obj.ToString().Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
        }
        #endregion
    }
}
