using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp.Models
{
    class Request
    {
        // ID
        static int _id = 0;
        // 日付
        static string _date = "";
        // mID
        static string _mID = "";
        // ユーザー名
        static string _userName = "";
        // 要望文
        static string _req = "";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Request(){
            _id = 0;
            _date = "";
            _mID = "";
            _userName = "";
            _req = "";
        }

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public void init(){
            _id = 0;
            _date = "";
            _mID = "";
            _userName = "";
            _req = "";
        }
    }
}
