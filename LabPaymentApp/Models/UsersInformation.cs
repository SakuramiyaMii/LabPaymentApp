using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp
{
    class UsersInformation : INotifyPropertyChanged
    {
        // 変更が起きたときに呼び出されるやつ
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            field = value;
            var h = this.PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
        }

        // カードmID
        private string mid;
        public string _mid { get { return this.mid; } set { this.SetProperty(ref this.mid, value); } }

        //残高
        private int balance;
        public int _balance{ get { return this.balance; } set { this.SetProperty(ref this.balance, value); }  }

        //ユーザ名
        private string user_name;
        public string _user_name { get { return this.user_name; } set { this.SetProperty(ref this.user_name, value); } }

        //権限レベル
        private string permission;
        public string _permission { get { return this.permission; } set { this.SetProperty(ref this.permission, value); } }

        public UsersInformation()
        {
            this.mid = "";
            this.balance = -999;
            this.user_name = "";
            this._permission = "";
        }

        public UsersInformation(string mid,int balance,string user_name,string permission)
        {
            this.mid = mid;
            this.balance = balance;
            this.user_name = user_name;
            this._permission = permission;
        }
    }
}
