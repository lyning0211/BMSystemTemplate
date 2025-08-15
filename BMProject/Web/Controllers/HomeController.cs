using BM.BLL.Common;
using BM.BLL.Common.Common;
using BM.BLL.Manage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Mvc;
using Web.App_Code;

namespace Web.Controllers
{
    public class HomeController : HelperController
    {
        /// <summary>
        /// 用户登录页面
        /// </summary>
        /// <returns></returns>
        public ActionResult Default()
        {
            LoginUserModel.LoginUser = null;
            //系统名称
            string projectName = SystemManageBLL.GetParameterValueByName("project_name");
            //公司名称
            string companyName = SystemManageBLL.GetParameterValueByName("company_name");
            ViewBag.ProjectName = projectName;
            ViewBag.CompanyName = companyName;
            ViewBag.Year = DateTime.Now.ToString("yyyy");
            return View();
        }

        //用户信息页面（添加/修改）
        public ActionResult RegisterUserInfo()
        {
            return View();
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <returns></returns>
        [FilterConfig]
        public ActionResult Index()
        {
            //获取缓存登录用户信息
            pUserMapInfo = (LoginUserModel)Session["LoginUser"];
            if (pUserMapInfo != null)
            {
                ViewBag.ProjectName = pUserMapInfo.ProjectName;
                ViewBag.UserGUID = pUserMapInfo.UserGUID;
                ViewBag.UserName = pUserMapInfo.UserName;
                ViewBag.UserRoleName = pUserMapInfo.UserRoleName;
            }

            //用户权限菜单
            string htmlModule = GetUserModuleData(pUserMapInfo.UserRoleGUID);
            ViewBag.HtmlModuleData = htmlModule;

            return View();
        }

        /// <summary>
        /// 欢迎页
        /// </summary>
        /// <returns></returns>
        [FilterConfig]
        public ActionResult Welcome()
        {
            //获取缓存登录用户信息
            pUserMapInfo = (LoginUserModel)Session["LoginUser"];
            if (pUserMapInfo != null)
            {
                ViewBag.ProjectName = pUserMapInfo.ProjectName;
                ViewBag.CompanyName = pUserMapInfo.CompanyName;
                ViewBag.UserGUID = pUserMapInfo.UserGUID;
                ViewBag.UserName = pUserMapInfo.UserName;
                ViewBag.RoleName = pUserMapInfo.UserRoleName;
                ViewBag.LoginIP = pUserMapInfo.LoginIP;
                ViewBag.Logintime = pUserMapInfo.LoginTime;
                ViewBag.Year = DateTime.Now.Year;
            }
            return View();
        }

        /// <summary>
        /// 修改密码页面
        /// </summary>
        /// <returns></returns>
        [FilterConfig]
        public ActionResult UpdatePwd()
        {
            string userGUID = cPublic.GetQueryString("UserGUID");
            string userName = "";
            DataTable dt = ProjectInfoBLL.GetUserTable("userGUID='" + userGUID + "'", "");
            if (dt != null && dt.Rows.Count > 0)
            {
                userName = dt.Rows[0]["UserName"].ToString();
            }
            ViewBag.UserGUID = userGUID;
            ViewBag.UserName = userName;
            return View();
        }


        /*数据*/
        /// <summary>
        /// 生成验证码
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateVerifyCodeData()
        {
            VerifyCode vc = new VerifyCode();
            byte[] result = vc.GetVerifyCode();
            return File(result, "image/jpeg jpeg jpg jpe");
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public JsonResult UserLoginData(string jsonData)
        {
            Message message = new Message();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);
                //if (HttpContext.Session["VerifyCode"] != null)
                //{
                //    //获取生成的session缓存的验证码
                //    string sessionVCode = HttpContext.Session["VerifyCode"].ToString().ToLower();
                //    if (pDict["UserName"].ToString().ToLower() != sessionVCode)
                //    {
                //        message.Success = false;
                //        message.ReturnString = "验证码错误";

                //        return Json(message);
                //    }
                //}
                //else
                //{
                //    message.Success = false;
                //    message.ReturnString = "验证码生成失败";

                //    return Json(message);
                //}

                var request = this.HttpContext.Request; // 或简写为 HttpContext.Request
                string ip = cPublic.GetClientIp(request);

                string passWord = ExpandUtility.ToMD5(pDict["UserPWD"].ToString());
                string myWhere = "UserName='" + pDict["UserName"].ToString() + "' AND UserPassWord='" + passWord + "'";
                DataTable dtUser = ProjectInfoBLL.GetUserTable(myWhere, "");
                if (dtUser.Rows.Count > 0)
                {
                    if (dtUser.Rows[0]["UserStatus"].ToString() != "0")
                    {
                        message.Success = false;
                        message.ReturnString = "登录失败，此账号已被禁止登录！";

                        return Json(message);
                    }

                    DataTable dtRole = ProjectInfoBLL.GetRoleTable("RoleID='" + dtUser.Rows[0]["UserRoleID"].ToInt() + "'", "");
                    if (dtRole.Rows.Count <= 0)
                    {
                        message.Success = false;
                        message.ReturnString = "登录失败，此账号角色不存在！";

                        return Json(message);
                    }

                    //将登录信息写入Session
                    string projectName = SystemManageBLL.GetParameterValueByName("project_name");//系统名称
                    string companyName = SystemManageBLL.GetParameterValueByName("company_name");//公司名称
                    LoginUserModel pUserMapInfo = new LoginUserModel
                    {
                        UserID = dtUser.Rows[0]["UserID"].ToInt(),
                        UserGUID = dtUser.Rows[0]["UserGUID"].ToString(),
                        UserName = dtUser.Rows[0]["UserName"].ToString(),
                        UserSex = dtUser.Rows[0]["UserSex"].ToString(),
                        UserRoleID = dtUser.Rows[0]["UserRoleID"].ToInt(),
                        UserRoleGUID = dtRole.Rows[0]["RoleGUID"].ToString(),
                        UserRoleName = dtRole.Rows[0]["RoleName"].ToString(),
                        LoginIP = ip,
                        LoginTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        ProjectName = projectName,
                        CompanyName = companyName
                    };
                    LoginUserModel.LoginUser = pUserMapInfo;

                    #region 添加登录日志
                    Dictionary<string, object> logDict = new Dictionary<string, object>();
                    logDict.Add("UserID", dtUser.Rows[0]["UserID"].ToString());
                    logDict.Add("UserName", dtUser.Rows[0]["UserName"].ToString());
                    logDict.Add("UserRoleID", dtUser.Rows[0]["UserRoleID"].ToString());
                    logDict.Add("UserRoleName", dtUser.Rows[0]["RoleName"].ToString());
                    logDict.Add("Operation_IP", ip);
                    logDict.Add("Operation_Note", "后台登录");
                    string strRes = LogInfoBLL.SaveLog_Login(logDict);
                    #endregion

                    message.Success = true;
                    message.ReturnString = "登录成功！";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "登录失败，账号或密码错误！";
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
        /// 绑定角色信息
        /// </summary>
        /// <returns></returns>
        public JsonResult BindRoleData()
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                DataTable dtRoles = ProjectInfoBLL.GetRoleTable("RoleType<>'sa'", "RoleName");
                sbHtml.Append("<option value=\"\"></option>");
                if (dtRoles != null && dtRoles.Rows.Count > 0)
                {
                    foreach (DataRow dr in dtRoles.Rows)
                    {
                        sbHtml.AppendFormat("<option value=\"{0}\">{1}</option>", dr["RoleID"], dr["RoleName"]);
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
        /// 注册用户信息
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public JsonResult RegisterUserInfoData(string jsonData)
        {
            Message message = new Message();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);
                var request = this.HttpContext.Request; // 或简写为 HttpContext.Request
                string ip = cPublic.GetClientIp(request);
                pDict.Add("Operation_IP", ip);

                string passWord = ExpandUtility.ToMD5(pDict["UserPassWord"].ToString());
                pDict.Add("UserPassWordMD5", passWord);

                //保存数据
                string result = ProjectInfoBLL.RegisterUserInfoData(pDict);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "注册成功！";
                }
                else if (result == "-1")
                {
                    message.Success = false;
                    message.ReturnString = "用户名重复！";
                }
                else
                {
                    message.Success = false;
                    message.ReturnString = "注册失败！";
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
        /// 获取用户权限菜单
        /// </summary>
        /// <param name="userRoleGUID">用户角色GUID</param>
        /// <returns></returns>
        public string GetUserModuleData(string userRoleGUID)
        {
            string result = "";
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                //角色父级菜单
                DataTable dtParentModule = ProjectInfoBLL.GetParentModule(userRoleGUID);
                //角色子级菜单
                DataTable dtChildModule = ProjectInfoBLL.GetChildModule(userRoleGUID);

                for (int i = 0; i < dtParentModule.Rows.Count; i++)
                {
                    sbHtml.Append("<dl>");
                    sbHtml.AppendFormat("<dt>{0}<i class=\"Hui-iconfont menu_dropdown-arrow\">&#xe6d5;</i></dt>", dtParentModule.Rows[i]["ModuleName"].ToString());
                    sbHtml.Append("<dd>");
                    sbHtml.Append("<ul>");
                    for (int j = 0; j < dtChildModule.Rows.Count; j++)
                    {
                        if (dtChildModule.Rows[j]["ParentID"].ToString() == dtParentModule.Rows[i]["ModuleID"].ToString())
                            sbHtml.AppendFormat("<li><a data-href=\"{1}\" data-title=\"{0}\" href=\"javascript:;\">{0}</a></li>", dtChildModule.Rows[j]["ModuleName"].ToString(), dtChildModule.Rows[j]["Link"].ToString());
                    }
                    sbHtml.Append("</ul></dd></dl>");
                }
                result = sbHtml.ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public JsonResult UpdatePwdData(string jsonData)
        {
            Message message = new Message();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);
                //根据UserGUID获取用户信息
                DataTable dtUser = ProjectInfoBLL.GetUserTable("UserGUID='" + pDict["UserGUID"] + "'", "");
                if (dtUser.Rows.Count <= 0)
                {
                    message.Success = false;
                    message.ReturnString = "用户不存在！";

                    return Json(message);
                }

                string oldPwd = dtUser.Rows[0]["UserPassWord"].ToString();
                if (oldPwd.ToLower() != ExpandUtility.ToMD5(pDict["OldPwd"].ToString()).ToLower())
                {
                    message.Success = false;
                    message.ReturnString = "原密码输入错误，请重新输入";
                }
                else if (ExpandUtility.ToMD5(pDict["NewPwd"].ToString()).ToLower() == ExpandUtility.ToMD5(pDict["OldPwd"].ToString()).ToLower())
                {
                    message.Success = false;
                    message.ReturnString = "新密码与原密码重复";
                }
                else
                {
                    Dictionary<string, object> dictData = new Dictionary<string, object>();
                    dictData.Add("UserGUID", pDict["UserGUID"].ToString());
                    dictData.Add("UserPassWordMD5", ExpandUtility.ToMD5(pDict["NewPwd"].ToString()));

                    //获取缓存登录用户信息
                    pUserMapInfo = (LoginUserModel)Session["LoginUser"];
                    string result = ProjectInfoBLL.UpdateUserPwd(dictData, pUserMapInfo);
                    if (result == "")
                    {
                        message.Success = true;
                        message.ReturnString = "修改成功";
                    }
                    else
                    {
                        message.Success = false;
                        message.ReturnString = "密码修改失败";
                    }
                }
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }

            return Json(message);
        }
    }
}