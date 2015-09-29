using CitadelsClient.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CitadelsClient.View
{
    /// <summary>
    /// LobbyWin.xaml 的交互逻辑
    /// </summary>
    public partial class LobbyWin : Window
    {
        public LobbyWin()
        {
            InitializeComponent();
            listView.DataContext = lobbyVm.RoomInfoList;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            RoomInfo r2 = new RoomInfo();
            r2.Id = 1;
            r2.NickNameList.Add("r2s1");
            r2.Num = 1;
            r2.NickNameList.Add("r2s2");
            r2.Num = 1;
            lobbyVm.RoomInfoList.Add(r2);
            lobbyVm.RoomInfoList[0].NickNameList.Add("ha");
            lobbyVm.RoomInfoList[0].Num = 0;
        }
        
    }
}
