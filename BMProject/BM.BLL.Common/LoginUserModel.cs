using System.Web;

namespace BM.BLL.Common
{
    public class LoginUserModel
    {
        //用户信息
        public string UserGUID { get; set; }
        public string UserName { get; set; }
        public string UserSex { get; set; }
        public string UserRoleGUID { get; set; }
        public string UserRoleName { get; set; }
        public string LoginIP { get; set; }
        public string LoginTime { get; set; }

        //其它信息
        public string SysRsc_No { get; set; }//一级菜单
        public string MenuRsc_No { get; set; }//二级菜单
        public string ProjectName { get; set; }//系统名称
        public string CompanyName { get; set; }//公司名称

        /// <summary>
        /// 已登录用户信息
        /// </summary>
        public static LoginUserModel LoginUser
        {
            get
            {
                return (LoginUserModel)HttpContext.Current.Session["LoginUser"];
            }
            set
            {
                HttpContext.Current.Session["LoginUser"] = value;
            }
        }
    }
}
