using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsServer
{
    public  class ServerMySQLControl
    {
        //构造函数
        public ServerMySQLControl()
        {
            MySqlConnection = new MySqlConnection("server=localhost;User Id=GameServer;password=forever;Database=citadelsdb");
            MySqlConnection.Open();
        }

        private MySqlConnection _mySqlConnection;
        /// <summary>
        /// 生成Mysql连接属性
        /// </summary>
        public MySqlConnection MySqlConnection
        {
            get
            {
                return _mySqlConnection;
            }

            set
            {
                _mySqlConnection = value;
            }
        }

        private MySqlCommand _mySqlCmd;
        /// <summary>
        /// 生成mysql命令
        /// </summary>
        public MySqlCommand MySqlCmd
        {
            get
            {
                return _mySqlCmd;
            }

            set
            {
                _mySqlCmd = value;
            }
        }
        /// <summary>
        /// 在citadelsdb的GameUser表中插入数据
        /// </summary>
        /// <param name="mail"></param>
        /// <param name="nickname"></param>
        /// <param name="pwd"></param>
        /// <param name="realName"></param>
        /// <returns></returns>
        public bool InsertToDb(string mail, string nickname, string pwd, string realName)
        {
            MySqlCmd.CommandText = String.Format("insert into GameUser(GameUser_Mail,GameUser_Nickname,GameUser_Pwd,GameUser_Name,GameUser_Exp) values('{0}','{1}','{2}','{3}',0)", mail, nickname, pwd, realName);
            MySqlCmd.Connection = MySqlConnection;
            if (MySqlCmd.ExecuteNonQuery() > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        ///在citadelsdb的GameUser表中依靠邮箱查询
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public GameUser SelecInDb(string mail)
        {
            GameUser gameUser = new GameUser();
            if (!IsExistInDb(mail))
            {
                gameUser.Mail = "null";
                return gameUser;
            }
            MySqlCmd.CommandText = String.Format("select* from GameUser where GameUser_Mail='{0}'", mail);
            MySqlCmd.Connection = MySqlConnection;
            MySqlDataReader reader = MySqlCmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader.HasRows)
                {
                    gameUser = new GameUser(reader.GetString(1), reader.GetString(2), reader.GetString(3), reader.GetString(4), reader.GetString(5));
                }
            }
            reader.Close();
            return gameUser;
        }
        /// <summary>
        /// 看mail是否存在
        /// </summary>
        /// <param name="mail"></param>
        /// <returns></returns>
        public bool IsExistInDb(string mail)
        {
            MySqlCmd = new MySqlCommand(String.Format("select count(*) from GameUser where GameUser_Mail='{0}'", mail), MySqlConnection);
            if(Convert.ToInt32(MySqlCmd.ExecuteScalar())>0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
