using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace LabPaymentApp
{
    /// <summary>
    /// Itemクラス
    /// </summary>
    class Item : INotifyPropertyChanged
    {
        // 変更が起きたときに呼び出されるやつ
        public event PropertyChangedEventHandler PropertyChanged;
        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            field = value;
            var h = this.PropertyChanged;
            if (h != null) { h(this, new PropertyChangedEventArgs(propertyName)); }
        }
        // 変更が起きたときに呼び出されるやつおわり

        // JANコード
        private string janCode;
        public string _janCode
        {
            get { return this.janCode; }
            set { this.SetProperty(ref this.janCode, value); }
        }

        // 商品名
        private string itemName;
        public string _itemName
        {
            get { return this.itemName; }
            set { this.SetProperty(ref this.itemName, value); }
        }
    
        // カテゴリID
        private int categoryId;
        public int _categoryId
        {
            get { return this.categoryId; }
            set { this.SetProperty(ref this.categoryId, value); }
        }

        // 価格
        private int price;
        public int _price
        {
            get { return this.price; }
            set { this.SetProperty(ref this.price, value); }
        }

        // 個数
        private int num;
        public int _num
        {
            get { return this.num; }
            set { this.SetProperty(ref this.num, value); }
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Item()
        {
            this.janCode = "";
            this.itemName = "";
            this.categoryId = 0;
            this.price = 0;
            this.num = 0;
        }

        public Item(string janCode,string itemName,int categoryId,int price,int num)
        {
            this.janCode = janCode;
            this.itemName = itemName;
            this.categoryId = categoryId;
            this.price = price;
            this.num = num;
        }
    }
}
