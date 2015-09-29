using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CitadelsClient.Model
{
    public class RoomInfo:NotificationObject
    {
        int _id;

        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                _id = value;
                RaisePropertyChanged("Id");
            }
        }

        public ObservableCollection<string> NickNameList { set; get; }

        int _num;
        public int Num
        {
            get
            {
                return NickNameList.Count;
            }

            set
            {
                _num = NickNameList.Count;
                RaisePropertyChanged("Num");
            }
        }

        public RoomInfo()
        {
            NickNameList = new ObservableCollection<string>();
        }
    }
}
