using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace ConsoleApp3.Controller
{
    [RoutePrefix("user")]
    public class UserController : SessionController
    {
        [HttpPost]
        [Route("login")]
        public JObject Login([FromBody] JObject param)
        {
            JObject jobj = new JObject();

            try
            {
                string name = param["name"].ToString().Trim().ToLower();
                string pwd = param["pwd"].ToString().Trim().ToLower();


                if (!CheckUser(name, pwd, CallIP, out string errmsg, out UserEntity user, out List<string> funcs))
                {
                    if (string.IsNullOrWhiteSpace(errmsg))
                    {
                        return ErrCodeManager.GetError(ErrCode.Unknown);
                    }
                    else
                    {
                        return ErrCodeManager.GetError(2, errmsg);
                    }
                }

                SessionManager.Instance().AddSession(user, CallIP, out string token);

                JObject jdata = new JObject();

                jdata["token"] = token;
                jdata["ftruename"] = user.TrueName;

                jobj["errcode"] = 0;
                jobj["data"] = jdata;
            }
            catch (SqlException e)
            {
                jobj = ErrCodeManager.GetError(ErrCode.CallDBFuncError, e.Number.ToString());
            }
            catch (Exception e)
            {
                jobj = ErrCodeManager.GetError(ErrCode.Unknown);
            }

            return jobj;
        }

        [HttpPost, Session(false)]
        [Route("logout")]
        public JObject Logout([FromBody] JObject param)
        {
            JObject jobj = new JObject();

            SessionManager.Instance().RemoveSession(Token);

            jobj["errcode"] = 0;

            return jobj;
        }

        [HttpPost, Session(true)]
        [Route("my")]
        public JObject My([FromBody] JObject param)
        {
            JObject jobj = new JObject();

            JObject jdata = new JObject();
            jdata["token"] = Token;
            jdata["id"] = this.UserData.UserInfo.UserID;
            jdata["name"] = this.UserData.UserInfo.UserName;
            jdata["truename"] = this.UserData.UserInfo.TrueName;

            jobj["errcode"] = 0;
            jobj["data"] = jdata;

            return jobj;
        }


        [HttpPost, Session(true)]
        [Route("my2")]
        public JObject My2([FromBody] JObject param)
        {
            return My(param);
        }
        private bool CheckUser(string name, string pwd, string ip, out string errmsg, out UserEntity user, out List<string> funcs)
        {
            bool succ = false;

            funcs = null;
            user = null;

            try
            {
                //TODO 这里要改成从DB查询
                if (name != "admin" || pwd != "123456")
                {
                    errmsg = "账号或密码错误";
                    return false;
                }

                //TODO 这里改成从DB查询
                var userinfo = new
                {
                    uid = 1,
                    name = "admin",
                    truename = "管理员",
                    funcs = new List<string>() { "my" }
                };

                user = new UserEntity(userinfo.uid, userinfo.name, userinfo.truename, ip, userinfo.funcs);

                errmsg = "";
                succ = true;

            }
            catch (Exception e)
            {
                throw;
            }

            return succ;
        }
    }
}
