using BM.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BM.BLL.Manage
{
    /// <summary>
    /// 日志信息类
    /// </summary>
    public class LogInfoBLL
    {
        /// <summary>
        /// 获取用户登录日志信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fieldName"></param>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetLogLoginTable(int pageIndex, int pageSize, string fieldName, string myWhere, string myOrderBy)
        {
            string sqlStr = "";
            DataTable dt = new DataTable();
            try
            {
                if (pageIndex == 0 && pageSize == 0)
                {
                    //查询全部数据
                    sqlStr = @"SELECT * FROM dbo.Log_Login_v";
                    if (myWhere != "")
                        sqlStr += " WHERE " + myWhere;
                    if (myOrderBy != "")
                        sqlStr += " ORDER BY " + myOrderBy;
                }
                else
                {
                    //分页查询
                    sqlStr = CommonBLL.GetPagingSQL(pageIndex, pageSize, "Log_Login_v", fieldName, "LogID", myWhere, myOrderBy);
                }
                dt = CommonDAL.GetdtList(sqlStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// 获取操作日志信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fieldName"></param>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetLogOperationTable(int pageIndex, int pageSize, string fieldName, string myWhere, string myOrderBy)
        {
            string sqlStr = "";
            DataTable dt = new DataTable();
            try
            {
                if (pageIndex == 0 && pageSize == 0)
                {
                    //查询全部数据
                    sqlStr = @"SELECT * FROM dbo.Log_Operation_v";
                    if (myWhere != "")
                        sqlStr += " WHERE " + myWhere;
                    if (myOrderBy != "")
                        sqlStr += " ORDER BY " + myOrderBy;
                }
                else
                {
                    //分页查询
                    sqlStr = CommonBLL.GetPagingSQL(pageIndex, pageSize, "Log_Operation_v", fieldName, "LogID", myWhere, myOrderBy);
                }
                dt = CommonDAL.GetdtList(sqlStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// 获取数据修改痕迹日志信息
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fieldName"></param>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetLogTraceTable(int pageIndex, int pageSize, string fieldName, string myWhere, string myOrderBy)
        {
            string sqlStr = "";
            DataTable dt = new DataTable();
            try
            {
                if (pageIndex == 0 && pageSize == 0)
                {
                    //查询全部数据
                    sqlStr = @"SELECT * FROM dbo.Log_Trace_v";
                    if (myWhere != "")
                        sqlStr += " WHERE " + myWhere;
                    if (myOrderBy != "")
                        sqlStr += " ORDER BY " + myOrderBy;
                }
                else
                {
                    //分页查询
                    sqlStr = CommonBLL.GetPagingSQL(pageIndex, pageSize, "Log_Trace_v", fieldName, "LogID", myWhere, myOrderBy);
                }
                dt = CommonDAL.GetdtList(sqlStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// 保存用户登录日志
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public static string SaveLog_Login(Dictionary<string, string> dictData)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Login (UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_IP,Operation_Note) VALUES (@UserID,@UserName,@UserRoleID,@UserRoleName,'" + mydate + "',@Operation_IP,@Operation_Note);");
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 保存数据操作日志
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public static string SaveLog_Operation(Dictionary<string, string> dictData)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Operation (Object_Name,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES (@Object_Name,@UserID,@UserName,@UserRoleID,@UserRoleName,'" + mydate + "',@Operation_Type,@Operation_IP,@Operation_Note);");
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 保存数据修改痕迹日志
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public static string SaveLog_Trace(Dictionary<string, string> dictData)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES (@Object_Type,@Object_GUID,@Object_Name,@Object_Field,@Old_Value,@New_Value,@UserID,@UserName,@UserRoleID,@UserRoleName,'" + mydate + "',@Operation_Type,@Operation_IP,@Operation_Note);");
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }



        /// <summary>
        /// 插入数据操作日志SQL语句
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public static string InserLog_OperationSQL(Dictionary<string, string> dictData)
        {
            return @"INSERT INTO dbo.Log_Operation(
                                Object_Name,
                                UserID,
                                UserName,
                                UserRoleID,
                                UserRoleName,
                                Operation_Time,
                                Operation_Type,
                                Operation_IP,
                                Operation_Note)
                    VALUES ('" + dictData["Object_Name"].ToString().Trim() + "','" + dictData["UserID"].ToString().Trim() + "','" + dictData["UserName"].ToString().Trim() + "','" + dictData["UserRoleID"].ToString().Trim() + "','" + dictData["UserRoleName"].ToString().Trim() + "','" + dictData["mydate"].ToString().Trim() + "','" + dictData["Operation_Type"].ToString().Trim() + "','" + dictData["Operation_IP"].ToString().Trim() + "',N'" + dictData["Operation_Note"].ToString().Trim() + "');";
        }

        /// <summary>
        /// 插入数据修改痕迹日志SQL语句
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public static string InserLog_TraceSQL(Dictionary<string, string> dictData)
        {
            return @"INSERT INTO dbo.Log_Trace(
                                Object_Type,
                                Object_GUID,
                                Object_Name,
                                Object_Field,
                                Old_Value,
                                New_Value,
                                UserID,
                                UserName,
                                UserRoleID,
                                UserRoleName,
                                Operation_Time,
                                Operation_Type,
                                Operation_IP,
                                Operation_Note) 
                     VALUES ('" + dictData["Object_Type"].ToString().Trim() + "','" + dictData["Object_GUID"].ToString().Trim() + "','" + dictData["Object_Name"].ToString().Trim() + "','" + dictData["Object_Field"].ToString().Trim() + "',N'" + dictData["Old_Value"].ToString().Trim() + "',N'" + dictData["New_Value"].ToString().Trim() + "','" + dictData["UserID"].ToString().Trim() + "','" + dictData["UserName"].ToString().Trim() + "','" + dictData["UserRoleID"].ToString().Trim() + "','" + dictData["UserRoleName"].ToString().Trim() + "','" + dictData["mydate"].ToString().Trim() + "','" + dictData["Operation_Type"].ToString().Trim() + "','" + dictData["Operation_IP"].ToString().Trim() + "',N'" + dictData["Operation_Note"].ToString().Trim() + "');";
        }
    }
}
