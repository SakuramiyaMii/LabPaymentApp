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

// Felica関係
using Windows.Devices.Enumeration;
using Windows.Devices.SmartCards;
using Windows.Networking.Proximity;
using Felica;
using System.Threading.Tasks;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace LabPaymentApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class UserAdditionalScreen : Page
    {
        public UserAdditionalScreen()
        {
            this.InitializeComponent();
        }

        ///////////////////////////////////////////////// mid読み取り
        
        // タイマー変数
        private DispatcherTimer _timer;

        // タイマースタート
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // タイマーが既に作成されていた場合はStartだけ行う。(一度作成したタイマーは破棄されないため)
            if (_timer == null)
            {
                // タイマー生成
                _timer = new DispatcherTimer();
                // タイマーイベントの間隔設定(1秒間隔)
                _timer.Interval = TimeSpan.FromSeconds(1);
                _timer.Tick += Check_Card;
            }
            // タイマーをスタートする
            _timer.Start();
        }

        // タイマー用メソッド
        // カードが翳された場合、midの取得、midテキストに代入を行う。
        private async void Check_Card(object sender, object e)
        {
            try
            {
                string mID = await Getmid();
                if (mID != "")
                {

                    mid_TEXT.Text = mID;
                }
            }catch{

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
        
        ///////////////////////////////////////////////// mid読み取りここまで

        // 追加決定ボタン押下イベント
        private void User_Add_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            // 入力パラメータチェック
            if(mid_TEXT.Text == ""){
                // midが未入力です。
                CheckFunction.Message_Show("Error","midが未入力です。");
                return;
            }else if(userName_TEXT.Text == ""){
                // 名前が未入力です。
                CheckFunction.Message_Show("Error","名前が未入力です。");
                return;
            }else if(balance_TEXT.Text == ""){
                // 残高が未入力です。
                CheckFunction.Message_Show("Error","残高が未入力です。");
                return;
            }else if(permisson_TEXT.SelectedIndex == 0){
                // 権限が未選択です。
                CheckFunction.Message_Show("Error","権限が未選択です。");
                return;
            }

            // フォーマットチェック
            if(!CheckFunction.mid_Integrity_Check(mid_TEXT.Text)){
                CheckFunction.Message_Show("Error","midのフォーマットが間違っています。もしくは未対応のカードです。");
                return;
            }else if(!CheckFunction.balance_Integrity_Check(balance_TEXT.Text)){
                CheckFunction.Message_Show("Error", "初期登録できる残高は0～99999の値です。");
                return;
            }else if(!CheckFunction.user_name_Integrity_Check(userName_TEXT.Text)){
                CheckFunction.Message_Show("Error", "登録できる名前は25文字以下です。");
                return;
            }


            // 既登録チェック
            DatabaseAccess db = new DatabaseAccess();
            if(db.Search_UserInformation(mid_TEXT.Text)){
                // 既登録されています。
                CheckFunction.Message_Show("Error", "このカードは既に登録されています。同じカードは登録できません。");
                return;
            }

            // ユーザー登録
            UsersInformation ui = new UsersInformation(mid_TEXT.Text,userName_TEXT.Text,int.Parse(balance_TEXT.Text),((ComboBoxItem)permisson_TEXT.SelectedItem).Content as string);
            try{
                db.Insert_UserInformation(ui);
            }catch{
                // タイマーストップ
                _timer.Stop();
                Frame.Navigate(typeof(OperationFailureScreen));
                return;
            }

            // タイマーストップ
            _timer.Stop();
            // 画面遷移
            Frame.Navigate(typeof(OperationSuccessfulScreen));
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            Frame.Navigate(typeof(UserListEditScreen));
        }
    }
}
