using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp3
{
    //储存用户信息
    public class UserEntity
    {
        //用户ID
        public int UserID { private set; get; }
        //用户名
        public string UserName { private set; get; }
        //真实姓名
        public string TrueName { private set; get; }

        //权限表
        private HashSet<string> Functions { set; get; } = new HashSet<string>();

        protected UserEntity() { }
        public UserEntity(int userid, string username, string truename, string ip, List<string> funcs)
        {
            UserID = userid;
            UserName = username;
            TrueName = truename;

            foreach (var func in funcs)
            {
                Functions.Add(func.Trim().ToLower());
            }
        }


        //检查此用户是否具有指定函数权限，funcname必须是非空且小写
        public bool CheckFunction(string api)
        {
            return Functions.Contains(api);
        }

        public object Clone()
        {
            UserEntity user = new UserEntity();

            user.TrueName = TrueName;
            user.UserID = UserID;
            user.UserName = UserName;
            user.Functions = new HashSet<string>();

            foreach (var item in Functions)
            {
                user.Functions.Add(item);
            }

            return user;
        }
    }
}
