using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp.Models
{
    class User
    {
        // カードのmID
        static string _mID = "";
        // 名前
        static string _name = "";
        // 残高
        static int _balance = 0;
        // 権限レベル
        static int _permission = 0;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public User(){
            _mID = "";
            _name = "";
            _permission = 0;
            _balance = 0;
        }

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public void init(){
            _mID = "";
            _name = "";
            _permission = 0;
            _balance = 0;
        }
    }
}
