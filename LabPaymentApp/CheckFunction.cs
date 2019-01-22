using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

namespace LabPaymentApp
{
    public static class CheckFunction
    {
        /// <summary>
        /// メッセージダイアログを表示するメソッド
        /// </summary>
        /// <param name="title">タイトル</param>
        /// <param name="msg">本文</param>
        public async static void Message_Show(string title, string msg){
            var msgs = new ContentDialog();
            msgs.Title = title;
            msgs.Content = msg;
            msgs.PrimaryButtonText = "OK";
            await msgs.ShowAsync();
        }

        /// <summary>
        /// JANコードの整合性チェックメソッド
        /// 
        /// </summary>
        /// <returns>trueの場合正常,falseの場合異常を示す</returns>
        public static bool JANCODE_Integrity_Check(string jan)
        {
            try
            {
                if(Regex.IsMatch(jan, @"^[0-9]{13}$") || Regex.IsMatch(jan, @"^[0-9]{8}$"))
                {
                    return true;
                }else{
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 商品名の整合性チェックメソッド
        /// 
        /// </summary>
        /// <returns>trueの場合正常,falseの場合異常を示す</returns>
        public static bool itemName_Integrity_Check(string itemName)
        {
            try
            {
                if (itemName.Length <= 50)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 値段の整合性チェックメソッド
        /// 
        /// </summary>
        /// <returns>trueの場合正常,falseの場合異常を示す</returns>
        public static bool price_Integrity_Check(string price)
        {
            try
            {
                if ((1 <= int.Parse(price)) && (5000 >= int.Parse(price)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 値段の整合性チェックメソッド
        /// 
        /// </summary>
        /// <returns>trueの場合正常,falseの場合異常を示す</returns>
        public static bool num_Integrity_Check(string num)
        {
            try
            {
                if ((0 <= int.Parse(num)) && (200 >= int.Parse(num)))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// midの整合性チェックメソッド
        /// 
        /// </summary>
        /// <returns>trueの場合正常,falseの場合異常を示す</returns>
        public static bool mid_Integrity_Check(string mid){
            try
            {
                var isAlphaNumericOnly = Regex.IsMatch(mid, @"^[A-Z0-9]{16}$");
                return isAlphaNumericOnly;
            }catch{
                return false;
            }
        }

        /// <summary>
        /// ユーザー名の整合性チェックメソッド
        /// </summary>
        /// <returns>trueの場合正常,falseの場合異常を示す</returns>
        public static bool user_name_Integrity_Check(string name){
            if(name.Length <= 25){
                return true;
            }else{
                return false;
            }
        }

        /// <summary>
        /// 残高の整合性チェックメソッド
        /// </summary>
        /// <returns>trueの場合正常,falseの場合異常を示す</returns>
        public static bool balance_Integrity_Check(string balance){
        try{
                var isNumOnly = Regex.IsMatch(balance, @"^[0-9]{1,5}$");
                return isNumOnly;
            }
            catch{
                return false;
            }
        }

        /// <summary>
        /// ログイン中のユーザーのStaticParamをアップデートします。
        /// </summary>
        /// <returns>trueの場合正常,falseの場合異常を示す</returns>
        public static bool update_user()
        {
            try
            {
                DatabaseAccess db = new DatabaseAccess();
                UsersInformation user = db.Get_UserInformation(StaticParam._mID);
                StaticParam._balance = user._balance;
                StaticParam._userName = user._user_name;
                StaticParam._permission = user._permission;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string Get_categoryName(int num){
            switch(num){
                case 1:
                    return "水・飲料";
                case 2:
                    return "カップ麺(ラーメン)";
                case 3:
                    return "カップ麺(その他)";
                case 4:
                    return "チョコレート菓子";
                case 5:
                    return "スナック菓子";
                case 6:
                    return "駄菓子";
                case 7:
                    return "ファミリーパック菓子";
                case 8:
                    return "アイス";
                case 9:
                    return "その他";

            }
            return "未定義";
        }
    }
}
