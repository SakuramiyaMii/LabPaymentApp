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
        
        
        // 別のページから遷移してきた時に呼び出されるメソッド
        // 画面描画は完了していないのでLoadedメソッドを利用？
        // 参照 : ttps://qiita.com/nagasakulllo/items/0b06ccc66b9fe0909b3f
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // ユーザー情報の再読込
            CheckFunction.update_user();

            if (StaticParam._permission == "利用者")
            {
                Product_Add_Button.IsEnabled = false;
                Product_Edit_Button.IsEnabled = false;
                User_Edit_Button.IsEnabled = false;
            }
            else if(StaticParam._permission == "仕入者")
            {
                User_Edit_Button.IsEnabled = false;
            }
            else if (StaticParam._permission == "管理者"){
                // 特になし
            }else{
                Product_Add_Button.IsEnabled = false;
                Product_Edit_Button.IsEnabled = false;
                User_Edit_Button.IsEnabled = false;
            }
        }



        // ページ描画完了後に呼び出されるメソッド(xamlにて指定)
        private void setmID(object sender, RoutedEventArgs e)
        {
            Username_Text.Text = StaticParam._userName + " 様";
            Balance_Text.Text = "残高 " + StaticParam._balance +" 円";
        }

        private void Purchase_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(ProductPurchaseScreen));
        }

        private void Charge_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(ChargeScreen));
        }

        private void Demand_Input_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(DemandInputScreen));
        }

        private void Demand_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(DemandViewScreen));
        }

        private void Personal_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(PersonalHistoryViewScreen));
        }

        private void All_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(AllHistoryViewScreen));
        }

        private void OP_View_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(OperationHistoryViewScreen));
        }

        private void Hotsell_View_Button_Click(object sender, RoutedEventArgs e)
        {
            //CheckFunction.Message_Show("未実装機能です。","近日実装予定");
            Enable_Toggle();
            Frame.Navigate(typeof(HotSellingProductsViewScreen));
        }

        private void Product_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(ProductRegistrationScreen));
        }

        private void Product_Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(ProductListEditScreen));
        }

        private void User_Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(UserListEditScreen));
        }

        private void Logout_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(LogoutCompleteScreen));
        }
        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Logout_Button.IsEnabled == true)
            {
                Logout_Button.IsEnabled = false;
                Charge_Button.IsEnabled = false;
                Demand_Input_Button.IsEnabled = false;
                Demand_View_Button.IsEnabled = false;
                Hotsell_View_Button.IsEnabled = false;
                Personal_View_Button.IsEnabled = false;
                Product_Add_Button.IsEnabled = false;
                Product_Edit_Button.IsEnabled = false;
                All_View_Button.IsEnabled = false;
                Purchase_Button.IsEnabled = false;
                OP_View_Button.IsEnabled = false;
                User_Edit_Button.IsEnabled = false;
            }
            else
            {
                Logout_Button.IsEnabled = true;
                Charge_Button.IsEnabled = true;
                Demand_Input_Button.IsEnabled = true;
                Demand_View_Button.IsEnabled = true;
                Hotsell_View_Button.IsEnabled = true;
                Personal_View_Button.IsEnabled = true;
                Product_Add_Button.IsEnabled = true;
                Product_Edit_Button.IsEnabled = true;
                All_View_Button.IsEnabled = true;
                Purchase_Button.IsEnabled = true;
                OP_View_Button.IsEnabled = true;
                User_Edit_Button.IsEnabled = true;
            }
        }
    }
}
