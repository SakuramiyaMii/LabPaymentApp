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
    public sealed partial class LogoutCompleteScreen : Page
    {
        private DispatcherTimer _timer;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // タイマーが既に作成されていた場合はStartだけ行う。(一度作成したタイマーは破棄されないため)
            if (_timer == null)
            {
                // タイマー生成
                _timer = new DispatcherTimer();
                // タイマーイベントの間隔設定(1秒間隔)
                _timer.Interval = TimeSpan.FromSeconds(5);
                _timer.Tick += Logout;
                this._timer.Start();
            }
            else
            {
                this._timer.Start();
            }
        }

        private void Logout(object sender, object e){
            // タイマーの停止
            this._timer.Stop();
            // 保持情報の初期化
            StaticParam.init();
            // 認証画面への遷移
            Frame.Navigate(typeof(AuthenticationScreen));
        }
        public LogoutCompleteScreen()
        {
            this.InitializeComponent();
        }

        private void Top_Transit_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            // タイマーの停止
            this._timer.Stop();
            // 保持情報の初期化
            StaticParam.init();
            // 認証画面への遷移
            Frame.Navigate(typeof(AuthenticationScreen));
        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Top_Transit_Button.IsEnabled == true)
            {
                Top_Transit_Button.IsEnabled = false;
            }
            else
            {
                Top_Transit_Button.IsEnabled = true;
            }
        }
    }
}
