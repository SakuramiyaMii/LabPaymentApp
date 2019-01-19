using Microsoft.Toolkit.Uwp.UI.Controls;
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
                // クソみたいなコード(解説 : クラスを変えるのがめんどくさいので無理やりmIDをuser_nameに置換している)
                UsersInformation ui = db.Get_UserInformation(demand._mid);
                demand._mid = ui._user_name;
                
                Demands.Add(demand);
            }
            return Demands;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(MenuScreen));
        }

        // ソート
        // 直前に選択されていたcollumの保存変数
        DataGridColumn c = null;
        private void dg_sorting(object sender, DataGridColumnEventArgs e)
        {
            if (e.Column.Tag.ToString() == null) return;
            // 直前に選ばれていたcollumが別ならアイコンを消しておく
            if (c != e.Column && c != null) c.SortDirection = null;

            if (e.Column.SortDirection == null || e.Column.SortDirection == DataGridSortDirection.Ascending)
            {
                // 選択collumが未選択・昇順状態であれば、降順にする
                e.Column.SortDirection = DataGridSortDirection.Descending;

                //選択Collumによって分岐
                if (e.Column.Tag.ToString() == "ID")
                {
                    dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands orderby i._demand_id descending select i);
                }
                else if (e.Column.Tag.ToString() == "Date")
                {
                    dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands orderby i._created_at descending select i);
                }
                else if (e.Column.Tag.ToString() == "mID")
                {
                    dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands orderby i._mid descending select i);
                }
                else if (e.Column.Tag.ToString() == "Detail")
                {
                    dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands orderby i._demand_detail descending select i);
                }

            }
            else
            {
                // 選択collumが降順状態であれば、昇順にする
                e.Column.SortDirection = DataGridSortDirection.Ascending;

                //選択Collumによって分岐
                if (e.Column.Tag.ToString() == "ID")
                {
                    dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands orderby i._demand_id ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Date")
                {
                    dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands orderby i._created_at ascending select i);
                }
                else if (e.Column.Tag.ToString() == "mID")
                {
                    dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands orderby i._mid ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Detail")
                {
                    dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands orderby i._demand_detail ascending select i);
                }
            }
            // 現在選ばれていたcollumの保持
            c = e.Column;
        }

        // リストに対してキーワードフィルターを適用するメソッド
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands where i._created_at.Contains(Keyword_Box.Text) | i._demand_detail.Contains(Keyword_Box.Text) | i._demand_id.ToString().Contains(Keyword_Box.Text) | i._mid.Contains(Keyword_Box.Text) orderby i._demand_id descending select i);
            Enable_Toggle();
        }

        private void Keyword_Box_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                dataGrid.ItemsSource = new ObservableCollection<DemandsInformation>(from i in Demands where i._created_at.Contains(Keyword_Box.Text) | i._demand_detail.Contains(Keyword_Box.Text) | i._demand_id.ToString().Contains(Keyword_Box.Text) | i._mid.Contains(Keyword_Box.Text) orderby i._demand_id descending select i);
            }
        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Back_Button.IsEnabled == true)
            {
                Back_Button.IsEnabled = false;
                Search_Button.IsEnabled = false;
            }
            else
            {
                Back_Button.IsEnabled = true;
                Search_Button.IsEnabled = true;
            }
        }
    }
}
