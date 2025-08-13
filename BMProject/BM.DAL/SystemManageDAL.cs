using BM.BLL.Common;
using BM.DAL.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BM.DAL
{
    /// <summary>
    /// 系统设置类
    /// </summary>
    public class SystemManageDAL
    {
        #region 参数配置信息 SA_Parameter
        /// <summary>
        /// 获取参数配置信息
        /// </summary>
        /// <param name="myWhere"></param>
        /// <returns></returns>
        public static DataTable GetParameterTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT * FROM dbo.SA_Parameter";
                if (myWhere != "")
                    sql += " WHERE " + myWhere;
                if (myOrderBy != "")
                    sql += " ORDER BY " + myOrderBy;

                dt = DBHelper.GetDataTable(sql);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 根据键名获取参数配置值
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        public static string GetParameterValueByName(string parameterName)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT COALESCE(MAX(ParameterValue), '') AS Option_Value FROM dbo.SA_Parameter WHERE ParameterName='" + parameterName + "';";
                strRes = DBHelper.GetSingle(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 保存参数配置
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pUserMapInfo"></param>
        /// <returns></returns>
        public static string SaveConfigInfoData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = DBHelper.ReadCurrDate();
                if (dictData["PID"].ToString() == "")
                {
                    string myWhere = "ParameterName='" + dictData["ParameterName"].ToString().Trim().Replace("'", "''") + "'";
                    DataTable dt = GetParameterTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//键名重复
                    }

                    //写入痕迹日志
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('parameter','" + dictData["ParameterName"].ToString().Trim().Replace("'", "''") + "','" + dictData["ParameterName"].ToString().Trim().Replace("'", "''") + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加参数配置信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_Parameter (ParameterName,ParameterValue,IsDisplay,Notes,Create_Time) VALUES (N'" + dictData["ParameterName"].ToString().Trim().Replace("'", "''") + "',N'" + dictData["ParameterValue"].ToString().Trim().Replace("'", "''") + "','Y',N'" + dictData["Notes"].ToString().Trim().Replace("'", "''") + "',GETDATE());");
                }
                else
                {
                    //记录日志
                    string myWhere = "PID='" + dictData["PID"].ToString() + "'";
                    DataTable dtOld = GetParameterTable(myWhere, "");
                    foreach (var item in dictData)
                    {
                        string object_name = item.Key;
                        string old_value = dtOld.Rows[0][item.Key].ToString().Trim().Replace("'", "''");
                        string new_value = item.Value.ToString().Trim().Replace("'", "''");
                        if (new_value != old_value)
                        {
                            //写入痕迹日志
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('parameter','" + dictData["ParameterName"].ToString().Trim().Replace("'", "''") + "','" + dictData["ParameterName"].ToString().Trim().Replace("'", "''") + "','" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改参数配置信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.SA_Parameter SET ParameterValue=N'" + dictData["ParameterValue"].ToString().Trim().Replace("'", "''") + "',Notes=N'" + dictData["Notes"].ToString().Trim().Replace("'", "''") + "' WHERE " + myWhere + ";");
                }
                int result = DBHelper.ExecuteSql(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 删除参数配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DelConfigInfo(string id, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = DBHelper.ReadCurrDate();
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('parameter','" + id + "','" + id + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除参数配置信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.SA_Parameter WHERE ParameterName='{0}';", id);
                int result = DBHelper.ExecuteSql(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 批量删除参数配置信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string DeleteConfigInfo(string idList, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = DBHelper.ReadCurrDate();
                string[] idListArray = idList.Split(',');
                foreach (string id in idListArray)
                {
                    string result = DelConfigInfo(id, pUserMapInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }
        #endregion
    }
}
