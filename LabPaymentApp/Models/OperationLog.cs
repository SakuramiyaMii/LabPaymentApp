using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp
{
    class OperationLog : INotifyPropertyChanged
    {
        // 変更が起きたときに呼び出されるやつ
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            field = value;
            var h = this.PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
        }
        // ID
        private int id;
        public int _id { get { return this.id; } set { this.SetProperty(ref this.id, value); } }


        // 日時
        private string date;
        public string _date { get { return this.date; } set { this.SetProperty(ref this.date, value); } }

        // mID
        private string mid;
        public string _mid { get { return this.mid; } set { this.SetProperty(ref this.mid, value); } }

        // ユーザ名
        private string user_name;
        public string _user_name { get { return this.user_name; } set { this.SetProperty(ref this.user_name, value); } }

        // JANコード
        private string detail;
        public string _detail { get { return this.detail; } set { this.SetProperty(ref this.detail, value); } }

        public OperationLog()
        {
            this.id = 0;
            this.date = "";
            this.mid = "";
            this.user_name = "";
            this.detail = "";
        }

        public OperationLog(int id,string date,string type, string mid, string user_name, string janCode, string itemName, int num, int price, int total_price)
        {
            this.id = id;
            this.date = date;
            this.mid = mid;
            this.user_name = user_name;
            this.detail = janCode;
        }
    }
}
