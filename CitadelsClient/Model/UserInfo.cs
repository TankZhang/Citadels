using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsClient.Model
{
   public  class UserInfo
    {
        string _mail;
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
        
        string _pwd;
        public string Pwd
        {
            get
            {
                return _pwd;
            }

            set
            {
                _pwd = DataCtrl.Encryption(value);
            }
        }

        string _nickName;
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

        string _realName;
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

        string _exp;
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

    }
}
