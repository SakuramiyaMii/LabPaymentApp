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
    public sealed partial class MenuScreen : Page
    {
        public MenuScreen()
        {
            this.InitializeComponent();
        }

        private void Purchase_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductPurchaseScreen));
        }

        private void Charge_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ChargeScreen));
        }

        private void Demand_Input_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DemandInputScreen));
        }

        private void Demand_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(DemandViewScreen));
        }

        private void Personal_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(PersonalHistoryViewScreen));
        }

        private void All_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(AllHistoryViewScreen));
        }

        private void OP_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(OperationHistoryViewScreen));
        }

        private void Hotsell_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(HotSellingProductsViewScreen));
        }

        private void Product_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductRegistrationScreen));
        }

        private void Product_Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductListEditScreen));
        }

        private void User_Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserListEditScreen));
        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LogoutCompleteScreen));
        }
    }
}
