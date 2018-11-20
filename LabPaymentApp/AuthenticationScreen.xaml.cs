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

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace LabPaymentApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class AuthenticationScreen : Page
    {
        public AuthenticationScreen()
        {
            this.InitializeComponent();
            // フルスクリーン化
            //Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
        }

        private void Auth_Comp_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
        }
    }
}
