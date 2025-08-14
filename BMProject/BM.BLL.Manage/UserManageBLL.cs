using BM.BLL.Common;
using BM.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BM.BLL.Manage
{
    /// <summary>
    /// 用户信息类
    /// </summary>
    public class UserManageBLL
    {
        #region 客户信息 User_Client
        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetClientTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT * FROM dbo.User_Client";
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
        /// 根据GUID获取客户名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetClientNameByGUID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT ClientName FROM dbo.User_Client WHERE ClientGUID='" + guid + "';";
                strRes = CommonDAL.ReadString(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRes;
        }

        /// <summary>
        /// 保存客户信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pUserMapInfo"></param>
        /// <returns></returns>
        public static string SaveClientInfoData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            SqlTrxHelper Trx = new SqlTrxHelper();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                if (string.IsNullOrEmpty(dictData["ClientGUID"].ToString()))
                {
                    string myWhere = "ClientName='" + dictData["ClientName"] + "'";
                    DataTable dt = GetClientTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//客户名称重复
                    }

                    string newGUID = Guid.NewGuid().ToString();
                    //写入痕迹日志
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('client','" + newGUID + "',@ClientName,'','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加客户信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.User_Client (ClientGUID,ClientName,Phone,Email,City,Address,Historical_Amount,Notes,Create_Time) VALUES ('" + newGUID + "',@ClientName,@Phone,@Email,@City,@Address,'" + dictData["Historical_Amount"].ToString().Trim() + "',@Notes,GETDATE());");
                }
                else
                {
                    string myWhere = "ClientName='" + dictData["ClientName"].ToString().Trim().Replace("'", "''") + "' AND ClientGUID<>'" + dictData["ClientGUID"] + "'";
                    DataTable dt = GetClientTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//客户名称重复
                    }

                    //记录日志
                    myWhere = "ClientGUID='" + dictData["ClientGUID"] + "'";
                    DataTable dtOld = GetClientTable(myWhere, "");
                    foreach (var item in dictData)
                    {
                        string object_name = item.Key;
                        if (object_name == "ClientGUID")
                            continue;
                        string old_value = dtOld.Rows[0][item.Key].ToString().Trim().Replace("'", "''");
                        string new_value = item.Value.ToString().Trim().Replace("'", "''");
                        if (new_value != old_value)
                        {
                            //写入痕迹日志
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('client',@ClientGUID,@ClientName,'" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改客户信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.User_Client SET ClientName=@ClientName,Phone=N@Phone,Email=@Email,City=@City,Address=@Address,Historical_Amount='" + dictData["Historical_Amount"].ToString().Trim() + "',Notes=@Notes WHERE ClientGUID=@ClientGUID;");
                }

                CommonDAL.ExecuteNonQuery(sqlStr.ToString(), Trx, CommonBLL.DictionaryToParameters(dictData));
                Trx.SqlTrx.Commit();
            }
            catch (Exception ex)
            {
                Trx.SqlTrx.Rollback();
                throw ex;
            }
            finally
            {
                Trx.DisposeSqlTrx();
            }
            return strRes;
        }

        /// <summary>
        /// 删除客户信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DelClientInfo(string id, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string clientName = GetClientNameByGUID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('client','" + id + "','" + clientName + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除客户信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.User_Client WHERE ClientGUID='{0}';", id);
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 批量删除客户信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string DeleteClientInfo(string idList, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string[] idListArray = idList.Split(',');
                foreach (string id in idListArray)
                {
                    string result = DelClientInfo(id, pUserMapInfo);
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
