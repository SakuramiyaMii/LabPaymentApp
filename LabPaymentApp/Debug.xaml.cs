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

using System.Threading.Tasks;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace LabPaymentApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class Debug : Page
    {
        public Debug()
        {
            this.InitializeComponent();
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AuthenticationScreen));
        }

        private async void testbutton_Click(object sender, RoutedEventArgs e)
        {
            string s = await RakutenSearchAPI.JAN_Search(Jancode_Box.Text);
            TestBox.AcceptsReturn = true;
            TestBox.Text = s;
            Candidate_Set(s);
        }

        // 候補ワードをボタンにセット
        private void Candidate_Set(string str){
            int count = 0;
            for(int i = 1; i < 11; i++ ){
                Button b = (Button)FindName("Candidate_" + (i.ToString()));
                b.Content = "候補" + i.ToString();
            }
            string[] strSet = str.Split('\n');
            foreach(string sub in strSet){
                count++;
                Button b = (Button)FindName("Candidate_" + (count.ToString()));
                if(b != null){
                    if (sub != null){
                        b.Content = sub;
                    }else{
                        b.Content = "該当無し";
                    }
                }
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
            tb.Text = tb.Text.Substring(0, (tb.Text.Length == 0 ? 1 : tb.Text.Length) - 1);
        }

        // テンキーここまで

        // コンボボックスの内容が更新された時に呼び出されるメソッド
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender != null)
            {
                ComboBox cb = (ComboBox)sender;
                // コンボボックスのうち選ばれた項目名を取得
                Combo_Item.Text = ((ComboBoxItem)cb.SelectedItem).Content as string;
                
            }
        }

        string genTb = "Gen_Text";
        TextBox gtb = new TextBox();

        private void Candidate_1_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if(gtb.Text == ""){
                gtb.Text += b.Content;
            }else{
                gtb.Text += " "+ b.Content;
            }
        }

        private void Candidate_2_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == ""){
                gtb.Text += b.Content;
            }
            else{
                gtb.Text += " " + b.Content;
            }
        }

        private void Candidate_3_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == ""){
                gtb.Text += b.Content;
            }
            else{
                gtb.Text += " " + b.Content;
            }
        }

        private void Candidate_4_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == "")
            {
                gtb.Text += b.Content;
            }
            else
            {
                gtb.Text += " " + b.Content;
            }
        }

        private void Candidate_5_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == "")
            {
                gtb.Text += b.Content;
            }
            else
            {
                gtb.Text += " " + b.Content;
            }
        }

        private void Candidate_6_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == "")
            {
                gtb.Text += b.Content;
            }
            else
            {
                gtb.Text += " " + b.Content;
            }
        }

        private void Candidate_7_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == "")
            {
                gtb.Text += b.Content;
            }
            else
            {
                gtb.Text += " " + b.Content;
            }
        }

        private void Candidate_8_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == "")
            {
                gtb.Text += b.Content;
            }
            else
            {
                gtb.Text += " " + b.Content;
            }
        }

        private void Candidate_9_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == "")
            {
                gtb.Text += b.Content;
            }
            else
            {
                gtb.Text += " " + b.Content;
            }
        }

        private void Candidate_10_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            Button b = (Button)sender;
            if (gtb.Text == "")
            {
                gtb.Text += b.Content;
            }
            else
            {
                gtb.Text += " " + b.Content;
            }
        }

        private void Clear_Button_Click(object sender, RoutedEventArgs e)
        {
            gtb = (TextBox)FindName(genTb);
            gtb.Text = "";

        }

        private void Setting_Button_Click(object sender, RoutedEventArgs e)
        {
            Item_Name.Text = Gen_Text.Text;
            Category_Name.Text = Combo_Item.Text;
            Num_Name.Text = Num_Box.Text;
        }

        private void Jancode_Box_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.Enter){
                exec();
            }
        }

        private async void exec()
        {
            string s = await RakutenSearchAPI.JAN_Search(Jancode_Box.Text);
            TestBox.AcceptsReturn = true;
            TestBox.Text = s;
            Candidate_Set(s);
        }

        // コンボボックスの内容が更新された際に呼び出されるメソッド

    }
}
