using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace ConsoleApp3
{
    public class SessionController : ApiController
    {
        public string CallIP { private set; get; }
        public string Token { private set; get; }
        public UserSession UserData { private set; get; }

        public void SetUserData(UserSession ud) => UserData = ud;
        public void SetToken(string token) => Token = token;

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken)
        {
            //取出IP放到CallIP字段
            string callip = GetCallIP(controllerContext.Request);

            CallIP = callip;
            return base.ExecuteAsync(controllerContext, cancellationToken);
        }

        private string GetCallIP(HttpRequestMessage request)
        {
            if (request.Properties.TryGetValue("System.ServiceModel.Channels.RemoteEndpointMessageProperty", out var property))
            {
                var remp = property as RemoteEndpointMessageProperty;

                if (remp != null)
                {
                    return remp.Address;
                }
            }
            return string.Empty;
        }

    }
}
