using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp
{
    class DemandsInformation : INotifyPropertyChanged
    {
        // 変更が起きたときに呼び出されるやつ
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            field = value;
            var h = this.PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
        }

        // 要望ID
        private int demand_id;
        public int _demand_id { get { return this.demand_id; } set { this.SetProperty(ref this.demand_id, value); } }

        //残高
        private string created_at;
        public string _created_at{ get { return this.created_at; } set { this.SetProperty(ref this.created_at, value); }  }

        //ユーザ名
        private string mid;
        public string _mid { get { return this.mid; } set { this.SetProperty(ref this.mid, value); } }

        //権限レベル
        private string demand_detail;
        public string _demand_detail { get { return this.demand_detail; } set { this.SetProperty(ref this.demand_detail, value); } }

        public DemandsInformation()
        {
            this.demand_id = -1;
            this.created_at = "";
            this.mid = "";
            this.demand_detail = "";
        }

        public DemandsInformation(int demand_id,string created_at, string mid, string demand_detail)
        {
            this.demand_id = demand_id;
            this.created_at = created_at;
            this.mid = mid;
            this.demand_detail = demand_detail;
        }
    }
}
