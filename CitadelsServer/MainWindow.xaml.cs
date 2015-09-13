using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CitadelsServer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = App.serverViewModel;
        }

        private void btnStartServer_Click(object sender, RoutedEventArgs e)
        {
            ServerIp sip = cmboxIP.SelectedItem as ServerIp;
            App.serverViewModel.serverNetControl = new ServerNetControl(sip.Ip.ToString(), tbxPort.Text);
            gridStartSuccess.Visibility = Visibility.Visible;
            gridStartWin.Visibility = Visibility.Collapsed;
        }
    }
}
