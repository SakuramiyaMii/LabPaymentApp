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
    public sealed partial class ChargeConfirmationScreen : Page
    {
        // 前画面からの引き継ぎ金額
        int cPrice = 0;

        public ChargeConfirmationScreen()
        {
            this.InitializeComponent();
        }

        private void Charge_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            DatabaseAccess db = new DatabaseAccess();
            db.Exec_Charge(StaticParam._mID, cPrice);
            db.Insert_Charge_Log(StaticParam._mID,cPrice);
            CheckFunction.Message_Show("チャージに成功しました。","");
            Frame.Navigate(typeof(MenuScreen));
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(ChargeScreen),cPrice);
        }

        // 別のページから遷移してきた時に呼び出されるメソッド
        // 画面描画は完了していないのでLoadedメソッドを利用？
        // 参照 : ttps://qiita.com/nagasakulllo/items/0b06ccc66b9fe0909b3f
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            cPrice = (int)e.Parameter;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            userName_Text.Text = StaticParam._userName + " 様";
            balance_Text.Text = StaticParam._balance + "円";
            charge_Text.Text = cPrice.ToString() + "円";
            after_Text.Text = (StaticParam._balance + cPrice).ToString() + "円"; 
        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Back_Button.IsEnabled == true)
            {
                Back_Button.IsEnabled = false;
                Charge_Decide_Button.IsEnabled = false;
            }
            else
            {
                Back_Button.IsEnabled = true;
                Charge_Decide_Button.IsEnabled = true;
            }
        }
    }
}
