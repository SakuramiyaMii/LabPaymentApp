using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace LabPaymentApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class Debug_DataBase : Page
    {
        public Debug_DataBase()
        {
            this.InitializeComponent();
        }

        //データベースアクセスサンプル
        private void Button_Click(object sender, RoutedEventArgs e)
        {

            /// <example> Get_AllUserInfomation
            
            DatabaseAccess dba = new DatabaseAccess();

            List<UsersInformation> users = dba.Get_AllUserInformation();

            foreach (UsersInformation ui in users)
            {
                //デバッグウインドウへの出力
                System.Diagnostics.Debug.WriteLine("{0},{1},{2},{3}",ui._mid,ui._balance,ui._user_name,ui._permission);
            }
            

            /// <example> Search_UserInfomation
            /*
            DatabaseAccess dba = new DatabaseAccess();
            string search_mid = "114514";
            bool flag = dba.Search_UserInformation(search_mid);
            System.Diagnostics.Debug.WriteLine("mid : {0} -> {1}", search_mid,  flag ? true : false);
            */

            /// <example> Insert_Information
            /*
            DatabaseAccess dba = new DatabaseAccess();
            dba.Insert_UserInformation(new UsersInformation("114514", 1, "Fuji", "3"));
            */

            /// <example> Delete_Information
            /*
            DatabaseAccess dba = new DatabaseAccess();
            dba.Delete_UserInformation("114514");
            */
        }
    }
}
