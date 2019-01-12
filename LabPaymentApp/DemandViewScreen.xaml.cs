using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class DemandViewScreen : Page
    {
        // 要望リストの定義(編集可能)
        private ObservableCollection<DemandsInformation> Demands = new ObservableCollection<DemandsInformation>();

        public DemandViewScreen()
        {
            this.InitializeComponent();
            // データバインディング用
            DataContext = this.GetDemands();
        }

        // 要望リスト初期化メソッド
        private ObservableCollection<DemandsInformation> GetDemands()
        {
            DatabaseAccess db = new DatabaseAccess();
            var all_demands = db.Get_AllDemandInformation();
            foreach (var demand in all_demands)
            {
                Demands.Add(demand);
            }
            return Demands;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
        }
    }
}
