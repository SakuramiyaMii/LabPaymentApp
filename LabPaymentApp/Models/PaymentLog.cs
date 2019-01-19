using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp
{
    class PaymentLog : INotifyPropertyChanged
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

        // 種別
        private string type;
        public string _type{ get { return this.type; } set { this.SetProperty(ref this.type, value); }  }

        // mID
        private string mid;
        public string _mid { get { return this.mid; } set { this.SetProperty(ref this.mid, value); } }

        // ユーザ名
        private string user_name;
        public string _user_name { get { return this.user_name; } set { this.SetProperty(ref this.user_name, value); } }

        // JANコード
        private string janCode;
        public string _janCode { get { return this.janCode; } set { this.SetProperty(ref this.janCode, value); } }

        // 商品名
        private string itemName;
        public string _itemName { get { return this.itemName; } set { this.SetProperty(ref this.itemName, value); } }

        // 個数
        private int num;
        public int _num { get { return this.num; } set { this.SetProperty(ref this.num, value); } }

        // 単価
        private int price;
        public int _price { get { return this.price; } set { this.SetProperty(ref this.price, value); } }

        // 小計
        private int total_price;
        public int _total_price { get { return this.total_price; } set { this.SetProperty(ref this.total_price, value); } }

        public PaymentLog()
        {
            this.id = 0;
            this.date = "";
            this.type = "";
            this.mid = "";
            this.user_name = "";
            this.janCode = "";
            this.itemName = "";
            this.num = 0;
            this.price = 0;
            this.total_price = 0;
        }

        public PaymentLog(int id, string date,string type, string mid, string user_name, string janCode, string itemName, int num, int price, int total_price)
        {
            this.id = id;
            this.date = date;
            this.type = type;
            this.mid = mid;
            this.user_name = user_name;
            this.janCode = janCode;
            this.itemName = itemName;
            this.num = num;
            this.price = price;
            this.total_price = total_price;
        }
    }
}
