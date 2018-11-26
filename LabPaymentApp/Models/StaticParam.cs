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
        // カードmID
        public static string _mID = "";
        // ユーザー名
        public static string _userName = "";
        // 残高
        public static int _balance = 0;
        // 権限レベル
        public static int _permission = 0;

        /// <summary>
        /// 初期化メソッド
        /// </summary>
        public void init(){
            _mID = "";
            _userName = "";
            _balance = 0;
            _permission = 0;
        }    
        
        
    }
}
