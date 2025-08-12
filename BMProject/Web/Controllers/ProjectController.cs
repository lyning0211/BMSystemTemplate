using BM.BLL.Common;
using BM.BLL.Common.Common;
using BM.DAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Mvc;
using Web.Helper;

namespace Web.Controllers
{
    //项目基础信息
    public class ProjectController : HelperController
    {
        //角色信息页面
        [FilterConfig]
        public ActionResult RoleIndex()
        {
            return View();
        }

        //角色信息页面（添加/修改）
        [FilterConfig]
        public ActionResult RoleInfo()
        {
            return View();
        }

        /// <summary>
        /// 角色权限分配页面
        /// </summary>
        /// <returns></returns>
        [FilterConfig]
        public ActionResult RolePermission()
        {
            string roleGUID = cPublic.GetQueryString("RoleGUID");
            string roleName = "";
            if (!string.IsNullOrEmpty(roleGUID))
            {
                //根据ID查询角色信息
                string myWhere = "RoleGUID='" + roleGUID + "'";
                DataTable dt = ProjectInfoDAL.GetRoleTable(myWhere, "");
                if (dt != null && dt.Rows.Count > 0)
                {
                    roleName = dt.Rows[0]["RoleName"].ToString();
                }
                ViewBag.RoleGUID = roleGUID;
                ViewBag.RoleName = roleName;
            }
            else
            {
                RedirectToAction("/Project/RoleIndex");
            }

            return View();
        }

        //账号信息页面
        [FilterConfig]
        public ActionResult UserIndex()
        {
            return View();
        }

        //账号信息页面（添加/修改）
        [FilterConfig]
        public ActionResult UserInfo()
        {
            return View();
        }

        //部门信息页面
        [FilterConfig]
        public ActionResult DepartmentIndex()
        {
            return View();
        }

        //部门信息页面（添加/修改）
        [FilterConfig]
        public ActionResult DepartmentInfo()
        {
            return View();
        }


        /*数据*/
        /// <summary>
        /// 得到条件
        /// </summary>
        /// <param name="jsonWhere"></param>
        /// <returns></returns>
        [FilterConfig]
        private string BuildWhereData(string jsonWhere)
        {
            Dictionary<string, object> dictList = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonWhere);
            StringBuilder myWhere = new StringBuilder();
            myWhere.Append("1=1");
            if (dictList["queryvalue"].ToString() != "")
            {
                myWhere.Append(" AND " + dictList["queryname"] + " LIKE " + "'%" + dictList["queryvalue"] + "%'");
            }
            if (dictList.ContainsKey("querysex") && dictList["querysex"].ToString() != "")
            {
                myWhere.Append(" AND UserSex='" + dictList["querysex"] + "'");
            }
            if (dictList.ContainsKey("querystatus") && dictList["querystatus"].ToString() != "")
            {
                myWhere.Append(" AND UserStatus='" + dictList["querystatus"] + "'");
            }

            return myWhere.ToString();
        }

