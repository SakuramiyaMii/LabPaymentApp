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
            //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e){
            // タイマー生成
            _timer = new DispatcherTimer();
            // タイマーイベントの間隔設定(0.1秒間隔)
            _timer.Interval = TimeSpan.FromSeconds(0.1);
            _timer.Tick += Check_Card;
            // タイマーをスタートする
            _timer.Start();
        }

        // タイマー用メソッド
        // カードが翳された場合、画面遷移を行う
        private async void Check_Card(object sender, object e){
            string mID = await Getmid();
            if(mID != ""){
                // タイマーの停止
                _timer.Stop();
                StaticParam._mID = mID;
                // 遷移
                Frame.Navigate(typeof(MenuScreen));
            }
        }

        private async void Auth_Comp_Button_Click(object sender, RoutedEventArgs e)
        {
            string mID = await Getmid();
            if (mID != ""){
                var dl = new MessageDialog(mID);
                await dl.ShowAsync();
            }
            StaticParam._mID = mID;

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
    }
}
