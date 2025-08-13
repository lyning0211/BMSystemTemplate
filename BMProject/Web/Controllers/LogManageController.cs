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
    //日志管理
    public class LogManageController : HelperController
    {
        //用户登录日志信息页面
        [FilterConfig]
        public ActionResult LogLoginIndex()
        {
            return View();
        }

        //数据操作日志信息页面
        [FilterConfig]
        public ActionResult LogOperationIndex()
        {
            return View();
        }

        //数据修改痕迹日志信息页面
        [FilterConfig]
        public ActionResult LogTraceIndex()
        {
            return View();
        }


        /*数据*/
        /// <summary>
        /// 获取登录日志信息列表
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetLogLoginTableData(string jsonData)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                //获取缓存登录用户信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                sbHtml.Append("<thead><tr class=\"text-c\">");
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "序号");//序号
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "用户名");//用户名
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "角色");//角色
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "登录时间");//登录时间
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "登录IP");//登录IP
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "备注");//备注
                sbHtml.Append("</tr></thead><tbody class=\"body\">");

                string myWhere = BuildWhereData(pDict["currentwhere"].ToString());
                string orderBy = "Operation_Time DESC";
                DataTable dt = LogInfoDAL.GetLogLoginTable(int.Parse(pDict["currentpageindex"].ToString()) - 1,
                                                        int.Parse(pDict["pagesize"].ToString()), "*", myWhere, orderBy);
                if (dt.Rows.Count == 0)
                {
                    sbHtml.Append("<tr><td colspan=\"15\" style=\"text-align:center\"><h5>没有数据...</h5></td></tr>");
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbHtml.Append("<tr class=\"text-c\">");
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", i + 1);
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["UserName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["UserRoleName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_Time"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_IP"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_Note"].ToString());
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
        /// 获取登录日志信息总记录数
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetLogLoginRecordCountData(string jsonWhere)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                string myWhere = BuildWhereData(jsonWhere);
                int recordCount = CommonDAL.GetRecordCount("Log_Login_v", myWhere);

                message.Success = true;
                message.ReturnString = recordCount.ToString();
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }

        /// <summary>
        /// 获取数据操作日志信息列表
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetLogOperationTableData(string jsonData)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                //获取缓存登录用户信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                sbHtml.Append("<thead><tr class=\"text-c\">");
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "序号");//序号
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "操作对象");//操作对象
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "操作类型");//操作类型
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "用户名");//用户名
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "用户角色");//用户角色
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "操作时间");//操作时间
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "操作IP");//操作IP
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "备注");//备注
                sbHtml.Append("</tr></thead><tbody class=\"body\">");

                string myWhere = BuildWhereData(pDict["currentwhere"].ToString());
                string orderBy = "Operation_Time DESC";
                DataTable dt = LogInfoDAL.GetLogOperationTable(pDict["currentpageindex"].ToString().ToInt() - 1, pDict["pagesize"].ToInt(), "*", myWhere, orderBy);
                if (dt.Rows.Count == 0)
                {
                    sbHtml.Append("<tr><td colspan=\"15\" style=\"text-align:center\"><h5>没有数据...</h5></td></tr>");
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbHtml.Append("<tr class=\"text-c\">");
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", i + 1);
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Object_Name"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", cPublic.GetOperationTyp(dt.Rows[i]["Operation_Type"].ToString()));
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["UserName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["UserRoleName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_Time"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_IP"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_Note"].ToString());
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
        /// 获取数据操作日志信息总记录数
        /// </summary>
        /// <param name="jsonWhere"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetLogOperationRecordCountData(string jsonWhere)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                string myWhere = BuildWhereData(jsonWhere);
                int recordCount = CommonDAL.GetRecordCount("Log_Operation_v", myWhere);

                message.Success = true;
                message.ReturnString = recordCount.ToString();
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }

        /// <summary>
        /// 获取数据修改痕迹日志信息列表
        /// </summary>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetLogTraceTableData(string jsonData)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                //获取缓存登录用户信息
                pUserMapInfo = (LoginUserModel)Session["LoginUser"];
                Dictionary<string, object> pDict = JsonHelper.JSONToObject<Dictionary<string, object>>(jsonData);

                sbHtml.Append("<thead><tr class=\"text-c\">");
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "序号");//序号
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "操作对象");//操作对象
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "对象属性");//对象属性
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "旧值");//旧值
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "新值");//新值
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "操作类型");//操作类型
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "用户名");//用户名
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "用户角色");//用户角色
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "操作时间");//操作时间
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "操作IP");//操作IP
                sbHtml.AppendFormat("<th><span>{0}</span></th>", "备注");//备注
                sbHtml.Append("</tr></thead><tbody class=\"body\">");

                string myWhere = BuildWhereData(pDict["currentwhere"].ToString());
                string orderBy = "Operation_Time DESC";
                DataTable dt = LogInfoDAL.GetLogTraceTable(pDict["currentpageindex"].ToString().ToInt() - 1, pDict["pagesize"].ToInt(), "*", myWhere, orderBy);
                if (dt.Rows.Count == 0)
                {
                    sbHtml.Append("<tr><td colspan=\"15\" style=\"text-align:center\"><h5>没有数据...</h5></td></tr>");
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    sbHtml.Append("<tr class=\"text-c\">");
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", i + 1);
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Object_Name"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Object_Field"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", cPublic.GetTraceValue(dt.Rows[i]["Object_Type"].ToString(), dt.Rows[i]["Old_Value"].ToString()));
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", cPublic.GetTraceValue(dt.Rows[i]["Object_Type"].ToString(), dt.Rows[i]["New_Value"].ToString()));
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", cPublic.GetOperationTyp(dt.Rows[i]["Operation_Type"].ToString()));
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["UserName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["UserRoleName"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_Time"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_IP"].ToString());
                    sbHtml.AppendFormat("<td><span>{0}</span></td>", dt.Rows[i]["Operation_Note"].ToString());
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
        /// 获取数据修改痕迹日志信息总记录数
        /// </summary>
        /// <param name="jsonWhere"></param>
        /// <returns></returns>
        [FilterConfig]
        public JsonResult GetLogTraceRecordCountData(string jsonWhere)
        {
            Message message = new Message();
            StringBuilder sbHtml = new StringBuilder();
            try
            {
                string myWhere = BuildWhereData(jsonWhere);
                int recordCount = CommonDAL.GetRecordCount("Log_Trace_v", myWhere);

                message.Success = true;
                message.ReturnString = recordCount.ToString();
            }
            catch (Exception ex)
            {
                message.Success = false;
                message.ReturnString = ex.Message;
            }
            return Json(message);
        }

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
            if (dictList.ContainsKey("object_type") && dictList["object_type"].ToString() != "")
            {
                myWhere.Append(" AND Object_Type='" + dictList["object_type"] + "'");
            }
            if (dictList["queryvalue"].ToString() != "")
            {
                myWhere.Append(" AND " + dictList["queryname"] + " LIKE " + "'%" + dictList["queryvalue"] + "%'");
            }
            if (dictList["querystartdate"].ToString() != "")
            {
                myWhere.Append(" AND CONVERT(varchar(50),Operation_Time, 23) >=" + "'" + dictList["querystartdate"] + "'");
            }
            if (dictList["queryenddate"].ToString() != "")
            {
                myWhere.Append(" AND CONVERT(varchar(50),Operation_Time, 23) <=" + "'" + dictList["queryenddate"] + "'");
            }

            return myWhere.ToString();
        }
    }
}