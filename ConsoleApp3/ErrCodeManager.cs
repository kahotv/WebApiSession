using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    public class ErrCodeManager
    {
        public static JObject m_errcode_0 = JObject.Parse("{\"errcode\":0}");
        public static JObject m_errcode_e1 = JObject.Parse("{\"errcode\":-1,\"errmsg\":\"未知错误\"}");
        public static JObject m_errcode_e2 = JObject.Parse("{\"errcode\":-2,\"errmsg\":\"权限不足或接口未找到\"}");
        public static JObject m_errcode_e3 = JObject.Parse("{\"errcode\":-3,\"errmsg\":\"入参错误，可能是类型错误，也可能是缺少参数\"}");
        //public static JObject m_errcode_e4 = //数据库错误
        public static JObject m_errcode_e5 = JObject.Parse("{\"errcode\":-5,\"errmsg\":\"您的IP被限制访问\"}");
        public static JObject m_errcode_e6 = JObject.Parse("{\"errcode\":-6,\"errmsg\":\"您的IP发生变动，请从新登陆\"}");
        public static JObject m_errcode_e7 = JObject.Parse("{\"errcode\":-7,\"errmsg\":\"入参错误，不是有效的json结构\"}");
        public static JObject m_errcode_e8 = JObject.Parse("{\"errcode\":-8,\"errmsg\":\"Host错误\"}");
        public static JObject m_errcode_e9 = JObject.Parse("{\"errcode\":-9,\"errmsg\":\"客户端证书错误或不匹配\"}");

        //已定义错误
        public static JObject GetError(ErrCode code, string msg = "")
        {
            switch (code)
            {
                case ErrCode.Success:
                    return m_errcode_0;
                case ErrCode.Unknown:
                    return m_errcode_e1;
                case ErrCode.NotLoginOrNotFoundFuntion:
                    return m_errcode_e2;
                case ErrCode.RequestParamsError:
                    return m_errcode_e3;
                case ErrCode.CallDBFuncError:
                    return JObject.Parse("{\"errcode\":-4,\"errmsg\":\"调用DB接口出现错误(" + msg + ")\"}");
                case ErrCode.IPBlock:
                    return m_errcode_e5;
                case ErrCode.IPChanged:
                    return m_errcode_e6;
                case ErrCode.RequestParamsNotJson:
                    return m_errcode_e7;
                case ErrCode.HostError:
                    return m_errcode_e8;
                case ErrCode.ClientCertError:
                    return m_errcode_e9;
                default:
                    return m_errcode_e1;
            }
        }

        //自定义错误
        public static JObject GetError(int code, string msg)
        {
            JObject jobj = new JObject();

            jobj["errcode"] = code;
            jobj["errmsg"] = msg;

            return jobj;
        }
    }

    public enum ErrCode
    {
        Success = 0,
        Unknown = -1,
        NotLoginOrNotFoundFuntion = -2,
        RequestParamsError = -3,
        CallDBFuncError = -4,
        IPBlock = -5,
        IPChanged = -6,
        RequestParamsNotJson = -7,
        HostError = -8,
        ClientCertError = -9
    }
}
