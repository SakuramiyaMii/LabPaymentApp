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
    public sealed partial class DemandInputScreen : Page
    {
        public DemandInputScreen()
        {
            this.InitializeComponent();
        }

        private void Demand_Send_Button_Click(object sender, RoutedEventArgs e)
        {
            if(DEMAND_TEXT.Text == ""){
                CheckFunction.Message_Show("Error", "要望文を入力して下さい。");
                return;
            }

            DatabaseAccess db = new DatabaseAccess();
            if(db.Insert_Demand(StaticParam._mID,DEMAND_TEXT.Text)){
                CheckFunction.Message_Show("要望文の送信に成功しました。", "");
                Frame.Navigate(typeof(MenuScreen));
            }
            else{
                CheckFunction.Message_Show("Error","要望文の送信に失敗しました。");
                return;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
        }
    }
}
