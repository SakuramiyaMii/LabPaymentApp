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
    public sealed partial class UserListEditScreen : Page
    {
        public UserListEditScreen()
        {
            this.InitializeComponent();
        }

        // デザイン用のイベントハンドラ　実装時削除
        private void Pre_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserInformationEditScreen));
        }

        private void User_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserAdditionalScreen));
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
        }
    }
}
