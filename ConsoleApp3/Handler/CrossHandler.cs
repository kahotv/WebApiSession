using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp3.Handler
{
    public class CrossHandler : DelegatingHandler
    {
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            //响应Options
            if (request.Method == HttpMethod.Options)
            {
                return request.CreateResponse(HttpStatusCode.OK);
            }

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                response = request.CreateResponse<JObject>(ErrCodeManager.GetError(ErrCode.NotLoginOrNotFoundFuntion));
            }

            //TODO 这个Origin要改成自己的前端站点
            response.Headers.Add("Access-Control-Allow-Origin", "https://xxx.xxx.com");
            //这里定义允许跨域的Header，里面有个X-Token用于框架传递token，是自定义的
            response.Headers.Add("Access-Control-Allow-Headers", "Origin, X-Requested-With, Content-Type, Accept, X-Token");
            response.Headers.Add("Access-Control-Allow-Methods", "POST,OPTIONS");
            response.Headers.Add("Access-Control-Allow-Credentials", "true");

            return response;
        }
    }
}
