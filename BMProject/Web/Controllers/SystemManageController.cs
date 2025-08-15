using BM.BLL.Common;
using BM.BLL.Common.Common;
using BM.BLL.Manage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web.Mvc;

namespace Web.Controllers
{
    //系统设置
    public class SystemManageController : HelperController
    {
        //参数配置页面
        [FilterConfig]
        public ActionResult SystemConfigIndex()
        {
            return View();
        }

        //参数配置信息页面（添加/修改）
        [FilterConfig]
        public ActionResult SystemConfigInfo()
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

            return myWhere.ToString();
        }

        #region 系统参数设置
        /// <summary>
        /// 获取参数配置数据列表
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetParameterTableData(string jsonData)
        {
            Message message = new Message();
            try
            {
                StringBuilder sbHtml = new StringBuilder();
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                sbHtml.Append("<thead><tr class=\"text-c\">");
                sbHtml.Append("<th width=\"25\"><input type=\"checkbox\" onchange=\"selAll(this);\" name=\"\" value=\"\" class=\"pointer\"></th>");//全选
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "序号");//序号
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "键名");//键名
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "键值");//键值
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "备注");//备注
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "创建时间");//创建时间
                sbHtml.AppendFormat("<th width=\"120\"><span>{0}</span></th>", "操作");//操作
                sbHtml.Append("</tr></thead><tbody class=\"body\">");

                string myWhere = BuildWhereData(pDict["currentwhere"].ToString());
                string orderBy = "Create_Time DESC";
                DataTable dt = SystemManageBLL.GetParameterTable(myWhere, orderBy);
                if (dt.Rows.Count == 0)
                {
                    sbHtml.Append("<tr><td colspan=\"15\" style=\"text-align:center\"><h5>没有数据...</h5></td></tr>");
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbHtml.Append("<tr class=\"text-c\">");
                    sbHtml.AppendFormat("<td><input type=\"checkbox\" value=\"{0}\" name=\"selAllData\" class=\"pointer\"></td>", dt.Rows[i]["ParameterName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", i + 1);
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["ParameterName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["ParameterValue"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Notes"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Create_Time"].ToString());
                    //操作
                    sbHtml.Append("<td>");
                    sbHtml.AppendFormat("<span><a style=\"text-decoration:none\" class=\"ml-5\" href=\"/SystemManage/SystemConfigInfo?ParameterName={1}\" title=\"{0}\"><i class=\"Hui-iconfont\">&#xe6df;</i></a></span>", "修改", dt.Rows[i]["ParameterName"].ToString());//修改
                    sbHtml.AppendFormat("<span><a title=\"{0}\" href=\"javascript:;\" onclick=\"del_info(this,'{1}')\" class=\"ml-5\" style=\"text-decoration:none\"><i class=\"Hui-iconfont\">&#xe6e2;</i></a></span>", "删除", dt.Rows[i]["ParameterName"].ToString());//删除
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
        /// 获取参数配置信息
        /// </summary>
        /// <param name="parameterName"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetParameterInfoData(string parameterName)
        {
            Message message = new Message();
            try
            {
                string myWhere = "ParameterName='" + parameterName + "'";
                DataTable dt = SystemManageBLL.GetParameterTable(myWhere, "");
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
        /// 保存参数配置数据
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult SaveParameterInfoData(string jsonData)
        {
            Message message = new Message();
            try
            {
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                //获取缓存登录用户信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];

                //保存数据
                string result = SystemManageBLL.SaveParameterInfoData(pDict, pUserMapInfo);
                if (result == "")
                {
                    message.Success = true;
                    message.ReturnString = "保存成功！";
                }
                else if (result == "-1")
                {
                    message.Success = false;
                    message.ReturnString = "键名重复！";
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
        /// 删除参数配置信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult DelParameterInfoData(string id)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录用户信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];
                string result = SystemManageBLL.DelParameterInfo(id, pUserMapInfo);
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
        /// 批量删除参数配置信息
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult DeleteParameterInfoData(string idList)
        {
            Message message = new Message();
            try
            {
                //获取缓存登录用户信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];
                string result = SystemManageBLL.DeleteParameterInfo(idList, pUserMapInfo);
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