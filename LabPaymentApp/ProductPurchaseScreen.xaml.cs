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
    public sealed partial class ProductPurchaseScreen : Page
    {
        // 前画面からのアイテムリスト(編集不可)
        private ObservableCollection<Item> cItems = new ObservableCollection<Item>();
        // アイテムリストの定義(編集可能)
        private ObservableCollection<Item> Items = new ObservableCollection<Item>();
        // カテゴリリストの定義(編集不可)
        private List<Category> categoryList = Categories.categoryList;


        public ProductPurchaseScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetItem();
        }

        // リストコレクション初期設定メソッド
        private ObservableCollection<Item> GetItem()
        {
            return Items;
        }

        // 別のページから遷移してきた時に呼び出されるメソッド
        // 画面描画は完了していないのでLoadedメソッドを利用？
        // 参照 : ttps://qiita.com/nagasakulllo/items/0b06ccc66b9fe0909b3f
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                cItems = e.Parameter as ObservableCollection<Item>;
            }
        }

        private void Purchase_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            if (Items.Count <= 0){
                CheckFunction.Message_Show("Error","商品が入力されていません");
                Enable_Toggle();
                return;
            }
            DatabaseAccess db = new DatabaseAccess();
            foreach(Item checkItem in Items){
                if(checkItem._num <= 0){
                    CheckFunction.Message_Show("Error", checkItem._itemName + "の購入数が0以下になっています。");
                    Enable_Toggle();
                    return;
                }

                if(db.isStocked_Item(checkItem._janCode,checkItem._num)){

                }else{
                    CheckFunction.Message_Show("Error",checkItem._itemName + "の在庫が不足しています。");
                    Enable_Toggle();
                    return;
                }
            }

            Frame.Navigate(typeof(ProductPurchaseConfirmationScreen),Items);
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(MenuScreen));
        }

        // 各レコードの削除ボタンで読み込まれるメソッド
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // イベントを発生させたボタンの取得
            Button b = (Button)sender;
            // DataGridの取得
            DataGrid dg = FindName("dataGrid") as DataGrid;
            try
            {
                // Itemsのうち、削除ボタンのtagに付与したJANコードと一致するレコードを検索し、Itemsから削除する
                Items.Remove(Items.First(x => x._janCode == (string)b.Tag));
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }finally{
                JANCODE_TEXT.Focus(FocusState.Keyboard);
            }

        }


        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem == null) return;
            Item item = dg.SelectedItem as Item;
            last_jan = item._janCode;
            dg.SelectedItem = null;
            JANCODE_TEXT.Focus(FocusState.Keyboard);
        }

        // JANCODE_BOXエンター押下処理
        string last_jan = "1";
        private void JANCODE_TEXT_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                JANCODE_TEXT.IsReadOnly = true;
                if (!CheckFunction.JANCODE_Integrity_Check(JANCODE_TEXT.Text))
                {
                    JANCODE_TEXT.IsReadOnly = false;
                    CheckFunction.Message_Show("Error", "JANコードが正しくありません");
                    JANCODE_TEXT.Text = "";
                    return;
                }

                // 謎ポイント
                // 初回、if (Items.First(x => x._janCode == last_jan)._janCode == JANCODE_TEXT.Text)でマッチするレコードがなかった場合例外発生、２回目以降は例外は発生せずelseに飛ぶ
                // 応急措置として同じコードを書いています
                // いろいろおかしい
                try
                {
                    // リスト上に存在する場合
                    if (Items.First(x => x._janCode == JANCODE_TEXT.Text)._janCode == JANCODE_TEXT.Text)
                    {
                        last_jan = JANCODE_TEXT.Text;
                        Items.First(x => x._janCode == JANCODE_TEXT.Text)._num += 1;
                    }
                    else
                    {
                        // リスト上に存在しない場合
                        DatabaseAccess db = new DatabaseAccess();
                        if (db.Search_Item(JANCODE_TEXT.Text))
                        {
                            // DB既登録の場合
                            Item item = db.Get_Item(JANCODE_TEXT.Text);
                            item._num = 1;
                            last_jan = JANCODE_TEXT.Text;
                            Items.Add(item);

                        }
                        else
                        {
                            // DB未登録の場合
                            CheckFunction.Message_Show("Error","データベースに存在しない商品です。");
                        }
                    }
                }
                catch
                {
                    // リスト上に存在しない場合
                    DatabaseAccess db = new DatabaseAccess();
                    if (db.Search_Item(JANCODE_TEXT.Text))
                    {
                        // DB既登録の場合
                        Item item = db.Get_Item(JANCODE_TEXT.Text);
                        item._num = 1;
                        last_jan = JANCODE_TEXT.Text;
                        Items.Add(item);

                    }
                    else
                    {
                        // DB未登録の場合
                        CheckFunction.Message_Show("Error", "データベースに存在しない商品です。");
                    }
                }


                // 処理完了後
                JANCODE_TEXT.Text = "";
                JANCODE_TEXT.IsReadOnly = false;
            }
        }

        // テンキーここから
        // テンキーの対象とするテキストボックス名
        TextBox tb = new TextBox();

        private void _0_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("0");
        }

        private void _1_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("1");
        }

        private void _2_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("2");
        }

        private void _3_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("3");
        }

        private void _4_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("4");
        }

        private void _5_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("5");
        }

        private void _6_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("6");
        }

        private void _7_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("7");
        }

        private void _8_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("8");
        }

        private void _9_Button_Click(object sender, RoutedEventArgs e)
        {
            ADD_Num("9");
        }

        private void BS_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Items.First(x => x._janCode == last_jan)._num.ToString().Length > 1)
                {
                    Items.First(x => x._janCode == last_jan)._num = int.Parse(Items.First(x => x._janCode == last_jan)._num.ToString().Substring(0, Items.First(x => x._janCode == last_jan)._num.ToString().Length - 1));
                }else{
                    Items.First(x => x._janCode == last_jan)._num = 0;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }finally{
                JANCODE_TEXT.Focus(FocusState.Keyboard);
            }
        }

        // 最後に登録したJANコードレコードの商品名に文字列を加えるメソッド
        private void ADD_Num(string num)
        {
            try
            {
                Items.First(x => x._janCode == last_jan)._num = int.Parse(Items.First(x => x._janCode == last_jan)._num.ToString() == "0"? num.ToString() : Items.First(x => x._janCode == last_jan)._num.ToString() + num.ToString());
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }finally{
                JANCODE_TEXT.Focus(FocusState.Keyboard);
            }
        }

        // テンキーここまで

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Item it in cItems)
            {
                Items.Add(it);
            }
            JANCODE_TEXT.Focus(FocusState.Keyboard);
        }

        private void DataGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            JANCODE_TEXT.Focus(FocusState.Keyboard);
        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Back_Button.IsEnabled == true)
            {
                Back_Button.IsEnabled = false;
                Purchase_Decide_Button.IsEnabled = false;
            }
            else
            {
                Back_Button.IsEnabled = true;
                Purchase_Decide_Button.IsEnabled = true;
            }
        }
    }
}
