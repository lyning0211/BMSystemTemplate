using BM.BLL.Common.Common;
using BM.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace BM.BLL.Manage
{
    /// <summary>
    /// 公共业务类
    /// </summary>
    public class CommonBLL
    {
        /// <summary>
        /// 获取数据Table
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">要显示的字段</param>
        /// <param name="myWhere">条件</param>
        /// <param name="myOrderBy">排序方式</param>
        /// <returns></returns>
        public static DataTable GetDataTable(string tableName, string fieldName, string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                //获得用户信息
                string sql = string.Format(@"SELECT {0} FROM {1}", fieldName, tableName);
                if (myWhere != "")
                    sql += " WHERE " + myWhere;
                if (myOrderBy != "")
                    sql += " ORDER BY " + myOrderBy;

                dt = CommonDAL.GetdtList(sql);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 获取总记录数
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="myWhere">条件</param>
        /// <returns></returns>
        public static int GetRecordCount(string tableName, string myWhere)
        {
            int recordCount = 0;
            try
            {
                string sqlStr = "SELECT COUNT(1) FROM " + tableName;
                if (!string.IsNullOrEmpty(myWhere))
                    sqlStr += " WHERE " + myWhere;
                recordCount = CommonDAL.ReadString(sqlStr).ToInt();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return recordCount;
        }

        /// <summary>
        ///得到分页SQL语句
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">总页数</param>
        /// <param name="tableName">表名</param>
        /// <param name="fieldName">要显示的字段</param>
        /// <param name="fieldID">用于排序的字段(唯一)</param>
        /// <param name="myWhere">条件</param>
        /// <param name="myOrderBy">排序方式</param>
        /// <returns></returns>
        public static string GetPagingSQL(int pageIndex, int pageSize, string tableName, string fieldName, string fieldID, string myWhere, string myOrderBy)
        {
            string strSQL = "";
            int liPriorNumber = 0;

            if (!string.IsNullOrEmpty(myOrderBy.Trim()))
                myOrderBy = " ORDER BY " + myOrderBy + " ";

            if (pageIndex > 0)
            {
                liPriorNumber = pageIndex * pageSize;
                if (string.IsNullOrEmpty(myWhere))
                {
                    strSQL += string.Format("SELECT TOP {0} {1} FROM {2} WHERE {3} NOT IN(", pageSize.ToString(), fieldName, tableName, fieldID);
                    strSQL += string.Format("SELECT TOP {0} {1} FROM {2} {3} )", liPriorNumber.ToString(), fieldID, tableName, myOrderBy);
                }
                else
                {
                    strSQL += string.Format("SELECT TOP {0} {1} FROM {2} WHERE {3} AND {4} NOT IN(", pageSize.ToString(), fieldName, tableName, myWhere, fieldID);
                    strSQL += string.Format("SELECT TOP {0} {1} FROM {2} WHERE {3} {4})", liPriorNumber.ToString(), fieldID, tableName, myWhere, myOrderBy);
                }
            }
            else
            {
                if (string.IsNullOrEmpty(myWhere))
                {
                    strSQL += string.Format("SELECT TOP {0} {1} FROM {2}", pageSize.ToString(), fieldName, tableName);
                }
                else
                {
                    strSQL += string.Format("SELECT TOP {0} {1} FROM {2} WHERE {3}", pageSize.ToString(), fieldName, tableName, myWhere);
                }
            }
            if (!string.IsNullOrEmpty(myOrderBy))
                strSQL += myOrderBy;
            return strSQL;
        }

        /// <summary>
        /// 将字典转换成sql参数
        /// </summary>
        public static SqlParameter[] DictionaryToParameters(Dictionary<string, object> dic)
        {
            if (dic == null) return null;
            return dic.Select(kvp => new SqlParameter("@" + kvp.Key.ToLower(), kvp.Value)).ToArray();
        }

        public static IList<SqlParameter> DictionaryToParametersList(Dictionary<string, object> dic)
        {
            if (dic == null) return null;
            return dic.Select(kvp => new SqlParameter("@" + kvp.Key.ToLower(), kvp.Value)).ToList();
        }
    }
}
