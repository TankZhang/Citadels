using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer
{
    public  class GameUser
    {
        private string _mail;
        public string Mail
        {
            get
            {
                return _mail;
            }

            set
            {
                _mail = value;
            }
        }

        private string _nickName;
        public string NickName
        {
            get
            {
                return _nickName;
            }

            set
            {
                _nickName = value;
            }
        }
        
        private string _pwd;
        public string Pwd
        {
            get
            {
                return _pwd;
            }

            set
            {
                _pwd = value;
            }
        }

        private string _realName;
        public string RealName
        {
            get
            {
                return _realName;
            }

            set
            {
                _realName = value;
            }
        }
        
        private string _exp;
        public string Exp
        {
            get
            {
                return _exp;
            }

            set
            {
                _exp = value;
            }
        }

        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }

            set
            {
                _status = value;
            }
        }

        //构造函数
        public GameUser()
        { }
        public GameUser(string mail,string nickName,string pwd,string realName,string exp)
        {
            Mail = mail;
            NickName = nickName;
            Pwd = pwd;
            RealName = realName;
            Exp = exp;
        }
    }
}
