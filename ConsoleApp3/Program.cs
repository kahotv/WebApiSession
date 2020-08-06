using ConsoleApp3.Handler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace ConsoleApp3
{
    class Program
    {
        static void Main(string[] args)
        {
            string url = "http://0.0.0.0:9000/";

            var config = new HttpSelfHostConfiguration(url);

            config.MapHttpAttributeRoutes();
            config.MessageHandlers.Add(new CrossHandler()); //处理跨域

            //去除xml，更改为默认支持json
            config.Formatters.XmlFormatter.SupportedMediaTypes.Clear();
            config.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("datatype", "json", "application/json"));
            config.Formatters.XmlFormatter.MediaTypeMappings.Add(new QueryStringMapping("datatype", "xml", "application/xml"));

            try
            {
                using (var svr = new HttpSelfHostServer(config))
                {
                    svr.OpenAsync().Wait();

                    Console.WriteLine("程序路径 : " + Process.GetCurrentProcess().MainModule.FileName);
                    Console.WriteLine("Server Listen on address : " + url);

                    while (true)
                    {
                        string cmd = Console.ReadLine();

                        if (cmd == "exit")
                            break;
                    }

                }
            }
            catch (System.AggregateException ex2)
            {
                Console.WriteLine("启动失败:");
                for (int i = 0; i < ex2.InnerExceptions.Count; i++)
                {
                    Console.WriteLine("{0} => {1}", i, ex2.InnerExceptions[i].Message);
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine("启动失败:" + ex.Message);
            }

            Console.WriteLine("回车键退出");
            Console.ReadKey();
        }
    }
}
