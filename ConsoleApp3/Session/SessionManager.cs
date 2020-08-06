using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    /// <summary>
    /// 管理所有的在线用户
    /// </summary>
    public class SessionManager
    {
        private static SessionManager m_sessionMgr = new SessionManager();

        //session表   <token,user>
        private ConcurrentDictionary<string, UserSession> FUserSessionList = new ConcurrentDictionary<string, UserSession>();
        //会话超时时间（分）
        private int FTimeOut = 5;

        private SessionManager()
        {
            try
            {
                //FTimeOut = Convert.ToInt32(ConfigurationManager.AppSettings["Session_TimeOut"]);
                //启动定时器，处理超时会话
                if (FTimeOut > 0)
                    (new System.Threading.Thread(RunTimer)).Start();
            }
            catch
            {
                //默认超时时间5分钟
                FTimeOut = 5;
            }
        }

        public static SessionManager Instance()
        {
            return m_sessionMgr;
        }

        /// <summary>
        /// 检查会话是否有效，并返回UserEntity
        public bool CheckSessionAndUpdateActive(string token, out UserSession user)
        {
            //如果有该用户会话信息

            lock (FUserSessionList)
            {
                if (FUserSessionList.TryGetValue(token, out UserSession tmp))
                {
                    if (!tmp.IsLoginTimeout(FTimeOut))
                    {
                        tmp.UpdateActive();
                        user = tmp;
                        return true;
                    }
                    else
                    {
                        //超时了就清除
                        FUserSessionList.TryRemove(token, out tmp);
                        user = tmp;
                        return false;
                    }
                }
            }

            user = null;
            return false;
        }

        //添加Session，如果已经存在就返回已存在的内容
        public void AddSession(UserEntity userin, string ip, out string token)
        {
            lock (FUserSessionList)
            {
                var list = FUserSessionList.Where(p => p.Value.UserInfo.UserID == userin.UserID);

                if (list.Count() > 0)
                {
                    //用户已登陆，更新UserEntity、LoginIP、活动时间
                    token = list.First().Key;
                    list.First().Value.SetNewLoginInfo(userin, ip);
                    list.First().Value.UpdateActive();
                }
                else
                {
                    //用户未登陆，创建一个
                    UserSession session = new UserSession(userin, ip);

                    token = Guid.NewGuid().ToString().ToUpper();

                    FUserSessionList.TryAdd(token, session);
                }
            }
        }

        public void RemoveSession(string token)
        {
            FUserSessionList.TryRemove(token, out UserSession user);
        }

        void RunTimer()
        {
            //TODO 这里清理过期的session，用户较少可以不清理
        }
    }
}
