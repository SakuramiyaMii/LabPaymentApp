﻿using Microsoft.Toolkit.Uwp.UI.Controls;
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
    public sealed partial class OperationHistoryViewScreen : Page
    {
        // ログリストの定義(編集不可)
        private ObservableCollection<OperationLog> Logs = new ObservableCollection<OperationLog>();

        public OperationHistoryViewScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetLogs();
        }

        // ログリスト初期化メソッド
        private ObservableCollection<OperationLog> GetLogs()
        {
            DatabaseAccess db = new DatabaseAccess();
            var all_Logs = db.Get_AllOperation();
            foreach (var log in all_Logs)
            {
                Logs.Add(log);
            }
            Logs = new ObservableCollection<OperationLog>(from i in Logs orderby i._date descending select i);
            return Logs;
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(MenuScreen));
        }

        // 選択行変更時にハイライトを表示しないようにする→削除実行時にハイライトがどっか行って不自然なので
        // 選択行のハイライトは消せたが選択項目の消し方がわからなかった
        // DataGrid.CurrentColumnIndexがそれっぽいがprivateなのでいじれなかった(アクセサもなかったっぽい)
        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            dg.SelectedItem = null;
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
                if (e.Column.Tag.ToString() == "Date")
                {
                    dataGrid.ItemsSource = new ObservableCollection<OperationLog>(from i in Logs orderby i._date descending select i);
                }
                else if (e.Column.Tag.ToString() == "User")
                {
                    dataGrid.ItemsSource = new ObservableCollection<OperationLog>(from i in Logs orderby i._user_name descending select i);
                }
                else if (e.Column.Tag.ToString() == "Detail")
                {
                    dataGrid.ItemsSource = new ObservableCollection<OperationLog>(from i in Logs orderby i._detail descending select i);
                }
            }
            else
            {
                // 選択collumが降順状態であれば、昇順にする
                e.Column.SortDirection = DataGridSortDirection.Ascending;

                // 選択collumによって分岐
                if (e.Column.Tag.ToString() == "Date")
                {
                    dataGrid.ItemsSource = new ObservableCollection<OperationLog>(from i in Logs orderby i._date ascending select i);
                }
                else if (e.Column.Tag.ToString() == "User")
                {
                    dataGrid.ItemsSource = new ObservableCollection<OperationLog>(from i in Logs orderby i._user_name ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Detail")
                {
                    dataGrid.ItemsSource = new ObservableCollection<OperationLog>(from i in Logs orderby i._detail ascending select i);
                }
            }
            // 現在選ばれていたcollumの保持
            c = e.Column;
        }
        // リストに対してキーワードフィルターを適用するメソッド
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            dataGrid.ItemsSource = new ObservableCollection<OperationLog>(from i in Logs where i._date.Contains(Keyword_Box.Text) | i._user_name.Contains(Keyword_Box.Text) | i._detail.Contains(Keyword_Box.Text) | i._mid.Contains(Keyword_Box.Text)  orderby i._date descending select i);
            Enable_Toggle();    
        }

        private void Keyword_Box_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                dataGrid.ItemsSource = new ObservableCollection<OperationLog>(from i in Logs where i._date.Contains(Keyword_Box.Text) | i._user_name.Contains(Keyword_Box.Text) | i._detail.Contains(Keyword_Box.Text) | i._mid.Contains(Keyword_Box.Text) orderby i._date descending select i);
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