        #region 角色管理
        /// <summary>
        /// 获取角色信息列表
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetRoleTableData(string jsonData)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                sbHtml.Append("<thead><tr class=\"text-c\">");
                sbHtml.Append("<th width=\"25\"><input type=\"checkbox\" onchange=\"selAll(this);\" name=\"\" value=\"\" class=\"pointer\"></th>");//全选
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "序号");//序号
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "角色名称");//角色名称
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "备注");//备注
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "创建时间");//创建时间
                sbHtml.AppendFormat("<th width=\"120\"><span>{0}</span></th>", "操作");//操作
                sbHtml.Append("</tr></thead><tbody class=\"body\">");

                string myWhere = BuildWhereData(pDict["currentwhere"].ToString());
                string orderBy = "Create_Time DESC";
                DataTable dt = ProjectInfoDAL.GetRoleTable(myWhere, orderBy);
                //if (dt.Rows.Count == 0)
                //{
                //    sbHtml.Append("<tr><td colspan=\"15\" style=\"text-align:center\"><h5>没有数据...</h5></td></tr>");//没有数据
                //}
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbHtml.Append("<tr class=\"text-c\">");
                    if (dt.Rows[i]["RoleType"].ToString() == "sa")//超级管理员
                        sbHtml.Append("<td><input type=\"checkbox\" value=\"\" name=\"\" class=\"pointer\" disabled=\"disabled\"></td>");
                    else
                        sbHtml.AppendFormat("<td><input type=\"checkbox\" value=\"{0}\" name=\"selAllData\" class=\"pointer\"></td>", dt.Rows[i]["RoleGUID"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", i + 1);
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["RoleName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["RoleNote"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Create_Time"].ToString());
                    //操作
                    if (dt.Rows[i]["RoleType"].ToString() == "sa")//超级管理员
                    {
                        sbHtml.Append("<td>/</td>");
                    }
                    else
                    {
                        sbHtml.Append("<td>");
                        sbHtml.AppendFormat("<span><a style=\"text-decoration:none\" class=\"ml-5\" href=\"/Project/RolePermission?RoleGUID={1}\" title=\"{0}\"><i class=\"Hui-iconfont\">&#xe61d;</i></a></span>", "权限设置", dt.Rows[i]["RoleGUID"].ToString());//权限设置
                        sbHtml.AppendFormat("<span><a style=\"text-decoration:none\" class=\"ml-5\" href=\"/Project/RoleInfo?RoleGUID={1}\" title=\"{0}\"><i class=\"Hui-iconfont\">&#xe6df;</i></a></span>", "修改", dt.Rows[i]["RoleGUID"].ToString());//修改
                        sbHtml.AppendFormat("<span><a title=\"{0}\" href=\"javascript:;\" onclick=\"del_info(this,'{1}')\" class=\"ml-5\" style=\"text-decoration:none\"><i class=\"Hui-iconfont\">&#xe6e2;</i></a></span>", "删除", dt.Rows[i]["RoleGUID"].ToString());//删除
                        sbHtml.Append("</td>");
                    }
                    sbHtml.Append("</tr>");
                }
                sbHtml.Append("</tbody>");

                message.Success = true;
                message.ReturnString = sbHtml.ToString();
                message.StrReturn = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }

        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetRoleInfoData(string id)
        {
            Message message = new Message();
            try
            {
                string myWhere = "RoleGUID='" + id + "'";
                DataTable dt = ProjectInfoDAL.GetRoleTable(myWhere, "");
                if (dt.Rows.Count > 0)
                {
                    message.Success = true;
                    message.ReturnString = JsonHelper.DataTableToJson2(dt);
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "数据加载失败！";//数据加载失败！
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 保存角色数据
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult SaveRoleInfoData(string jsonData)
        {
            Message message = new Message();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //保存数据
                string result = ProjectInfoDAL.SaveRoleInfoData(pDict, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "保存成功！";
                }
                else if (result == "-1")
                {
                    message.Success = false;
                    message.ReturnString = "角色名称重复！";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "保存失败！";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 删除角色信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult DelRoleInfoData(string id)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //删除数据
                string result = ProjectInfoDAL.DelRoleInfo(id, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "删除成功";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "删除失败";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 批量删除角色信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult DeleteRoleInfoData(string idList)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //批量删除数据
                string result = ProjectInfoDAL.DeleteRoleInfo(idList, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "删除成功";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "删除失败";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 获取角色权限信息列表
        /// </summary>
        /// <param name="roleGUID"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetRolePermissionTableData(string roleGUID)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                sbHtml.Append("<thead><tr class=\"text-c\">");
                sbHtml.Append("<th width=\"25\"><input type=\"checkbox\" onchange=\"selAll(this);\" name=\"\" value=\"\" class=\"pointer\"></th>");//全选
                sbHtml.AppendFormat("<th width=\"150\"><span>{0}</span></th>", "菜单ID");//菜单ID
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "菜单名称");//菜单名称
                sbHtml.AppendFormat("<th width=\"150\"><span>{0}</span></th>", "上级菜单");//上级菜单
                sbHtml.AppendFormat("<th width=\"90\"><span>{0}</span></th>", "是否显示");//是否显示
                sbHtml.AppendFormat("<th width=\"80\"><span>{0}</span></th>", "排序");//排序
                sbHtml.Append("</tr></thead><tbody>");

                //所有一级菜单列表
                DataTable dtParentModule = ProjectInfoDAL.GetModuleTable("ParentID=0 AND IsFirstMenu='Y'", "ModuleID");
                //所有二级菜单列表
                DataTable dtChildModule = ProjectInfoDAL.GetModuleTable("ParentID>0 AND IsFirstMenu='N'", "ModuleID");
                //角色权限列表
                DataTable dtRolePermission = ProjectInfoDAL.GetRolePermissionTable("RoleGUID='" + roleGUID + "'", "");
                if (dtChildModule.Rows.Count == 0)
                {
                    sbHtml.Append("<tr><td colspan=\"15\" style=\"text-align:center\"><h5>没有数据...</h5></td></tr>");
                }
                for (int i = 0; i < dtChildModule.Rows.Count; i++)
                {
                    //上级菜单名称
                    string parentModuleName = "";
                    DataRow[] drData = dtParentModule.Select("ModuleID='" + dtChildModule.Rows[i]["ParentID"] + "'");
                    if (drData != null && drData.Length > 0)
                    {
                        parentModuleName = drData[0]["ModuleName"].ToString();
                    }

                    int flag = 0;
                    sbHtml.Append("<tr class=\"text-c\">");
                    for (int j = 0; j < dtRolePermission.Rows.Count; j++)
                    {
                        if (dtChildModule.Rows[i]["ModuleID"].ToString() == dtRolePermission.Rows[j]["ModuleID"].ToString())
                        {
                            sbHtml.AppendFormat("<td><input type=\"checkbox\" value=\"{0}\" name=\"selAllData\" class=\"pointer\" checked></td>", dtChildModule.Rows[i]["ModuleID"]);
                            flag = 1;
                        }
                    }
                    if (flag == 0)
                    {
                        sbHtml.AppendFormat("<td><input type=\"checkbox\" value=\"{0}\" name=\"selAllData\" class=\"pointer\"></td>", dtChildModule.Rows[i]["ModuleID"]);
                    }
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dtChildModule.Rows[i]["ModuleID"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dtChildModule.Rows[i]["ModuleName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", parentModuleName);
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dtChildModule.Rows[i]["IsDisplay"].ToString() == "Y" ? "是" : "否");
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dtChildModule.Rows[i]["OrderNo"].ToString());
                    sbHtml.Append("</tr>");
                }
                sbHtml.Append("</tbody>");

                message.Success = true;
                message.ReturnString = sbHtml.ToString();
                message.StrReturn = dtChildModule.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }

        /// <summary>
        /// 保存角色菜单权限
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult SaveRolePermissionData(string jsonData)
        {
            Message message = new Message();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);
                if (string.IsNullOrEmpty(pDict["RoleGUID"].ToString()))
                {
                    message.Success = false;
                    message.ReturnString = "角色信息有误！";
                }
                if (string.IsNullOrEmpty(pDict["idList"].ToString()))
                {
                    message.Success = false;
                    message.ReturnString = "菜单信息为空！";
                }
                string[] idListArray = pDict["idList"].ToString().Split(',');
                if (idListArray.Length <= 0)
                {
                    message.Success = false;
                    message.ReturnString = "菜单信息为空！";
                }

                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //保存数据
                string result = ProjectInfoDAL.SaveRolePermissionData(pDict, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "权限修改成功";
                }
                else if (result == "-1")
                {
                    message.Success = false;
                    message.ReturnString = "角色菜单权限没有发生改变，不需要保存！";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "设置权限失败";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }
        #endregion

        #region 账号管理
        /// <summary>
        /// 获取账号信息列表
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetUserTableData(string jsonData)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                sbHtml.Append("<thead><tr class=\"text-c\">");
                sbHtml.Append("<th width=\"25\"><input type=\"checkbox\" onchange=\"selAll(this);\" name=\"\" value=\"\" class=\"pointer\"></th>");//全选
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "序号");//序号
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "用户名");//用户名
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "角色");//角色
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "性别");//性别
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "联系电话");//联系电话
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "邮箱账号");//邮箱账号
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "状态");//状态
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "创建时间");//创建时间
                sbHtml.AppendFormat("<th width=\"120\"><span>{0}</span></th>", "操作");//操作
                sbHtml.Append("</tr></thead><tbody class=\"body\">");

                DataTable dtRoles = ProjectInfoDAL.GetRoleTable("", "");
                string myWhere = BuildWhereData(pDict["currentwhere"].ToString());
                string orderBy = "Create_Time DESC";
                DataTable dt = ProjectInfoDAL.GetUserTable(myWhere, orderBy);
                //if (dt.Rows.Count == 0)
                //{
                //    sbHtml.Append("<tr><td colspan=\"15\" style=\"text-align:center\"><h5>没有数据...</h5></td></tr>");//没有数据
                //}
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    //角色类型
                    string roleType = "";
                    DataRow[] drRoles = dtRoles.Select("RoleGUID ='" + dt.Rows[i]["UserRoleGUID"].ToString() + "'");
                    if (drRoles != null && drRoles.Length > 0)
                        roleType = drRoles[0]["RoleType"].ToString();

                    sbHtml.Append("<tr class=\"text-c\">");
                    sbHtml.AppendFormat("<td><input type=\"checkbox\" value=\"{0}\" name=\"selAllData\" class=\"pointer\"></td>", dt.Rows[i]["UserGUID"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", i + 1);
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["UserName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["RoleName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", cPublic.GetTraceValue("user", dt.Rows[i]["UserSex"].ToString()));
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Phone"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Email"].ToString());
                    if (dt.Rows[i]["UserStatus"].ToString() == "0")
                        sbHtml.AppendFormat("<td class=\"td-status\"><span class=\"label label-success radius\">{0}</span></td>", "已启用");//已启用
                    else if (dt.Rows[i]["UserStatus"].ToString() == "-1")
                        sbHtml.AppendFormat("<td class=\"td-status\"><span class=\"label radius\">{0}</span></td>", "已停用");//已停用
                    else
                        sbHtml.Append("<td><span></span></td>");
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Create_Time"].ToString());
                    //操作
                    if (roleType == "sa")//超级管理员
                    {
                        sbHtml.Append("<td>/</td>");
                    }
                    else
                    {
                        sbHtml.Append("<td>");
                        sbHtml.AppendFormat("<span><a style=\"text-decoration:none\" class=\"ml-5\" href=\"/Project/UserInfo?UserGUID={1}\" title=\"{0}\"><i class=\"Hui-iconfont\">&#xe6df;</i></a></span>", "修改", dt.Rows[i]["UserGUID"].ToString());//修改
                        sbHtml.AppendFormat("<span><a title=\"{0}\" href=\"javascript:;\" onclick=\"reset_password('{1}')\" class=\"ml-5\" style=\"text-decoration:none\"><i class=\"Hui-iconfont\">&#xe63f;</i></a></span>", "重置密码", dt.Rows[i]["UserGUID"].ToString());//重置密码
                        if (pUserMapInfo.UserGUID != dt.Rows[i]["UserGUID"].ToString())//非当前登录用户(自己不能删除和停用自己的信息)
                        {
                            sbHtml.AppendFormat("<span><a title=\"{0}\" href=\"javascript:;\" onclick=\"del_info('{1}')\" class=\"ml-5\" style=\"text-decoration:none\"><i class=\"Hui-iconfont\">&#xe6e2;</i></a></span>", "删除", dt.Rows[i]["UserGUID"].ToString());//删除
                        }
                        sbHtml.Append("</td>");
                    }
                    sbHtml.Append("</tr>");
                }
                sbHtml.Append("</tbody>");

                message.Success = true;
                message.ReturnString = sbHtml.ToString();
                message.StrReturn = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }

        /// <summary>
        /// 绑定角色信息
        /// </summary>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult BindRoleData()
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                DataTable dtRoles = ProjectInfoDAL.GetRoleTable("RoleType<>'sa'", "RoleName");
                sbHtml.Append("<option value=\"\"></option>");
                if (dtRoles != null && dtRoles.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtRoles.Rows)
                    {
                        sbHtml.AppendFormat("<option value=\"{0}\">{1}</option>", dr["RoleGUID"], dr["RoleName"]);
                    }
                }

                message.Success = true;
                message.ReturnString = sbHtml.ToString();
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }

        /// <summary>
        /// 获取账号信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetUserInfoData(string id)
        {
            Message message = new Message();
            try
            {
                string myWhere = "UserGUID='" + id + "'";
                DataTable dt = ProjectInfoDAL.GetUserTable(myWhere, "");
                if (dt.Rows.Count > 0)
                {
                    message.Success = true;
                    message.ReturnString = JsonHelper.DataTableToJson2(dt);
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "数据加载失败！";//数据加载失败！
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 保存用户数据
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult SaveUserInfoData(string jsonData)
        {
            Message message = new Message();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //保存数据
                string result = ProjectInfoDAL.SaveUserInfoData(pDict, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "保存成功！";
                }
                else if (result == "-1")
                {
                    message.Success = false;
                    message.ReturnString = "用户名重复！";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "保存失败！";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="userGUID"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult ResetPasswordData(string userGUID)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //重置密码
                string result = ProjectInfoDAL.ResetUserPassword(userGUID, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "密码已重置成功，初始密码为123456，请尽快修改密码！";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "重置密码失败！";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 删除账号信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult DelUserInfoData(string id)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //删除数据
                string result = ProjectInfoDAL.DelUserInfo(id, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "删除成功";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "删除失败";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 批量删除账号信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult DeleteUserInfoData(string idList)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //批量删除数据
                string result = ProjectInfoDAL.DeleteUserInfo(idList, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "删除成功";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "删除失败";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }
        #endregion

        #region 部门管理
        /// <summary>
        /// 获取部门信息列表
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetDepartmentTableData(string jsonData)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                sbHtml.Append("<thead><tr class=\"text-c\">");
                sbHtml.Append("<th width=\"25\"><input type=\"checkbox\" onchange=\"selAll(this);\" name=\"\" value=\"\" class=\"pointer\"></th>");//全选
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "序号");//序号
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "部门名称");//部门名称
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "备注");//备注
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "创建时间");//创建时间
                sbHtml.AppendFormat("<th width=\"120\"><span>{0}</span></th>", "操作");//操作
                sbHtml.Append("</tr></thead><tbody class=\"body\">");

                string myWhere = BuildWhereData(pDict["currentwhere"].ToString());
                string orderBy = "Create_Time DESC";
                DataTable dt = ProjectInfoDAL.GetDepartmentTable(myWhere, orderBy);
                //if (dt.Rows.Count == 0)
                //{
                //    sbHtml.Append("<tr><td colspan=\"15\" style=\"text-align:center\"><h5>没有数据...</h5></td></tr>");//没有数据
                //}
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbHtml.Append("<tr class=\"text-c\">");
                    sbHtml.AppendFormat("<td><input type=\"checkbox\" value=\"{0}\" name=\"selAllData\" class=\"pointer\"></td>", dt.Rows[i]["DepartmentGUID"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", i + 1);
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["DepartmentName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["DepartmentNote"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Create_Time"].ToString());
                    //操作
                    sbHtml.Append("<td>");
                    sbHtml.AppendFormat("<span><a style=\"text-decoration:none\" class=\"ml-5\" href=\"/Project/DepartmentInfo?DepartmentGUID={1}\" title=\"{0}\"><i class=\"Hui-iconfont\">&#xe6df;</i></a></span>", "修改", dt.Rows[i]["DepartmentGUID"].ToString());//修改
                    sbHtml.AppendFormat("<span><a title=\"{0}\" href=\"javascript:;\" onclick=\"del_info(this,'{1}')\" class=\"ml-5\" style=\"text-decoration:none\"><i class=\"Hui-iconfont\">&#xe6e2;</i></a></span>", "删除", dt.Rows[i]["DepartmentGUID"].ToString());//删除
                    sbHtml.Append("</td>");
                    sbHtml.Append("</tr>");
                }
                sbHtml.Append("</tbody>");

                message.Success = true;
                message.ReturnString = sbHtml.ToString();
                message.StrReturn = dt.Rows.Count.ToString();
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetDepartmentInfoData(string id)
        {
            Message message = new Message();
            try
            {
                string myWhere = "DepartmentGUID='" + id + "'";
                DataTable dt = ProjectInfoDAL.GetDepartmentTable(myWhere, "");
                if (dt.Rows.Count > 0)
                {
                    message.Success = true;
                    message.ReturnString = JsonHelper.DataTableToJson2(dt);
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "数据加载失败！";//数据加载失败！
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 保存部门数据
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult SaveDepartmentInfoData(string jsonData)
        {
            Message message = new Message();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //保存数据
                string result = ProjectInfoDAL.SaveDepartmentInfoData(pDict, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "保存成功！";
                }
                else if (result == "-1")
                {
                    message.Success = false;
                    message.ReturnString = "部门名称重复！";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "保存失败！";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 删除部门信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult DelDepartmentInfoData(string id)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //删除数据
                string result = ProjectInfoDAL.DelDepartmentInfo(id, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "删除成功";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "删除失败";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }

        /// <summary>
        /// 批量删除部门信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult DeleteDepartmentInfoData(string idList)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录账号信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //批量删除数据
                string result = ProjectInfoDAL.DeleteDepartmentInfo(idList, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "删除成功";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "删除失败";
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }
        #endregion

    }
}