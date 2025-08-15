using BM.BLL.Common;
using BM.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BM.BLL.Manage
{
    /// <summary>
    /// 项目基础信息类
    /// </summary>
    public class ProjectInfoBLL
    {
        #region 角色信息 SA_Role
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetRoleTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT * FROM dbo.SA_Role";
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
        /// 根据GUID获取角色名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetRoleNameByGUID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT RoleName FROM dbo.SA_Role WHERE RoleGUID='" + guid + "';";
                strRes = CommonDAL.ReadString(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRes;
        }

        /// <summary>
        /// 根据GUID获取角色类型
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetRoleTypeByGUID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT RoleType FROM dbo.SA_Role WHERE RoleGUID='" + guid + "';";
                strRes = CommonDAL.ReadString(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRes;
        }

        /// <summary>
        /// 保存角色信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pUserMapInfo"></param>
        /// <returns></returns>
        public static string SaveRoleInfoData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            SqlTrxHelper Trx = new SqlTrxHelper();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                if (string.IsNullOrEmpty(dictData["RoleGUID"].ToString()))
                {
                    string myWhere = "RoleName='" + dictData["RoleName"] + "'";
                    DataTable dt = GetRoleTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//角色名称重复
                    }

                    string newGUID = Guid.NewGuid().ToString();
                    //写入痕迹日志
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('role','" + newGUID + "',@RoleName,'','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加角色信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_Role (RoleGUID,RoleName,RoleType,Notes,Create_Time) VALUES ('" + newGUID + "',@RoleName,'other',@Notes,GETDATE());");
                }
                else
                {
                    string myWhere = "RoleName='" + dictData["RoleName"].ToString().Trim().Replace("'", "''") + "' AND RoleGUID<>'" + dictData["RoleGUID"] + "'";
                    DataTable dt = GetRoleTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//角色名称重复
                    }

                    //记录日志
                    myWhere = "RoleGUID='" + dictData["RoleGUID"] + "'";
                    DataTable dtOld = GetRoleTable(myWhere, "");
                    foreach (var item in dictData)
                    {
                        string object_name = item.Key;
                        if (object_name == "RoleGUID")
                            continue;
                        string old_value = dtOld.Rows[0][item.Key].ToString().Trim().Replace("'", "''");
                        string new_value = item.Value.ToString().Trim().Replace("'", "''");
                        if (new_value != old_value)
                        {
                            //写入痕迹日志
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('role',@RoleGUID,@RoleName,'" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改角色信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.SA_Role SET RoleName=@RoleName,Notes=@Notes WHERE RoleGUID=@RoleGUID;");
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
        /// 删除角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DelRoleInfo(string id, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string roleName = GetRoleNameByGUID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('role','" + id + "','" + roleName + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除角色信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.SA_Role WHERE RoleGUID='{0}';", id);
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 批量删除角色信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string DeleteRoleInfo(string idList, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string[] idListArray = idList.Split(',');
                foreach (string id in idListArray)
                {
                    string result = DelRoleInfo(id, pUserMapInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }
        #endregion

        #region 账号信息 SA_User
        /// <summary>
        /// 获取账号信息
        /// </summary>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetUserTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT * FROM dbo.SA_User_v";
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
        /// 获取账号信息-分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="fieldName"></param>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetUserTable_Paging(int pageIndex, int pageSize, string fieldName, string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sqlStr = CommonBLL.GetPagingSQL(pageIndex, pageSize, "SA_User_v", fieldName, "UserID", myWhere, myOrderBy);
                dt = CommonDAL.GetdtList(sqlStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// 根据GUID获取用户名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetUserNameByGUID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT UserName FROM dbo.SA_User WHERE UserGUID='" + guid + "';";
                strRes = CommonDAL.ReadString(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRes;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public static string UpdateUserPwd(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user',@UserGUID,'" + pUserMapInfo.UserName + "','UserPassWord','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update password','" + pUserMapInfo.LoginIP + "','修改密码');");

                //修改密码
                sqlStr.AppendFormat("UPDATE dbo.SA_User SET UserPassWord=@UserPassWordMD5 WHERE UserGUID=@UserGUID;");

                CommonDAL.ExecuteNonQuery(sqlStr.ToString(), CommonBLL.DictionaryToParameters(dictData));
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 注册账号信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <returns></returns>
        public static string RegisterUserInfoData(Dictionary<string, object> dictData)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            SqlTrxHelper Trx = new SqlTrxHelper();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string myWhere = "UserName='" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "'";
                DataTable dt = GetUserTable(myWhere, "");
                if (dt.Rows.Count > 0)
                {
                    return "-1";//用户名重复
                }

                string newGUID = Guid.NewGuid().ToString();
                //添加数据
                sqlStr.AppendFormat("INSERT INTO dbo.SA_User (UserGUID,UserName,UserPassWord,UserStatus,UserSex,UserRoleID,Phone,Email,Create_Time) VALUES ('" + newGUID + "',@UserName,@UserPassWordMD5,@UserStatus,@UserSex,@UserRoleID,@Phone,@Email,GETDATE());");

                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) SELECT 'user',UserGUID,UserName,'','','',UserID,UserName,UserRoleID,(SELECT RoleName FROM dbo.SA_Role WHERE RoleID=@UserRoleID),'" + mydate + "','register',@Operation_IP,'注册账号' FROM dbo.SA_User WHERE UserGUID='{0}';", newGUID);

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
        /// 保存账号信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pUserMapInfo"></param>
        /// <returns></returns>
        public static string SaveUserInfoData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            SqlTrxHelper Trx = new SqlTrxHelper();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                if (string.IsNullOrEmpty(dictData["UserGUID"].ToString()))
                {
                    string myWhere = "UserName='" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "'";
                    DataTable dt = GetUserTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//用户名重复
                    }

                    string newGUID = Guid.NewGuid().ToString();
                    //写入痕迹日志
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + newGUID + "',@UserName,'','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加账号信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_User (UserGUID,UserName,UserPassWord,UserStatus,UserSex,UserRoleID,Phone,Email,Create_Time) VALUES ('" + newGUID + "',@UserName,'E10ADC3949BA59ABBE56E057F20F883E',@UserStatus,@UserSex,@UserRoleID,@Phone,@Email,GETDATE());");
                }
                else
                {
                    string myWhere = "UserName='" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "' AND UserGUID<>'" + dictData["UserGUID"] + "'";
                    DataTable dt = GetUserTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//用户名重复
                    }

                    //记录日志
                    myWhere = "UserGUID='" + dictData["UserGUID"] + "'";
                    DataTable dtOld = GetUserTable(myWhere, "");
                    foreach (var item in dictData)
                    {
                        string object_name = item.Key;
                        if (object_name == "UserGUID")
                            continue;
                        string old_value = dtOld.Rows[0][item.Key].ToString().Trim().Replace("'", "''");
                        string new_value = item.Value.ToString().Trim().Replace("'", "''");
                        if (new_value != old_value)
                        {
                            //写入痕迹日志
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user',@UserGUID,@UserName,'" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改账号信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.SA_User SET UserName=@UserName,UserStatus=@UserStatus,UserSex=@UserSex,UserRoleID=@UserRoleID,Phone=@Phone,Email=@Email WHERE UserGUID=@UserGUID;");
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
        /// 重置密码
        /// </summary>
        /// <param name="userGUID"></param>
        /// <returns></returns>
        public static string ResetUserPassword(string userGUID, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string userName = GetUserNameByGUID(userGUID);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + userGUID + "','" + userName + "','UserPassWord','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','reset password','" + pUserMapInfo.LoginIP + "','重置密码');");

                //修改数据(E10ADC3949BA59ABBE56E057F20F883E  123456)
                sqlStr.AppendFormat("UPDATE dbo.SA_User SET UserPassWord=N'E10ADC3949BA59ABBE56E057F20F883E' WHERE UserGUID='" + userGUID + "';");

                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 删除账号信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DelUserInfo(string id, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string userName = GetUserNameByGUID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + id + "','" + userName + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除账号信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.SA_User WHERE UserGUID='{0}';", id);
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 批量删除账号信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string DeleteUserInfo(string idList, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string[] idListArray = idList.Split(',');
                foreach (string id in idListArray)
                {
                    string result = DelUserInfo(id, pUserMapInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }
        #endregion

        #region 菜单权限
        /// <summary>
        /// 根据角色查询父级菜单
        /// </summary>
        /// <param name="roleGUID"></param>
        /// <returns></returns>
        public static DataTable GetParentModule(string roleGUID)
        {
            string sql = "";
            DataTable dt = new DataTable();
            try
            {
                string roleType = GetRoleTypeByGUID(roleGUID);
                if (roleType == "sa")//超级管理员
                {
                    sql = @"SELECT * FROM dbo.SA_Module
			             WHERE IsDisplay='Y' AND ParentID=0
			             ORDER BY OrderNo";
                }
                else//其他角色
                {
                    sql = string.Format(@"SELECT * FROM dbo.SA_Module
			             WHERE ModuleID in
			             (
				             SELECT DISTINCT ParentID as ModuleID
						     FROM dbo.SA_RolePermission AS a, dbo.SA_Module AS b
						     WHERE a.RoleGUID='{0}' AND b.ModuleID=a.ModuleID AND b.IsDisplay='Y' AND b.ParentID <>0
			             )
			             ORDER BY OrderNo;", roleGUID);
                }

                dt = CommonDAL.GetdtList(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// 根据角色查询子级菜单
        /// </summary>
        /// <param name="roleGUID"></param>
        /// <returns></returns>
        public static DataTable GetChildModule(string roleGUID)
        {
            string sql = "";
            DataTable dt = new DataTable();
            try
            {
                string roleType = GetRoleTypeByGUID(roleGUID);
                if (roleType == "sa")//超级管理员
                {
                    sql = @"SELECT * FROM dbo.SA_Module
			            WHERE IsDisplay='Y' AND ParentID<>0
			            ORDER BY ParentID,OrderNo";
                }
                else//其他角色
                {
                    sql = string.Format(@"SELECT b.* FROM dbo.SA_RolePermission AS a, dbo.SA_Module AS b
			            WHERE a.RoleGUID='{0}' AND b.ModuleID=a.ModuleID AND b.IsDisplay='Y' AND b.ParentID <>0
			            ORDER BY b.ParentID, b.OrderNo;", roleGUID);
                }

                dt = CommonDAL.GetdtList(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// 获取菜单信息
        /// </summary>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetModuleTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                //获得账号信息
                string sql = @"SELECT * FROM dbo.SA_Module";
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
        /// 获取角色菜单权限信息
        /// </summary>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetRolePermissionTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                //获得账号信息
                string sql = @"SELECT * FROM dbo.SA_RolePermission";
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
        /// 保存角色菜单权限信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pUserMapInfo"></param>
        /// <returns></returns>
        public static string SaveRolePermissionData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string roleName = GetRoleNameByGUID(dictData["RoleGUID"].ToString());
                //角色权限菜单ID
                string idList = dictData["idList"].ToString();

                //获取角色原来菜单权限
                //SQL Server 2017+ 优先使用 STRING_AGG
                //string strSql = "SELECT ISNULL(STRING_AGG(ModuleID, ','),'') AS ConcatenatedString FROM dbo.SA_RolePermission WHERE RoleGUID='" + dictData["RoleGUID"] + "';";

                string strSql = @"SELECT ISNULL(STUFF(
                                        (
                                            SELECT ',' + CAST(ModuleID AS VARCHAR(10)) 
                                            FROM dbo.SA_RolePermission
                                            WHERE RoleGUID='" + dictData["RoleGUID"] + @"'
                                            ORDER BY ModuleID
                                            FOR XML PATH(''), TYPE
                                        ).value('.', 'NVARCHAR(MAX)'), 
                                        1, 1, ''
                                    ),'') AS ConcatenatedOrders;";
                string old_idList = CommonDAL.ReadString(strSql).ToString();
                if (old_idList == idList)
                {
                    return "-1";//角色菜单权限没有发生改变，不需要保存！
                }

                //写入痕迹日志
                string mydate = CommonDAL.ReadCurrDate();
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('role','" + dictData["RoleGUID"] + "','" + roleName + "','rolepermission','','" + idList + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','rolepermission','" + pUserMapInfo.LoginIP + "','设置角色菜单权限');");

                //先删除
                sqlStr.AppendFormat(@"DELETE dbo.SA_RolePermission WHERE RoleGUID='{0}';", dictData["RoleGUID"]);

                //添加数据
                string[] idListArray = idList.Split(',');
                foreach (var moduleid in idListArray)
                {
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_RolePermission (RoleGUID,ModuleID) VALUES ('" + dictData["RoleGUID"] + "','" + moduleid + "');");
                }

                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }
        #endregion

        #region 部门信息 SA_Department
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="myWhere"></param>
        /// <param name="myOrderBy"></param>
        /// <returns></returns>
        public static DataTable GetDepartmentTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT * FROM dbo.SA_Department";
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
        /// 根据GUID获取部门名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetDepartmentNameByGUID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT DepartmentName FROM dbo.SA_Department WHERE DepartmentGUID='" + guid + "';";
                strRes = CommonDAL.ReadString(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRes;
        }

        /// <summary>
        /// 保存部门信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pUserMapInfo"></param>
        /// <returns></returns>
        public static string SaveDepartmentInfoData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            SqlTrxHelper Trx = new SqlTrxHelper();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                if (string.IsNullOrEmpty(dictData["DepartmentGUID"].ToString()))
                {
                    string myWhere = "DepartmentName='" + dictData["DepartmentName"].ToString().Trim().Replace("'", "''") + "'";
                    DataTable dt = GetDepartmentTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//部门名称重复
                    }

                    string newGUID = Guid.NewGuid().ToString();
                    //写入痕迹日志
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('department','" + newGUID + "',@DepartmentName,'','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加部门信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_Department (DepartmentGUID,DepartmentName,Notes,Create_Time) VALUES ('" + newGUID + "',@DepartmentName,@DepartmentName,GETDATE());");
                }
                else
                {
                    string myWhere = "DepartmentName='" + dictData["DepartmentName"].ToString().Trim().Replace("'", "''") + "' AND DepartmentGUID<>'" + dictData["DepartmentGUID"] + "'";
                    DataTable dt = GetDepartmentTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//部门名称重复
                    }

                    //记录日志
                    myWhere = "DepartmentGUID='" + dictData["DepartmentGUID"] + "'";
                    DataTable dtOld = GetDepartmentTable(myWhere, "");
                    foreach (var item in dictData)
                    {
                        string object_name = item.Key;
                        if (object_name == "DepartmentGUID")
                            continue;
                        string old_value = dtOld.Rows[0][item.Key].ToString().Trim().Replace("'", "''");
                        string new_value = item.Value.ToString().Trim().Replace("'", "''");
                        if (new_value != old_value)
                        {
                            //写入痕迹日志
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('department',@DepartmentGUID,@DepartmentName,'" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改部门信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.SA_Department SET DepartmentName=@DepartmentName,Notes=@Notes WHERE DepartmentGUID=@DepartmentGUID;");
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
        /// 删除部门信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DelDepartmentInfo(string id, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string departmentName = GetDepartmentNameByGUID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('department','" + id + "','" + departmentName + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除部门信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.SA_Department WHERE DepartmentGUID='{0}';", id);
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 批量删除部门信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string DeleteDepartmentInfo(string idList, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string[] idListArray = idList.Split(',');
                foreach (string id in idListArray)
                {
                    string result = DelDepartmentInfo(id, pUserMapInfo);
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
