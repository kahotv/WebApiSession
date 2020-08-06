using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    //储存用户信息+登陆信息
    public class UserSession
    {
        public UserEntity UserInfo { private set; get; }

        //登陆IP
        public string LoginIP { private set; get; }

        //登陆时间
        public DateTime LoginTime { get; } = DateTime.Now;

        //最后活动时间
        public DateTime LastActiveTime { private set; get; } = DateTime.Now;

        public Dictionary<string, object> SessionData { set; get; } = new Dictionary<string, object>();

        public UserSession(UserEntity user, string ip)
        {
            UserInfo = user.Clone() as UserEntity;
            LoginIP = ip;
        }

        //设置用户信息
        public void SetNewLoginInfo(UserEntity user, string ip)
        {
            UserInfo = user.Clone() as UserEntity;
            LoginIP = ip;
        }

        //更新用户最后活动时间
        public void UpdateActive()
        {
            LastActiveTime = DateTime.Now;
        }

        //是否登陆超时
        public bool IsLoginTimeout(int timeout_min)
        {
            if ((DateTime.Now - LastActiveTime).TotalMinutes > timeout_min)
                return true;
            return false;
        }
    }
}
