using System;
using System.IO;
using System.Web;

namespace Web.Helper
{
    public class cPublic
    {
        public static HttpRequest Request
        {
            get
            {
                HttpContext current = HttpContext.Current;
                if (current == null || current.Request == null)
                {
                    return null;
                }

                return current.Request;
            }
        }

        /// <summary>
        /// 获取客户端IP地址(兼容阿里云负载均衡SLB)
        /// 取X-Forwarded-For中以","或者char(0)分隔的第一个ip（兼容华为云WAF）
        /// </summary>
        /// <param name="hr"></param>
        /// <returns></returns>
        public static string GetClientIp(HttpRequestBase hr)
        {
            string[] ip = hr.Headers.GetValues("X-Forwarded-For");
            string myIP = ip != null && ip.Length > 0 && !string.IsNullOrEmpty(ip[0]) ? ip[0].Split(',')[0].Trim().Split((char)0)[0].Trim() : hr.UserHostAddress;
            if (myIP.Length > 30) myIP = myIP.Substring(myIP.Length - 30, 30);
            return myIP;
        }

        //接收返回的值
        public static string GetQueryString(string strName)
        {
            return GetQueryString(Request, strName);
        }

        public static string GetQueryString(HttpRequest request, string strName)
        {
            if (request == null || request.QueryString[strName] == null)
            {
                return string.Empty;
            }

            return request.QueryString[strName];
        }

        //记录日志
        public static void WriteLog(string pcErrorMsg, string pLog_Name)
        {
            try
            {
                FileStream fs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Log/" + pLog_Name + ".log", FileMode.Append);
                StreamWriter sw = new StreamWriter(fs);
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": " + pcErrorMsg);
                sw.WriteLine("-----------------------------------------------------------------------------------------------------------------------------");
                sw.Flush();//清空缓冲区
                sw.Close();
                fs.Close();//关闭流
            }
            catch { }
        }

        //解析痕迹日志操作值
        public static string GetTraceValue(string object_type, string value)
        {
            string resultValue = value;
            if (object_type == "user")
            {
                switch (value.ToLower())
                {
                    case "-1":
                        resultValue = "停用";//停用
                        break;
                    case "0":
                        resultValue = "启用";//启用
                        break;
                    case "m":
                        resultValue = "男";//男
                        break;
                    case "f":
                        resultValue = "女";//女
                        break;
                    default:
                        resultValue = value;
                        break;

                }
            }

            return resultValue;
        }

        //解析日志操作类型
        public static string GetOperationTyp(string value)
        {
            string resultValue = "";
            switch (value.ToLower())
            {
                case "register":
                    resultValue = "注册";//注册
                    break;
                case "add":
                    resultValue = "添加";//添加
                    break;
                case "update":
                    resultValue = "修改";//修改
                    break;
                case "delete":
                    resultValue = "删除";//删除
                    break;
                case "reset password":
                    resultValue = "重置密码";//重置密码
                    break;
                case "update password":
                    resultValue = "修改密码";//修改密码
                    break;
                case "import":
                    resultValue = "导入";//导入
                    break;
                case "export":
                    resultValue = "导出";//导出
                    break;
                default:
                    resultValue = value;
                    break;

            }

            return resultValue;
        }
    }
}