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
    // Sorting="dg_sorting" ←ソート実装後にDatagridに追加するやつ

    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class HotSellingProductsViewScreen : Page
    {
        // データリストの定義(編集不可)(PaymentLogの使い回し)
        private ObservableCollection<PaymentLog> Data = new ObservableCollection<PaymentLog>();

        // 売上データ
        private Dictionary<string, int> sellingList = new Dictionary<string, int>();
        // 売上スコア
        private Dictionary<string, int> sellingScore = new Dictionary<string, int>();

        public HotSellingProductsViewScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetData();
        }

        // データリスト初期化メソッド
        private ObservableCollection<PaymentLog> GetData()
        {
            DatabaseAccess db = new DatabaseAccess();
            var all_Logs = db.Get_AllPayment();


            // 売上商品の集計
            foreach (var buf in all_Logs)
            {
                if (buf._type == "購入")
                {
                    if (sellingList.ContainsKey(buf._janCode))
                    { // 既にリストに存在する場合、個数を加算
                        sellingList[buf._janCode] += buf._num;
                        // 件数の記録
                        sellingScore[buf._janCode]++;
                    }
                    else
                    {
                        sellingList.Add(buf._janCode, buf._num); // リストに含まれない場合、新規追加
                        sellingScore.Add(buf._janCode, 1);
                    }
                }
            }

            // 売れ筋評価
            foreach (var item in sellingList)
            {
                // 売上日時リスト
                DateTime[] sellingDate = db.Get_AllSellingDate(item.Key);
                // 売上個数リスト
                int[] sellingHistory = new int[0];
                // 仕入れ日時リスト
                DateTime[] stockingDate = db.Get_AllStockingDate(item.Key);
                // 仕入れインデックス
                int stockingidx = 0;
                // itemの売上件数
                int total_record = sellingScore[item.Key];
                // 初期化(ここまでは売上件数の保持につかっていたが、ここからはスコアを保存する)
                sellingScore[item.Key] = 0;
                // 仕入れ個数履歴
                int[] history = db.Get_AllStockingHistory(item.Key);
                // 仕入れ履歴インデックス
                int historyidx = 0,total_sell = 0;


                // 売上商品の集計
                foreach (var buf in all_Logs)
                {
                    if (buf._type == "購入")
                    {
                        if (item.Key == buf._janCode)
                        { // リストの追加
                            Array.Resize(ref sellingHistory, sellingHistory.Length + 1);
                            sellingHistory[sellingHistory.Length - 1] = buf._num;
                        }
                        else
                        {
                            
                        }
                    }
                }

                // スコア計算
                for (int i = 0; i < sellingDate.Length; i++)
                {
                    total_sell += sellingHistory[i];
                    for (; ; )
                    {
                        if (stockingidx == stockingDate.Length - 1)
                        {
                            // 現在参照している仕入れ日時が最終仕入れの場合
                            break;
                        }
                        // 現在参照している仕入れ日時よりも後に仕入れがあった場合
                        if (stockingDate[stockingidx + 1] > sellingDate[i])
                        {
                            // 現在参照している仕入れ日時と後の仕入れ日時の間の売上日時の場合
                            break;
                        }
                        else
                        {
                            // 現在参照している仕入れ日時の後の仕入れ日時以降の売上日時の場合

                            for (; ; )
                            {
                                if (history[historyidx] - total_sell >= 0)
                                {
                                    break;
                                }
                                else
                                {
                                    
                                }
                                if (total_sell >= history[historyidx])
                                {
                                    total_sell = total_sell - history[historyidx];
                                    historyidx++;
                                    stockingidx++;
                                    continue;
                                }
                            }
                            break;
                        }
                    }
                    // 売上日時と、直前の仕入れ日時の差を分換算にてスコア算出(大きいほど売れていない)
                    TimeSpan ts = new TimeSpan();
                    if (i == 0)
                    {
                        // 初売上の場合
                        ts = sellingDate[i] - stockingDate[stockingidx];
                    }else if(sellingDate[i - 1] > stockingDate[stockingidx])
                    {
                        // 直前のデータが売上日時の場合
                        ts = sellingDate[i] - sellingDate[i-1];
                    }
                    else
                    {
                        // 直前のデータが仕入れ日時の場合
                        ts = sellingDate[i] - stockingDate[stockingidx];
                    }
                    sellingScore[item.Key] += (int)ts.TotalMinutes;
                }

                sellingScore[item.Key] /= item.Value;
                // (売上件数/売上個数)のレートでスコアを減らす手法（一度に多く買っている場合の考慮）
                float r = ((float)total_record / (float)item.Value);
                sellingScore[item.Key] = (int)(sellingScore[item.Key] * r);

            }


            foreach (var log in sellingScore)
            {
                PaymentLog pl = new PaymentLog();
                
                if (log.Key != "")
                {
                    Item item = db.Get_Item(log.Key);
                    pl._price = log.Value;
                    pl._type = CheckFunction.Get_categoryName(item._categoryId);
                    pl._itemName = item._itemName;
                    pl._num = item._num;
                }
                Data.Add(pl);
            }
            List<Item> itemlist = db.Get_AllItem();
            foreach(var item in itemlist){
                if(!sellingList.ContainsKey(item._janCode)){
                    PaymentLog pl = new PaymentLog();
                    pl._price = 99999;
                    pl._type = CheckFunction.Get_categoryName(item._categoryId);
                    pl._itemName = item._itemName;
                    pl._num = item._num;
                    Data.Add(pl);
                }
            }
            Data = new ObservableCollection<PaymentLog>(from i in Data orderby i._price ascending select i);
            return Data;
        }

        // リストに対してキーワードフィルターを適用するメソッド
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            // OR検索用
            //String[] words = Keyword_Box.Text.Split(" ");

            dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Data where i._mid.Contains(Keyword_Box.Text) | i._janCode.Contains(Keyword_Box.Text) | i._itemName.Contains(Keyword_Box.Text) | i._user_name.Contains(Keyword_Box.Text) | i._date.Contains(Keyword_Box.Text) | i._type.Contains(Keyword_Box.Text) | i._num.ToString().Contains(Keyword_Box.Text) | i._price.ToString().Contains(Keyword_Box.Text) | i._total_price.ToString().Contains(Keyword_Box.Text) orderby i._date descending select i);

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

                dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Data where i._mid.Contains(Keyword_Box.Text) | i._janCode.Contains(Keyword_Box.Text) | i._itemName.Contains(Keyword_Box.Text) | i._user_name.Contains(Keyword_Box.Text) | i._date.Contains(Keyword_Box.Text) | i._type.Contains(Keyword_Box.Text) | i._num.ToString().Contains(Keyword_Box.Text) | i._price.ToString().Contains(Keyword_Box.Text) | i._total_price.ToString().Contains(Keyword_Box.Text) orderby i._date descending select i);

                // OR検索用
                // dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Logs where words.Any(i._mid.Contains) | words.Any(i._janCode.Contains) | words.Any(i._itemName.Contains) | words.Any(i._user_name.Contains) | words.Any(i._date.Contains) | words.Any(i._type.Contains) | words.Any(i._num.ToString().Contains) | words.Any(i._price.ToString().Contains) | words.Any(i._total_price.ToString().Contains) orderby i._date descending select i);

            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
        }



        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void Category_TEXT_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            if (cb.SelectedIndex == 0) return;
            dataGrid.ItemsSource = new ObservableCollection<PaymentLog>(from i in Data where  i._type.Contains(CheckFunction.Get_categoryName(cb.SelectedIndex))  orderby i._price ascending select i);
        }

    }
}
