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
    public sealed partial class ProductPurchaseScreen : Page
    {
        public ProductPurchaseScreen()
        {
            this.InitializeComponent();
        }

        private void Purchase_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPurchaseConfirmationScreen));
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
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
        
        // テンキーここまで
    }
}
