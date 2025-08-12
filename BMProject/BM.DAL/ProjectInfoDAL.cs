using BM.BLL.Common;
using BM.DAL.Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BM.DAL
{
    public class ProjectInfoDAL
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

                dt = DBHelper.GetDataTable(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 根据ID获取角色名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetRoleNameByID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT RoleName FROM dbo.SA_Role WHERE RoleGUID='" + guid + "';";
                strRes = DBHelper.GetSingle(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return strRes;
        }

        /// <summary>
        /// 根据ID获取角色类型
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetRoleTypeByID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT RoleType FROM dbo.SA_Role WHERE RoleGUID='" + guid + "';";
                strRes = DBHelper.GetSingle(sqlStr).ToString();
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
            try
            {
                string mydate = DBHelper.ReadCurrDate();
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
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('role','" + newGUID + "',N'" + dictData["RoleName"].ToString().Trim().Replace("'", "''") + "','','','','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加角色信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_Role (RoleGUID,RoleName,RoleType,RoleNote,Create_Time) VALUES ('" + newGUID + "',N'" + dictData["RoleName"].ToString().Trim().Replace("'", "''") + "','other',N'" + dictData["RoleNote"].ToString().Trim().Replace("'", "''") + "',GETDATE());");
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
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('role','" + dictData["RoleGUID"] + "',N'" + dictData["RoleName"].ToString().Trim().Replace("'", "''") + "','" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改角色信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.SA_Role SET RoleName=N'" + dictData["RoleName"].ToString().Trim().Replace("'", "''") + "',RoleNote=N'" + dictData["RoleNote"].ToString().Trim().Replace("'", "''") + "' WHERE RoleGUID='" + dictData["RoleGUID"] + "';");
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
                string mydate = DBHelper.ReadCurrDate();
                string roleName = GetRoleNameByID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('role','" + id + "','" + roleName + "','','','','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除角色信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.SA_Role WHERE RoleGUID='{0}';", id);
                int result = DBHelper.ExecuteSql(sqlStr.ToString());
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
                string mydate = DBHelper.ReadCurrDate();
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

                dt = DBHelper.GetDataTable(sql);
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
                string sqlStr = CommonDAL.GetPagingSQL(pageIndex, pageSize, "SA_User_v", fieldName, "UserGUID", myWhere, myOrderBy);
                dt = DBHelper.GetDataTable(sqlStr);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return dt;
        }

        /// <summary>
        /// 根据ID获取用户名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetUserNameByID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT UserName FROM dbo.SA_User WHERE UserGUID='" + guid + "';";
                strRes = DBHelper.GetSingle(sqlStr).ToString();
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
                string mydate = DBHelper.ReadCurrDate();
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + dictData["UserGUID"] + "',N'" + pUserMapInfo.UserName + "','UserPassWord','','','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update password','" + pUserMapInfo.LoginIP + "','修改密码');");

                //修改密码
                sqlStr.AppendFormat("UPDATE dbo.SA_User SET UserPassWord=N'" + dictData["UserPassWordMD5"].ToString().Trim().Replace("'", "''") + "' WHERE UserGUID='" + dictData["UserGUID"] + "';");

                int result = DBHelper.ExecuteSql(sqlStr.ToString());
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
        public static string SaveUserInfoData(Dictionary<string, object> dictData)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = DBHelper.ReadCurrDate();
                string myWhere = "UserName='" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "'";
                DataTable dt = GetUserTable(myWhere, "");
                if (dt.Rows.Count > 0)
                {
                    return "-1";//用户名重复
                }

                string newGUID = Guid.NewGuid().ToString();
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + newGUID + "',N'" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "','','','','/','/','/','/','" + mydate + "','register','" + dictData["Operation_IP"] + "','注册账号');");

                //添加数据
                sqlStr.AppendFormat("INSERT INTO dbo.SA_User (UserGUID,UserName,UserPassWord,UserStatus,UserSex,UserRoleGUID,Phone,Email,Create_Time) VALUES ('" + newGUID + "',N'" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "','" + dictData["UserPassWordMD5"] + "','" + dictData["UserStatus"] + "','" + dictData["UserSex"] + "','" + dictData["UserRoleGUID"] + "','" + dictData["Phone"] + "','" + dictData["Email"].ToString().Trim().Replace("'", "''") + "',GETDATE());");
                int result = DBHelper.ExecuteSql(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
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
            try
            {
                string mydate = DBHelper.ReadCurrDate();
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
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + newGUID + "',N'" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "','','','','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加账号信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_User (UserGUID,UserName,UserPassWord,UserStatus,UserSex,UserRoleGUID,Phone,Email,Create_Time) VALUES ('" + newGUID + "',N'" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "','E10ADC3949BA59ABBE56E057F20F883E','" + dictData["UserStatus"] + "','" + dictData["UserSex"] + "','" + dictData["UserRoleGUID"] + "','" + dictData["Phone"] + "','" + dictData["Email"].ToString().Trim().Replace("'", "''") + "',GETDATE());");
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
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + dictData["UserGUID"] + "',N'" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "','" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改账号信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.SA_User SET UserName=N'" + dictData["UserName"].ToString().Trim().Replace("'", "''") + "',UserStatus='" + dictData["UserStatus"] + "',UserSex='" + dictData["UserSex"] + "',UserRoleGUID='" + dictData["UserRoleGUID"] + "',Phone='" + dictData["Phone"] + "',Email='" + dictData["Email"].ToString().Trim().Replace("'", "''") + "' WHERE UserGUID='" + dictData["UserGUID"] + "';");
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
                string mydate = DBHelper.ReadCurrDate();
                string userName = GetUserNameByID(userGUID);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + userGUID + "','" + userName + "','UserPassWord','','','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','reset password','" + pUserMapInfo.LoginIP + "','重置密码');");

                //修改数据(E10ADC3949BA59ABBE56E057F20F883E  123456)
                sqlStr.AppendFormat("UPDATE dbo.SA_User SET UserPassWord=N'E10ADC3949BA59ABBE56E057F20F883E' WHERE UserGUID='" + userGUID + "';");

                int result = DBHelper.ExecuteSql(sqlStr.ToString());
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
                string mydate = DBHelper.ReadCurrDate();
                string userName = GetUserNameByID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('user','" + id + "','" + userName + "','','','','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除账号信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.SA_User WHERE UserGUID='{0}';", id);
                int result = DBHelper.ExecuteSql(sqlStr.ToString());
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
                string mydate = DBHelper.ReadCurrDate();
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
                string roleType = GetRoleTypeByID(roleGUID);
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

                dt = DBHelper.GetDataTable(sql);
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
                string roleType = GetRoleTypeByID(roleGUID);
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

                dt = DBHelper.GetDataTable(sql);
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

                dt = DBHelper.GetDataTable(sql);
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

                dt = DBHelper.GetDataTable(sql);
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
                string roleName = GetRoleNameByID(dictData["RoleGUID"].ToString());
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
                string old_idList = DBHelper.GetSingle(strSql).ToString();
                if (old_idList == idList)
                {
                    return "-1";//角色菜单权限没有发生改变，不需要保存！
                }

                //写入痕迹日志
                string mydate = DBHelper.ReadCurrDate();
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('role','" + dictData["RoleGUID"] + "','" + roleName + "','rolepermission','','" + idList + "','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','rolepermission','" + pUserMapInfo.LoginIP + "','设置角色菜单权限');");

                //先删除
                sqlStr.AppendFormat(@"DELETE dbo.SA_RolePermission WHERE RoleGUID='{0}';", dictData["RoleGUID"]);

                //添加数据
                string[] idListArray = idList.Split(',');
                foreach (var moduleid in idListArray)
                {
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_RolePermission (RoleGUID,ModuleID) VALUES ('" + dictData["RoleGUID"] + "','" + moduleid + "');");
                }

                int result = DBHelper.ExecuteSql(sqlStr.ToString());
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

                dt = DBHelper.GetDataTable(sql);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 根据ID获取部门名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetDepartmentNameByID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT DepartmentName FROM dbo.SA_Department WHERE DepartmentGUID='" + guid + "';";
                strRes = DBHelper.GetSingle(sqlStr).ToString();
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
            try
            {
                string mydate = DBHelper.ReadCurrDate();
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
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('department','" + newGUID + "',N'" + dictData["DepartmentName"].ToString().Trim().Replace("'", "''") + "','','','','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加部门信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_Department (DepartmentGUID,DepartmentName,DepartmentNote,Create_Time) VALUES ('" + newGUID + "',N'" + dictData["DepartmentName"].ToString().Trim().Replace("'", "''") + "',N'" + dictData["DepartmentNote"].ToString().Trim().Replace("'", "''") + "',GETDATE());");
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
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('department','" + dictData["DepartmentGUID"] + "',N'" + dictData["DepartmentName"].ToString().Trim().Replace("'", "''") + "','" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改部门信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.SA_Department SET DepartmentName=N'" + dictData["DepartmentName"].ToString().Trim().Replace("'", "''") + "',DepartmentNote=N'" + dictData["DepartmentNote"].ToString().Trim().Replace("'", "''") + "' WHERE DepartmentGUID='" + dictData["DepartmentGUID"] + "';");
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
                string mydate = DBHelper.ReadCurrDate();
                string departmentName = GetDepartmentNameByID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserGUID,UserName,UserRoleGUID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('department','" + id + "','" + departmentName + "','','','','" + pUserMapInfo.UserGUID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleGUID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除部门信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.SA_Department WHERE DepartmentGUID='{0}';", id);
                int result = DBHelper.ExecuteSql(sqlStr.ToString());
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
                string mydate = DBHelper.ReadCurrDate();
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
