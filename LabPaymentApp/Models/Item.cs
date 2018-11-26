using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp.Models
{
    /// <summary>
    /// Itemクラス
    /// </summary>
    class Item
    {
        // JANコード
        static string _janCode = "";
        // 商品名
        static string _itemName = "";
        // 価格
        static int _price = 0;
        // 個数
        static int _num = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Item()
        {
            _janCode = "";
            _itemName = "";
            _price = 0;
            _num = 0;
        }

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public void init(){
            _janCode = "";
            _itemName = "";
            _price = 0;
            _num = 0;
        }

        public void Dispose(){

        }
    }
}
