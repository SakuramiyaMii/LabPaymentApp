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
            Frame.Navigate(typeof(UserAdditionalScreen));
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
        }

        // 各レコードの編集ボタンで読み込まれるメソッド
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
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
    }
}
