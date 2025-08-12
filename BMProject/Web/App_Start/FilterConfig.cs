using System.Web;
using System.Web.Mvc;

namespace Web
{
    public class FilterConfig : ActionFilterAttribute, IAuthorizationFilter
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        /// <summary>
        /// 登录授权过滤器
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            //如果Session为空、则跳转回登录首页
            if (HttpContext.Current.Session["LoginUser"] == null)
            {
                filterContext.Result = new RedirectResult("/Home/Default");
            }
        }
    }
}
