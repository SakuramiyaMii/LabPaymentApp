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
    public sealed partial class ProductRegistrationScreen : Page
    {
        // 前画面からのアイテムリスト(編集不可)
        private ObservableCollection<Item> cItems = new ObservableCollection<Item>();
        // アイテムリストの定義(編集可能)
        private ObservableCollection<Item> Items = new ObservableCollection<Item>();
        // カテゴリリストの定義(編集不可)
        private List<Category> categoryList = Categories.categoryList;


        public ProductRegistrationScreen()
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
            if (e.Parameter != null) { 
               cItems = e.Parameter as ObservableCollection<Item>;
            }
        }

        private void Registration_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            if (Items.Count <= 0)
            {
                CheckFunction.Message_Show("Error", "商品が入力されていません");
                return;
            }
            // Items整合性チェック
            foreach (Item item in Items){
                // 入力パラメータチェック
                if (item._janCode == "")
                {
                    // JANコードが未入力です。
                    CheckFunction.Message_Show("Error", item._janCode + "\nJANコードが未入力です。");
                    return;
                }
                else if (item._itemName == "")
                {
                    // 商品名が未入力です。
                    CheckFunction.Message_Show("Error", item._janCode + "\n商品名が未入力です。");
                    return;
                }
                else if (item._price.ToString() == "")
                {
                    // 価格が未入力です。
                    CheckFunction.Message_Show("Error", item._janCode + "\n価格が未入力です。");
                    return;
                }
                else if (item._num.ToString() == "")
                {
                    // 在庫が未選択です。
                    CheckFunction.Message_Show("Error", item._janCode + "\n在庫が未入力です。");
                    return;
                }
                else if (item._categoryId == 0)
                {
                    // カテゴリが未選択です。
                    CheckFunction.Message_Show("Error", item._janCode + "\nカテゴリが未選択です。");
                    return;

                }

                // フォーマットチェック
                if (!CheckFunction.JANCODE_Integrity_Check(item._janCode))
                {
                    CheckFunction.Message_Show("Error", item._janCode + "\nJANコードのフォーマットが間違っています。なんでこのエラー出たの？");
                    return;
                }
                else if (!CheckFunction.itemName_Integrity_Check(item._itemName))
                {
                    CheckFunction.Message_Show("Error", item._janCode + "\n登録できる商品名は５０文字以下です。");
                    return;
                }
                else if (!CheckFunction.price_Integrity_Check(item._price.ToString()))
                {
                    CheckFunction.Message_Show("Error", item._janCode + "\n登録できる価格は0～5000の値です。");
                    return;
                }
                else if (!CheckFunction.num_Integrity_Check(item._num.ToString()))
                {
                    CheckFunction.Message_Show("Error", item._janCode + "\n登録できる在庫数は0～200の値です。");
                    return;
                }
            }
            Frame.Navigate(typeof(ProductRegistrationConfirmationScreen), Items);
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
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
            }

        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            dg.SelectedItem = null;
        }

        // JANCODE_BOXエンター押下処理
        string last_jan = "1";
        private async void JANCODE_TEXT_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                JANCODE_TEXT.IsReadOnly = true;
                if(!CheckFunction.JANCODE_Integrity_Check(JANCODE_TEXT.Text)){
                    JANCODE_TEXT.IsReadOnly = false;
                    CheckFunction.Message_Show("Error", "JANコードが正しくありません");
                    JANCODE_TEXT.Text = "";
                    return;
                }

                try
                {
                    string s = await RakutenSearchAPI.JAN_Search(JANCODE_TEXT.Text);
                    WORD_BOX.Text = s;
                    Candidate_Set(s);
                }
                catch{
                    CheckFunction.Message_Show("検索APIエラー","次の要因が考えられます。\n・インターネットに接続していない\n・短時間で複数回入力を行った\n・JANコードが入力されていない");
                }
                finally{
                   
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
                    }else{
                        DatabaseAccess db = new DatabaseAccess();
                        if(db.Search_Item(JANCODE_TEXT.Text)){
                            // DB既登録の場合
                            Item item = db.Get_Item(JANCODE_TEXT.Text);
                            item._num = 1;
                            last_jan = JANCODE_TEXT.Text;
                            Items.Add(item);
                            
                        }
                        else{
                            // DB未登録の場合
                            Item item = new Item(JANCODE_TEXT.Text, "", 0, 0, 1);
                            last_jan = JANCODE_TEXT.Text;
                            Items.Add(item);
                        }
                    }
                }catch{
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
                        Item item = new Item(JANCODE_TEXT.Text, "", 0, 0, 1);
                        last_jan = JANCODE_TEXT.Text;
                        Items.Add(item);
                    }
                }

                //DatabaseAccess db = new DatabaseAccess();
                //db.Insert_Item(item);

                // 処理完了後
                JANCODE_TEXT.Text = "";
                JANCODE_TEXT.IsReadOnly = false;
            }
        }

        // 候補ワードをボタンにセット
        private void Candidate_Set(string str)
        {
            int count = 0;
            for (int i = 1; i < 11; i++)
            {
                Button b = (Button)FindName("Candidate_" + (i.ToString()));
                b.Content = "候補" + i.ToString();
            }
            string[] strSet = str.Split('\n');
            foreach (string sub in strSet)
            {
                count++;
                Button b = (Button)FindName("Candidate_" + (count.ToString()));
                if (b != null)
                {
                    if (sub != null)
                    {
                        b.Content = sub;
                    }
                    else
                    {
                        b.Content = "該当無し";
                    }
                }
            }
        }

        private void Candidate_1_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }
        private void Candidate_2_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_3_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_4_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_5_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_6_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_7_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_8_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_9_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_10_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            ADD_itemName(bt.Content.ToString());
        }

        private void Candidate_Clear_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Items.First(x => x._janCode == last_jan)._itemName = "";
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }
        }

        // 最後に登録したJANコードレコードの商品名に文字列を加えるメソッド
        private void ADD_itemName(string itemName){
            try
            {
                if (Items.First(x => x._janCode == last_jan)._itemName == "")
                {
                    Items.First(x => x._janCode == last_jan)._itemName += itemName;
                }
                else
                {
                    Items.First(x => x._janCode == last_jan)._itemName += " " + itemName;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Item it in cItems)
            {
                Items.Add(it);
            }
        }
    }
}
