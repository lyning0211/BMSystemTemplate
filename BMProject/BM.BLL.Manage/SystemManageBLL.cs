using BM.BLL.Common;
using BM.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace BM.BLL.Manage
{
    /// <summary>
    /// 系统设置类
    /// </summary>
    public class SystemManageBLL
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

                dt = CommonDAL.GetdtList(sql);

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
                strRes = CommonDAL.ReadString(sqlStr).ToString();
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
        public static string SaveParameterInfoData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            SqlTrxHelper Trx = new SqlTrxHelper();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                if (dictData["PID"].ToString() == "")
                {
                    string myWhere = "ParameterName='" + dictData["ParameterName"].ToString().Trim().Replace("'", "''") + "'";
                    DataTable dt = GetParameterTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//键名重复
                    }

                    //写入痕迹日志
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('parameter',@ParameterName,@ParameterName,'','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加参数配置信息');");

                    //添加数据
                    sqlStr.AppendFormat("INSERT INTO dbo.SA_Parameter (ParameterName,ParameterValue,IsDisplay,Notes,Create_Time) VALUES (@ParameterName,@ParameterValue,'Y',@Notes,GETDATE());");
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
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('parameter',@ParameterName,@ParameterName,'" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改参数配置信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.SA_Parameter SET ParameterValue=@ParameterValue,Notes=@Notes WHERE " + myWhere + ";");
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
        /// 删除参数配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DelParameterInfo(string id, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('parameter','" + id + "','" + id + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除参数配置信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.SA_Parameter WHERE ParameterName='{0}';", id);
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
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
        public static string DeleteParameterInfo(string idList, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string[] idListArray = idList.Split(',');
                foreach (string id in idListArray)
                {
                    string result = DelParameterInfo(id, pUserMapInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }
        #endregion

        #region 货币信息设置 Sys_Currency
        /// <summary>
        /// 获取货币信息设置
        /// </summary>
        /// <param name="myWhere"></param>
        /// <returns></returns>
        public static DataTable GetCurrencyTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT * FROM dbo.Sys_Currency";
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
        /// 根据GUID获取货币名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetCurrencyNameByGUID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT CurrencyName FROM dbo.Sys_Currency WHERE CurrencyGUID='" + guid + "';";
                strRes = CommonDAL.ReadString(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 保存货币信息
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pUserMapInfo"></param>
        /// <returns></returns>
        public static string SaveCurrencyInfoData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            SqlTrxHelper Trx = new SqlTrxHelper();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                if (dictData["CurrencyGUID"].ToString() == "")
                {
                    string myWhere = "CurrencyName='" + dictData["CurrencyName"].ToString().Trim().Replace("'", "''") + "'";
                    DataTable dt = GetCurrencyTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//键名重复
                    }

                    string newGUID = Guid.NewGuid().ToString();
                    //写入痕迹日志
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('currency','" + newGUID + "',@CurrencyName,'','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加货币信息');");

                    //添加数据
                    sqlStr.AppendFormat(@"INSERT INTO dbo.Sys_Currency (CurrencyGUID,CurrencyName,CurrencySymbol,Country,RMB_ExchangeRate,Is_GreaterThanRMB,Notes,Create_Time) VALUES ('" + newGUID + "',@CurrencyName,@CurrencySymbol,@Country,@RMB_ExchangeRate,@Is_GreaterThanRMB,@Notes,GETDATE());");
                }
                else
                {
                    string myWhere = "CurrencyName='" + dictData["CurrencyName"].ToString().Trim().Replace("'", "''") + "' AND CurrencyGUID<>'" + dictData["CurrencyGUID"] + "'";
                    DataTable dt = GetCurrencyTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//货币名称重复
                    }

                    //记录日志
                    myWhere = "CurrencyGUID='" + dictData["CurrencyGUID"] + "'";
                    DataTable dtOld = GetCurrencyTable(myWhere, "");
                    foreach (var item in dictData)
                    {
                        string object_name = item.Key;
                        if (object_name == "CurrencyGUID")
                            continue;
                        string old_value = dtOld.Rows[0][item.Key].ToString().Trim().Replace("'", "''");
                        string new_value = item.Value.ToString().Trim().Replace("'", "''");
                        if (new_value != old_value)
                        {
                            //写入痕迹日志
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('currency',@CurrencyGUID,@CurrencyName,'" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改货币信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.Sys_Currency SET CurrencyName=@CurrencyName,CurrencySymbol=@CurrencySymbol,Country=@Country,RMB_ExchangeRate=@RMB_ExchangeRate,Is_GreaterThanRMB=@Is_GreaterThanRMB,Notes=@Notes WHERE CurrencyGUID=@CurrencyGUID;");
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
        /// 删除货币信息设置
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static string DelCurrencyInfo(string id, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string currencyName = GetCurrencyNameByGUID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('currency','" + id + "','" + currencyName + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除货币信息');");

                //执行删除语句
                sqlStr.AppendFormat(@"DELETE dbo.Sys_Currency WHERE CurrencyGUID='{0}';", id);
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 批量删除货币信息设置
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public static string DeleteCurrencyInfo(string idList, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string[] idListArray = idList.Split(',');
                foreach (string id in idListArray)
                {
                    string result = DelCurrencyInfo(id, pUserMapInfo);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }
        #endregion

        #region 货物类型设置 Sys_GoodsType
        /// <summary>
        /// 获取货物类型设置
        /// </summary>
        /// <param name="myWhere"></param>
        /// <returns></returns>
        public static DataTable GetGoodsTypeTable(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT * FROM dbo.Sys_GoodsType";
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
        /// 获取完整三级联动查询（一次性获取所有层级关系）
        /// </summary>
        /// <param name="myWhere"></param>
        /// <returns></returns>
        public static DataTable GetGoodsTypeLevel3Table(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT 
                                    L1.TypeID AS Level1ID, L1.TypeName AS Level1Name,
                                    L2.TypeID AS Level2ID, L2.TypeName AS Level2Name,
                                    L3.TypeID AS Level3ID, L3.TypeName AS Level3Name
                                FROM Sys_GoodsType L1
                                LEFT JOIN Sys_GoodsType L2 ON L2.ParentID = L1.TypeID AND L2.Level = 2
                                LEFT JOIN Sys_GoodsType L3 ON L3.ParentID = L2.TypeID AND L3.Level = 3
                                WHERE L1.Level = 1";
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
        /// 获取二级联动查询
        /// </summary>
        /// <param name="myWhere"></param>
        /// <returns></returns>
        public static DataTable GetGoodsTypeLevel2Table(string myWhere, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = @"SELECT 
                                    L1.TypeID AS Level1ID, L1.TypeName AS Level1Name,
                                    L2.TypeID AS Level2ID, L2.TypeName AS Level2Name
                                FROM Sys_GoodsType L1
                                LEFT JOIN Sys_GoodsType L2 ON L2.ParentID = L1.TypeID AND L2.Level = 2
                                WHERE L1.Level = 1";
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
        /// 根据三级ID反向查询完整路径
        /// </summary>
        /// <param name="Level3ID">三级分类ID</param>
        /// <returns></returns>
        public static DataTable GetGoodsTypeByLevel3Table(int Level3ID, string myOrderBy)
        {
            DataTable dt = new DataTable();
            try
            {
                string sql = string.Format(@"WITH TypeCTE AS (
                                    SELECT TypeID, ParentID, TypeName, Level 
                                    FROM Sys_GoodsType WHERE TypeID={0}
                                    UNION ALL
                                    SELECT P.TypeID, P.ParentID, P.TypeName, P.Level
                                    FROM Sys_GoodsType P
                                    INNER JOIN TypeCTE C ON C.ParentID = P.TypeID
                                )
                                SELECT * FROM TypeCTE ORDER BY Level;", Level3ID);
                dt = CommonDAL.GetdtList(sql);

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 根据GUID获取货物类型名称
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string GetTypeNameByGUID(string guid)
        {
            string strRes = "";
            try
            {
                string sqlStr = @"SELECT TypeName FROM dbo.Sys_GoodsType WHERE TypeGUID='" + guid + "';";
                strRes = CommonDAL.ReadString(sqlStr).ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 保存货物类型
        /// </summary>
        /// <param name="dictData"></param>
        /// <param name="pUserMapInfo"></param>
        /// <returns></returns>
        public static string SaveGoodsTypeInfoData(Dictionary<string, object> dictData, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            SqlTrxHelper Trx = new SqlTrxHelper();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                if (dictData["TypeGUID"].ToString() == "")
                {
                    string myWhere = "TypeName='" + dictData["TypeName"].ToString().Trim().Replace("'", "''") + "'";
                    DataTable dt = GetGoodsTypeTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//货物类型名称重复
                    }

                    string newGUID = Guid.NewGuid().ToString();
                    //写入痕迹日志
                    sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('goodstype','" + newGUID + "',@TypeName,'','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','add','" + pUserMapInfo.LoginIP + "','添加货物类型信息');");

                    //添加数据
                    sqlStr.AppendFormat(@"INSERT INTO dbo.Sys_GoodsType (TypeGUID,TypeName,ParentID,Level,Notes,Create_Time) VALUES ('" + newGUID + "',@TypeName,@ParentID,@Level,@Notes,GETDATE());");
                }
                else
                {
                    string myWhere = "TypeName='" + dictData["TypeName"].ToString().Trim().Replace("'", "''") + "' AND TypeGUID<>'" + dictData["TypeGUID"] + "'";
                    DataTable dt = GetGoodsTypeTable(myWhere, "");
                    if (dt.Rows.Count > 0)
                    {
                        return "-1";//货物类型名称重复
                    }

                    //记录日志
                    myWhere = "TypeGUID='" + dictData["TypeGUID"] + "'";
                    DataTable dtOld = GetGoodsTypeTable(myWhere, "");
                    foreach (var item in dictData)
                    {
                        string object_name = item.Key;
                        if (object_name == "TypeGUID")
                            continue;
                        string old_value = dtOld.Rows[0][item.Key].ToString().Trim().Replace("'", "''");
                        string new_value = item.Value.ToString().Trim().Replace("'", "''");
                        if (new_value != old_value)
                        {
                            //写入痕迹日志
                            sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('goodstype',@TypeGUID,@TypeName,'" + object_name + "','" + old_value + "','" + new_value + "','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','update','" + pUserMapInfo.LoginIP + "','修改货物类型信息');");
                        }
                    }
                    //修改数据
                    sqlStr.AppendFormat("UPDATE dbo.Sys_GoodsType SET TypeName=@TypeName,ParentID=@ParentID,Level=@Level,Notes=@Notes WHERE TypeGUID=@TypeGUID;");
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
        /// 删除货物类型设置
        /// </summary>
        /// <param name="id"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string DelGoodsTypeInfo(string id, string level, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string typeName = GetTypeNameByGUID(id);
                //写入痕迹日志
                sqlStr.AppendFormat("INSERT INTO dbo.Log_Trace (Object_Type,Object_GUID,Object_Name,Object_Field,Old_Value,New_Value,UserID,UserName,UserRoleID,UserRoleName,Operation_Time,Operation_Type,Operation_IP,Operation_Note) VALUES('goodstype','" + id + "','" + typeName + "','','','','" + pUserMapInfo.UserID + "','" + pUserMapInfo.UserName + "','" + pUserMapInfo.UserRoleID + "','" + pUserMapInfo.UserRoleName + "','" + mydate + "','delete','" + pUserMapInfo.LoginIP + "','删除货物类型');");

                //执行删除语句
                if (level == "1")//删除一级分类
                {
                    //1.删除所有三级分类
                    sqlStr.AppendFormat(@"DELETE dbo.Sys_GoodsType WHERE WHERE ParentID IN(SELECT TypeID FROM Sys_GoodsType WHERE ParentID=(SELECT TypeID FROM Sys_GoodsType WHERE TypeGUID='{0}'));", id);

                    //2.删除所有二级分类
                    sqlStr.AppendFormat(@"DELETE dbo.Sys_GoodsType WHERE ParentID=(SELECT TypeID FROM Sys_GoodsType WHERE TypeGUID='{0}');", id);
                }
                else if (level == "2")//删除二级分类
                {
                    //1.删除所有三级分类
                    sqlStr.AppendFormat(@"DELETE dbo.Sys_GoodsType WHERE ParentID=(SELECT TypeID FROM Sys_GoodsType WHERE TypeGUID='{0}');", id);
                }
                sqlStr.AppendFormat(@"DELETE dbo.Sys_GoodsType WHERE TypeGUID='{0}';", id);
                CommonDAL.ExecuteNonQuery(sqlStr.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return strRes;
        }

        /// <summary>
        /// 批量删除货物类型设置
        /// </summary>
        /// <param name="idList"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public static string DeleteGoodsTypeInfo(string idList, string level, LoginUserModel pUserMapInfo)
        {
            string strRes = "";
            StringBuilder sqlStr = new StringBuilder();
            try
            {
                string mydate = CommonDAL.ReadCurrDate();
                string[] idListArray = idList.Split(',');
                foreach (string id in idListArray)
                {
                    string result = DelGoodsTypeInfo(id, level, pUserMapInfo);
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
