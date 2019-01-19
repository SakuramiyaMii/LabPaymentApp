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
    public sealed partial class ChargeScreen : Page
    {
        // 前画面からの引き継ぎ金額
        int cPrice = 0;

        public ChargeScreen()
        {
            this.InitializeComponent();
        }

        private void Charge_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            if(CheckFunction.balance_Integrity_Check(Num_Box.Text)){
                if (int.Parse(Num_Box.Text) > 10000) {
                    CheckFunction.Message_Show("Error", "一度にチャージできる金額は10000円までです。");
                    Enable_Toggle();
                    return;
                } else if ((int.Parse(Num_Box.Text) + StaticParam._balance) > 100000) {
                    CheckFunction.Message_Show("Error", "残高の最大は100000円までです。");
                    Enable_Toggle();
                    return;
                } else if (int.Parse(Num_Box.Text) == 0) {
                    CheckFunction.Message_Show("Error", "チャージ額は1円以上を指定して下さい。");
                    Enable_Toggle();
                }
                else {
                    DatabaseAccess db = new DatabaseAccess();
                    Frame.Navigate(typeof(ChargeConfirmationScreen), int.Parse(Num_Box.Text));
                }
            }else{
                CheckFunction.Message_Show("Error","チャージ額は0~10000の値で指定して下さい。");
                Enable_Toggle();
                return;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(MenuScreen));
        }

        // 別のページから遷移してきた時に呼び出されるメソッド
        // 画面描画は完了していないのでLoadedメソッドを利用？
        // 参照 : ttps://qiita.com/nagasakulllo/items/0b06ccc66b9fe0909b3f
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                cPrice = (int)e.Parameter;
            }
        }

        // テンキーここから
        // テンキーの対象とするテキストボックス名
        string targetBox = "Num_Box";
        TextBox tb = new TextBox();

        private void _0_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "0";
        }

        private void _1_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "1";
        }

        private void _2_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "2";
        }

        private void _3_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "3";
        }

        private void _4_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "4";
        }

        private void _5_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "5";
        }

        private void _6_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "6";
        }

        private void _7_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "7";
        }

        private void _8_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "8";
        }

        private void _9_Button_Click(object sender, RoutedEventArgs e)
        {
            tb = FindName(targetBox) as TextBox;
            tb.Text += "9";
        }

        private void BS_Button_Click(object sender, RoutedEventArgs e)
        {
            Num_Box.Text = Num_Box.Text.Substring(0, (Num_Box.Text.Length == 0 ? 1 : Num_Box.Text.Length) - 1);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            userName_Text.Text = StaticParam._userName + " 様";
            balance_Text.Text = StaticParam._balance + "円";
            if(cPrice > 0){
                Num_Box.Text = cPrice.ToString();
            }
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
