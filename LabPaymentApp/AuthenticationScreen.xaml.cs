using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using System.Threading.Tasks;

// Felica関係
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;
using Windows.Networking.Proximity;
using Felica;



// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace LabPaymentApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class AuthenticationScreen : Page
    {
        // タイマー変数
        private DispatcherTimer _timer;

        public AuthenticationScreen()
        {
            this.InitializeComponent();
            // フルスクリーン化
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e){
            // タイマーが既に作成されていた場合はStartだけ行う。(一度作成したタイマーは破棄されないため)
            if (_timer == null)
            {
                // タイマー生成
                _timer = new DispatcherTimer();
                // タイマーイベントの間隔設定(1秒間隔)
                this._timer.Interval = TimeSpan.FromSeconds(1);
                this._timer.Tick += Check_Card;
                this._timer.Start();
            }else{
                this._timer.Start();
            }
        }

        // タイマー用メソッド
        // カードが翳された場合、画面遷移を行う
        private async void Check_Card(object sender, object e){
            try
            {
                string mID = await Getmid();
                if (mID != "")
                {
                    Enable_Toggle();
                    // mIDが登録されているかのチェック
                    DatabaseAccess db = new DatabaseAccess();
                    if(db.Search_UserInformation(mID)){
                        // タイマーの停止
                        this._timer.Stop();
                        // ユーザー情報の取得
                        UsersInformation user = db.Get_UserInformation(mID);
                        StaticParam._mID = user._mid;
                        StaticParam._userName = user._user_name;
                        StaticParam._balance = user._balance;
                        StaticParam._permission = user._permission;
                        // 遷移
                        Frame.Navigate(typeof(MenuScreen),user);
                    }
                    else{
                        // ダイアログ表示中も裏でタイマーが走るようなので一旦止めています。
                        // CheckFunction.Show_Messageを使用していないのは非同期スレッドが立つらしく確認する前にタイマーがスタートしてしまう為
                        this._timer.Stop();
                        var msg = new ContentDialog();
                        msg.Title = "Error";
                        msg.Content = "登録されていないカードです。";
                        msg.PrimaryButtonText = "OK";
                        await msg.ShowAsync();
                        this._timer.Start();
                    }
                }
            }catch{
            }
        }

        private void Auth_Comp_Button_Click(object sender, RoutedEventArgs e)
        {
            // タイマーの停止
            this._timer.Stop();
            // 遷移
            Frame.Navigate(typeof(MenuScreen));
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
                try{

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
               catch{
                    return "";
               }
            }
        }

        // デバッグ画面遷移イベント
        private void Debug_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Debug));
        }

        private void Datagrid_test_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(Datagrid));
        }

        private void Mode_toggle_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            // タイマーの停止
            this._timer.Stop();
            Frame.Navigate(typeof(QuickPurchaseScreen));
        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (mode_toggle.IsEnabled == true)
            {
                mode_toggle.IsEnabled = false;
            }
            else
            {
                mode_toggle.IsEnabled = true;
            }
        }
    }
}
