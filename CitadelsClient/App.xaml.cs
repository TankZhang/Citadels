using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CitadelsClient
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
       public static ClientNetControl clientNetControl;

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            clientNetControl.SocketClient.Disconnect(false);clientNetControl.SocketClient.Shutdown(System.Net.Sockets.SocketShutdown.Both); clientNetControl.SocketClient.Close();
        }
    }
}
