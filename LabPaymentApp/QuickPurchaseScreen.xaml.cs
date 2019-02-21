using Felica;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;
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
    public sealed partial class QuickPurchaseScreen : Page
    {
        // アイテムリストの定義(編集可能)
        private ObservableCollection<Item> Items = new ObservableCollection<Item>();
        // カテゴリリストの定義(編集不可)
        private List<Category> categoryList = Categories.categoryList;
        // タイマー変数
        private DispatcherTimer _timer;

        // 小計金額
        int total_price = 0;


      public QuickPurchaseScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetItem();
        }

        // タイマー用メソッド
        private async void Check_Card(object sender, object e)
        {
            string mID = "";
            // タイマーの停止
            this._timer.Stop();

            // カードを離した際のGetmid()で例外が走るみたいなので応急措置です
            try
            {
                mID = await Getmid();
            }catch{
                this._timer.Start();
                return;
            }

            try
            {
                if (mID != "")
                {
                    // mIDが登録されていた場合
                    // mIDが登録されているかのチェック
                    DatabaseAccess db = new DatabaseAccess();
                    if (db.Search_UserInformation(mID))
                    {
                        // 事前処理
                        // 決済額のリセット
                        total_price = 0;

                        // カード情報表示
                        UsersInformation uis = db.Get_UserInformation(mID);
                        USER_INFO.Text = uis._user_name + " 様  残高 " + uis._balance + "円";

                        // 入力内容チェック
                        if (Items.Count <= 0)
                        {
                            this._timer.Start();
                            return;
                        }
                      

                        foreach (Item checkItem in Items)
                        {
                            if (checkItem._num <= 0)
                            {
                                var msg = new ContentDialog();
                                msg.Title = "Error";
                                msg.Content = checkItem._itemName + "の個数が0になっています。";
                                msg.PrimaryButtonText = "OK";
                                await msg.ShowAsync();
                                JANCODE_TEXT.Focus(FocusState.Keyboard);
                                this._timer.Start();

                                return;
                            }

                            if (db.isStocked_Item(checkItem._janCode, checkItem._num))
                            {

                            }
                            else
                            {
                                var msg = new ContentDialog();
                                msg.Title = "Error";
                                msg.Content = checkItem._itemName + "の在庫数が不足しています。";
                                msg.PrimaryButtonText = "OK";
                                await msg.ShowAsync();
                                JANCODE_TEXT.Focus(FocusState.Keyboard);
                                this._timer.Start();
                                return;
                            }
                        }
                        // 入力内容チェックここまで
                        
                        foreach (Item it in Items)
                        {
                            total_price += it._price * it._num;
                        }
                        // 決済可否チェック

                        if (db.Check_Payment(mID, total_price))
                        {
                            db.Exec_Payment(mID, total_price);
                            foreach (Item item in Items)
                            {
                                db.Insert_Purchase_Log(mID, item._janCode, item._num, item._price);
                                db.Reduce_Item(item._janCode, item._num);
                            }
                            UsersInformation ui = db.Get_UserInformation(mID);
                            // ここで音を出してもいいかも
                            SE.Play();
                            USER_INFO.Text = uis._user_name + " 様  残高 " + (uis._balance - total_price) + "円 決済成功";
                            var msg = new ContentDialog();
                            msg.FontSize = 74;
                            msg.Title = "決済に成功しました。";
                            msg.Content = "残高 " + ui._balance + "円";
                            msg.PrimaryButtonText = "OK";
                            //await msg.ShowAsync();
                            Items.Clear();
                            JANCODE_TEXT.Focus(FocusState.Keyboard);
                            this._timer.Start();
                            return;
                        }
                        else
                        {
                            
                            var msg = new ContentDialog();
                            msg.Title = "Error";
                            msg.Content = "残高が不足しています。";
                            msg.PrimaryButtonText = "OK";
                            await msg.ShowAsync();
                            JANCODE_TEXT.Focus(FocusState.Keyboard);
                            this._timer.Start();
                            return;
                        }

                        // 決済可否チェックここまで
                    }
                    else
                    {
                        // ダイアログ表示中も裏でタイマーが走るようなので一旦止めています。
                        // CheckFunction.Show_Messageを使用していないのは非同期スレッドが立つらしく確認する前にタイマーがスタートしてしまう為
                        
                        var msg = new ContentDialog();
                        msg.Title = "Error";
                        msg.Content = "登録されていないカードです。";
                        msg.PrimaryButtonText = "OK";
                        await msg.ShowAsync();
                        JANCODE_TEXT.Focus(FocusState.Keyboard);
                        this._timer.Start();
                    }
                }else{
                    this._timer.Start();
                    USER_INFO.Text = "";
                }
            }
            catch(Exception es)
            {
                var msg = new ContentDialog();
                msg.Title = "Error";
                msg.Content = "不明なエラーです。管理者に問い合わせて下さい。\n" + es;
                msg.PrimaryButtonText = "OK";
                await msg.ShowAsync();
                JANCODE_TEXT.Focus(FocusState.Keyboard);
                this._timer.Start();
            }
        }

        // カード読込メソッド
        private async Task<string> Getmid()
        {
            // Reader検索
            var selector = SmartCardReader.GetDeviceSelector(SmartCardReaderKind.Any);
            var devices = await DeviceInformation.FindAllAsync(selector);
            var device = devices.FirstOrDefault();
            if (device == null)
            {
                return "";
            }

            var reader = await SmartCardReader.FromIdAsync(device.Id);
            if (reader == null)
            {
                return "";
            }

            // カード検索
            var cards = await reader.FindAllCardsAsync();
            var card = cards.FirstOrDefault();
            if (card == null)
            {
                return "";
            }

            // 接続してポーリングコマンド送信
            using (var con = await card.ConnectAsync())
            {
                var handler = new AccessHandler(con);
                try
                {
                    var result = await handler.TransparentExchangeAsync(new byte[] { 6, 0, 0xff, 0xff, 0, 3 });
                    byte[] idm = new byte[8];
                    Array.Copy(result, 2, idm, 0, idm.Length);
                    string s = "";
                    foreach (byte b in idm)
                    {
                        // 0x00がでたらループを抜けるならここにif文とかいれる
                        s += b.ToString("X2");
                    }
                    return s;
                }
                catch
                {
                    return "";
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // タイマーが既に作成されていた場合はStartだけ行う。(一度作成したタイマーは破棄されないため)
            if (_timer == null)
            {
                // タイマー生成
                _timer = new DispatcherTimer();
                // タイマーイベントの間隔設定(1秒間隔)
                this._timer.Interval = TimeSpan.FromSeconds(1);
                this._timer.Tick += Check_Card;
                this._timer.Start();
            }
            else
            {
                this._timer.Start();
            }
        }

        // リストコレクション初期設定メソッド
        private ObservableCollection<Item> GetItem()
        {
            return Items;
        }

        // 選択セル変更イベント
        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            if (dg.SelectedItem == null) return;
            Item item = dg.SelectedItem as Item;
            last_jan = item._janCode;
            dg.SelectedItem = null;
            JANCODE_TEXT.Focus(FocusState.Keyboard);
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

        // JANCODE_BOXエンター押下処理
        string last_jan = "1";
        private void JANCODE_TEXT_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            //Encoding Enc = Encoding.GetEncoding("");
            //if (Enc.GetByteCount(JANCODE_TEXT.Text) == JANCODE_TEXT.Text.Length * 2
            //JANCODE_TEXT.Text = Regex.Replace(JANCODE_TEXT.Text, "[０-９]", p => ((char)(p.Value[0] - '０' + '0')).ToString());
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
                                CheckFunction.Message_Show("Error", "データベースに存在しない商品です。");
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
                }
                else
                {
                    Items.First(x => x._janCode == last_jan)._num = 0;
                }
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }
            finally
            {
                JANCODE_TEXT.Focus(FocusState.Keyboard);
            }
        }

        // 最後に登録したJANコードレコードの商品名に文字列を加えるメソッド
        private void ADD_Num(string num)
        {
            try
            {
                Items.First(x => x._janCode == last_jan)._num = int.Parse(Items.First(x => x._janCode == last_jan)._num.ToString() == "0" ? num.ToString() : Items.First(x => x._janCode == last_jan)._num.ToString() + num.ToString());
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }
            finally
            {
                JANCODE_TEXT.Focus(FocusState.Keyboard);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            this._timer.Stop();
            Frame.Navigate(typeof(AuthenticationScreen));
        }

        private void DataGrid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            JANCODE_TEXT.Focus(FocusState.Keyboard);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            JANCODE_TEXT.Focus(FocusState.Keyboard);
        }
        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (btn.IsEnabled == true)
            {
                btn.IsEnabled = false;
            }
            else
            {
                btn.IsEnabled = true;
            }
        }

    }
}
