using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp
{
    // 各種パラメータを保持する為のクラス
    // (こんなもん作っていいのか...？)
    class StaticParam
    {
        // ログイン者のカードmID
        public static string _mID = "";
        // ログイン者のユーザー名
        public static string _userName = "";
        // ログイン者の残高
        public static int _balance = 0;
        // ログイン者の権限レベル
        public static int _permission = 0;

        // 商品登録時の仕入れ値
        public static int _siirePrice = 0;

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public static void init(){
            _mID = "";
            _userName = "";
            _balance = 0;
            _permission = 0;
        }    
    }
}
