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
using Microsoft.Toolkit.Uwp.UI.Controls;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace LabPaymentApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class UserListEditScreen : Page
    {
        // ユーザーリストの定義(編集可能)
        private ObservableCollection<UsersInformation> Users = new ObservableCollection<UsersInformation>();

        public UserListEditScreen()
        {
            this.InitializeComponent();
            // データバインディング用
            DataContext = this.GetUsers();
        }

        // ユーザーリスト初期化メソッド
        private ObservableCollection<UsersInformation> GetUsers()
        {
            DatabaseAccess db = new DatabaseAccess();
            var all_users = db.Get_AllUserInformation();
            foreach (var user in all_users)
            {
                Users.Add(user);
            }
            //Users.Add(new UsersInformation() { _mid = "40352231964", _user_name = "テスト太郎", _balance = 1980, _permission = "利用者" });
            return Users;
        }

        // デザイン用のイベントハンドラ　実装時削除
        private void Pre_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserInformationEditScreen));
        }

        private void User_Add_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(UserAdditionalScreen));
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(MenuScreen));
        }

        // 各レコードの編集ボタンで読み込まれるメソッド
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            Button bt = (Button)sender;
            bt.IsEnabled = false;
            Enable_Toggle();
            // イベントを発生させたボタンの取得
            Button b = (Button)sender;
            // DataGridの取得
            DataGrid dg = FindName("dataGrid") as DataGrid;
            try
            {
                // 編集ボタンのtagに付与したmIDを編集するユーザーとし、編集画面へ遷移する
                string mid = (string)b.Tag;
                Frame.Navigate(typeof(UserInformationEditScreen),mid);
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
                Enable_Toggle();
                return;
            }

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
                if (e.Column.Tag.ToString() == "mid")
                {
                    dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users orderby i._mid descending select i);
                }
                else if (e.Column.Tag.ToString() == "User")
                {
                    dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users orderby i._user_name descending select i);
                }
                else if (e.Column.Tag.ToString() == "Balance")
                {
                    dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users orderby i._balance descending select i);
                }
                else if (e.Column.Tag.ToString() == "Permission")
                {
                    dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users orderby i._permission descending select i);
                }

            }
            else
            {
                // 選択collumが降順状態であれば、昇順にする
                e.Column.SortDirection = DataGridSortDirection.Ascending;

                //選択Collumによって分岐
                if (e.Column.Tag.ToString() == "mid")
                {
                    dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users orderby i._mid ascending select i);
                }
                else if (e.Column.Tag.ToString() == "User")
                {
                    dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users orderby i._user_name ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Balance")
                {
                    dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users orderby i._balance ascending select i);
                }
                else if (e.Column.Tag.ToString() == "Permission")
                {
                    dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users orderby i._permission ascending select i);
                }
            }
            // 現在選ばれていたcollumの保持
            c = e.Column;
        }

        // リストに対してキーワードフィルターを適用するメソッド
        // ちょっと無駄な実装をしている→無駄を取り除いた
        private void Search_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users where i._mid.Contains(Keyword_Box.Text) | i._user_name.Contains(Keyword_Box.Text) | i._permission.Contains(Keyword_Box.Text) | i._balance.ToString().Contains(Keyword_Box.Text)  orderby i._mid descending select i);
            Enable_Toggle();
        }

        private void Keyword_Box_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter){
                dataGrid.ItemsSource = new ObservableCollection<UsersInformation>(from i in Users where i._mid.Contains(Keyword_Box.Text) | i._user_name.Contains(Keyword_Box.Text) | i._permission.Contains(Keyword_Box.Text) | i._balance.ToString().Contains(Keyword_Box.Text) orderby i._mid descending select i);
            }
        }
        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Back_Button.IsEnabled == true)
            {
                Back_Button.IsEnabled = false;
                User_Add_Button.IsEnabled = false;
                Search_Button.IsEnabled = false;
            }
            else
            {
                Back_Button.IsEnabled = true;
                User_Add_Button.IsEnabled = true;
                Search_Button.IsEnabled = true;
            }
        }

    }
}
