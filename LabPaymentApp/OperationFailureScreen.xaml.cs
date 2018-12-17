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
    public sealed partial class OperationFailureScreen : Page
    {
        private DispatcherTimer _timer;
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // タイマー生成
            _timer = new DispatcherTimer();
            // タイマーイベントの間隔設定(5秒間隔)
            _timer.Interval = TimeSpan.FromSeconds(5);
            _timer.Tick += Goto_Menu;
            // タイマーをスタートする
            _timer.Start();
        }

        private void Goto_Menu(object sender, object e)
        {
            // タイマーの停止
            _timer.Stop();
            // メニュー画面への遷移
            Frame.Navigate(typeof(MenuScreen));
        }

        public OperationFailureScreen()
        {
            this.InitializeComponent();
        }

        private void Top_Transit_Button_Click(object sender, RoutedEventArgs e)
        {
            // タイマーの停止
            _timer.Stop();
            // メニュー画面への遷移
            Frame.Navigate(typeof(MenuScreen));
        }
    }
}
