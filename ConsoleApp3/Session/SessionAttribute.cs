using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace ConsoleApp3
{
    public class SessionAttribute : ActionFilterAttribute
    {
        private bool m_checkapi = false;            //是否检查api权限
        private SessionAttribute() { }
        public SessionAttribute(bool checkapi)
        {
            m_checkapi = checkapi;
        }

        /// <summary>
        /// session Filiter，在需要验证session的函数使用这个标签
        /// </summary>
        /// <param name="actionContext"></param>
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            UserSession user = null;
            string token = "";
            do
            {
                var headers = actionContext.Request.Headers;
                if (!headers.TryGetValues("X-Token", out var list))
                    break;

                if (list == null || list.Count() == 0)
                    break;

                token = list.First();

                //用token里取出UserEntity
                if (!SessionManager.Instance().CheckSessionAndUpdateActive(token, out user))
                    break;

            } while (false);

            if (user == null)
            {
                //未登录
                actionContext.Response = actionContext.Request.CreateResponse<JObject>(ErrCodeManager.GetError(ErrCode.NotLoginOrNotFoundFuntion));
                return;
            }

            SessionController controller = actionContext.ControllerContext.Controller as SessionController;

            //IP变动判断
            if (controller.CallIP != user.LoginIP)
            {
                actionContext.Response = actionContext.Request.CreateResponse<JObject>(ErrCodeManager.GetError(ErrCode.IPChanged));
                return;
            }

            //API权限判断
            if (m_checkapi)
            {
                if (!GetCallPath(actionContext.Request.RequestUri, out string api))
                {
                    //Uri格式错误等于接口没找到
                    actionContext.Response = actionContext.Request.CreateResponse<JObject>(ErrCodeManager.GetError(ErrCode.NotLoginOrNotFoundFuntion));
                    return;
                }

                if (!user.UserInfo.CheckFunction(api))
                {
                    //权限不足
                    actionContext.Response = actionContext.Request.CreateResponse<JObject>(ErrCodeManager.GetError(ErrCode.NotLoginOrNotFoundFuntion));
                    return;
                }
            }

            //放入User
            controller.SetUserData(user);
            controller.SetToken(token);

        }
        //获取模块名和函数名（小写）
        public static bool GetCallPath(Uri uri, out string api)
        {
            //取末尾作为api名
            api = uri.Segments.LastOrDefault();

            if (!string.IsNullOrWhiteSpace(api))
            {
                api = uri.Segments.Last().ToLower();
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
