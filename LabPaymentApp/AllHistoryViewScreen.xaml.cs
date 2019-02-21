using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class AllHistoryViewScreen : Page
    {
        // ログリストの定義(編集不可)
        private ObservableCollection<PaymentLog> Logs = new ObservableCollection<PaymentLog>();
        public AllHistoryViewScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetLogs();
        }

        // ログリスト初期化メソッド
        private ObservableCollection<PaymentLog> GetLogs()
        {
            DatabaseAccess db = new DatabaseAccess();
            var all_Logs = db.Get_AllPayment();
            foreach (var log in all_Logs)
            {
                Logs.Add(log);
            }
            Logs = new ObservableCollection<PaymentLog>(from i in Logs orderby i._date descending select i);
            return Logs;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(MenuScreen));
        }

        // 選択行変更時にハイライトを表示しないようにする→削除実行時にハイライトがどっか行って不自然なので
        // 選択行のハイライトは消せたが選択項目の消し方がわからなかった
        // DataGrid.CurrentColumnIndexがそれっぽいがprivateなのでいじれなかった(アクセサもなかったっぽい)
        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            dg.SelectedItem = null;
        }

        // ソート
        // 直前に選択されていたcollumの保存変数
        DataGridColumn c = null;
        private void dg_sorting(object sender, DataGridColumnEventArgs e)
        {
            if (e.Column.Tag.ToString() == null) return;
            // 直前に選ばれていたcollumが別ならアイコンを消しておく
            if (c != e.Column && c != null) c.SortDirection = null;

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Ascending)
            {
                // 選択collumが未選択・昇順状態であれば、降順にする
                e.Column.SortDirection = DataGridSortDirection.Descending;

                //選択Collumによって分岐
                if (e.Column.Tag.ToString() == "Date")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._date descending select i);
                }
                else if (e.Column.Tag.ToString() == "Type")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._type descending select i);
                }
                else if (e.Column.Tag.ToString() == "User")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._user_name descending select i);
                }
                else if (e.Column.Tag.ToString() == "Item")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._itemName descending select i);
                }
                else if (e.Column.Tag.ToString() == "Num")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._num descending select i);
                }
                else if (e.Column.Tag.ToString() == "Price")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._price descending select i);
                }
                else if (e.Column.Tag.ToString() == "Total")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._total_price descending select i);
                }
            }
            else
            {
                // 選択collumが降順状態であれば、昇順にする
                e.Column.SortDirection = DataGridSortDirection.Ascending;

                // 選択collumによって分岐
                if (e.Column.Tag.ToString() == "Date")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._date ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Type")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._type ascending select i);
                }
                else if (e.Column.Tag.ToString() == "User")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._user_name ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Item")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._itemName ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Num")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._num ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Price")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._price ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Total")
                {
                    dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs orderby i._total_price ascending select i);
                }
            }
            // 現在選ばれていたcollumの保持
            c = e.Column;
        }

        // リストに対してキーワードフィルターを適用するメソッド
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            // OR検索用
            //String[] words = Keyword_Box.Text.Split(" ");
            
            dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs where i._mid.Contains(Keyword_Box.Text) | i._janCode.Contains(Keyword_Box.Text) | i._itemName.Contains(Keyword_Box.Text) | i._user_name.Contains(Keyword_Box.Text) | i._date.Contains(Keyword_Box.Text) | i._type.Contains(Keyword_Box.Text) | i._num.ToString().Contains(Keyword_Box.Text) | i._price.ToString().Contains(Keyword_Box.Text) | i._total_price.ToString().Contains(Keyword_Box.Text) orderby i._date descending select i);

            // OR検索用
            //dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs where words.Any(i._mid.Contains) | words.Any(i._janCode.Contains) | words.Any(i._itemName.Contains) | words.Any(i._user_name.Contains) | words.Any(i._date.Contains) | words.Any(i._type.Contains) | words.Any(i._num.ToString().Contains) | words.Any(i._price.ToString().Contains) | words.Any(i._total_price.ToString().Contains) orderby i._date descending select i);
            // AND検索を実装するなら、ObservableCollection<PaymentLog>のローカル変数にLogsをコピーして、ワード数分だけ絞り込み、最後にLogsに反映で実装できる？（自信ない
        }

        private void Keyword_Box_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                // OR検索用
                // String[] words = Keyword_Box.Text.Split(" ");
                
                dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs where i._mid.Contains(Keyword_Box.Text) | i._janCode.Contains(Keyword_Box.Text) | i._itemName.Contains(Keyword_Box.Text) | i._user_name.Contains(Keyword_Box.Text) | i._date.Contains(Keyword_Box.Text) | i._type.Contains(Keyword_Box.Text) | i._num.ToString().Contains(Keyword_Box.Text) | i._price.ToString().Contains(Keyword_Box.Text) | i._total_price.ToString().Contains(Keyword_Box.Text) orderby i._date descending select i);
                
                // OR検索用
                // dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs where words.Any(i._mid.Contains) | words.Any(i._janCode.Contains) | words.Any(i._itemName.Contains) | words.Any(i._user_name.Contains) | words.Any(i._date.Contains) | words.Any(i._type.Contains) | words.Any(i._num.ToString().Contains) | words.Any(i._price.ToString().Contains) | words.Any(i._total_price.ToString().Contains) orderby i._date descending select i);

            }
        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Back_Button.IsEnabled == true)
            {
                Set_Button.IsEnabled = false;
                Back_Button.IsEnabled = false;
            }
            else
            {
                Set_Button.IsEnabled = true;
                Back_Button.IsEnabled = true;
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DatabaseAccess db = new DatabaseAccess();
            Balance_TEXT.Text = db.Get_Balance().ToString();
            int All_Balance = int.Parse(Balance_TEXT.Text);
            foreach(PaymentLog pl in Logs){
                if(pl._type == "チャージ"){
                    All_Balance += pl._total_price;
                }else if(pl._type == "仕入れ"){
                    All_Balance += pl._total_price;
                    // チャージログに仕入れ額を負値で保存しているため、表示用に反転している
                    pl._total_price *= -1;
                }
            }
            All_Balance_TEXT.Text = All_Balance.ToString() + "円";
        }

        private void Set_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            DatabaseAccess db = new DatabaseAccess();
            int balance = 0;
            if(!int.TryParse(Balance_TEXT.Text,out balance)){
                CheckFunction.Message_Show("Error","設定額が不正です。");
            }else{
                if (balance >= 0 && balance <= 500000)
                {
                    db.Insert_Operation_Log(StaticParam._mID, "初期金額更新(" + db.Get_Balance().ToString() + "円 → " + balance.ToString() + "円)");
                    db.Set_Balance(balance);
                    
                    int All_Balance = balance;
                    foreach (PaymentLog pl in Logs)
                    {
                        if (pl._type == "チャージ")
                        {
                            All_Balance += pl._total_price;
                        }
                        else if (pl._type == "仕入れ")
                        {
                            All_Balance += pl._total_price;
                        }
                    }
                    All_Balance_TEXT.Text = All_Balance.ToString() + "円";
                    CheckFunction.Message_Show("初期金額を更新しました。", "");
                }else{
                    CheckFunction.Message_Show("Error", "設定額は0円以上500000円以下です。");
                }
            }
            Enable_Toggle();
            return;
        }
    }
}
