using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Model:NotificationObject
    {
        private string _wpf = "哈哈";

        public string Wpf
        {
            get
            {
                return _wpf;
            }

            set
            {
                _wpf = value;
                this.RaisePropertyChanged("Wpf");
            }
        }
        public void Copy(object obj)
        {
            this.Wpf+="哈哈";
        }
    }
}
