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
    public sealed partial class UserInformationEditScreen : Page
    {
        // 編集対象のユーザーデータ
        UsersInformation ui = new UsersInformation();

        public UserInformationEditScreen()
        {
            this.InitializeComponent();
        }

        // 別のページから遷移してきた時に呼び出されるメソッド
        // 画面描画は完了していないのでLoadedメソッドを利用？
        // 参照 : ttps://qiita.com/nagasakulllo/items/0b06ccc66b9fe0909b3f
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            string parent_mid = e.Parameter as string;
            DatabaseAccess db = new DatabaseAccess();
            var all_user = db.Get_AllUserInformation();
            foreach(var user in all_user){
                if(user._mid == parent_mid){
                    ui = user;
                }
            }
        }

        private void User_Edit_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            // 入力パラメータチェック
            if (mid_TEXT.Text == "")
            {
                // midが未入力です。
                CheckFunction.Message_Show("Error", "midが未入力です。");
                return;
            }
            else if (userName_TEXT.Text == "")
            {
                // 名前が未入力です。
                CheckFunction.Message_Show("Error", "名前が未入力です。");
                return;
            }
            else if (balance_TEXT.Text == "")
            {
                // 残高が未入力です。
                CheckFunction.Message_Show("Error", "残高が未入力です。");
                return;
            }
            else if (permisson_TEXT.SelectedIndex == 0)
            {
                // 権限が未選択です。
                CheckFunction.Message_Show("Error", "権限が未選択です。");
                return;
            }

            // フォーマットチェック
            if (!CheckFunction.mid_Integrity_Check(mid_TEXT.Text))
            {
                CheckFunction.Message_Show("Error", "midのフォーマットが間違っています。もしくは未対応のカードです。");
                return;
            }
            else if (!CheckFunction.balance_Integrity_Check(balance_TEXT.Text))
            {
                CheckFunction.Message_Show("Error", "登録できる残高は0～99999の値です。");
                return;
            }
            else if (!CheckFunction.user_name_Integrity_Check(userName_TEXT.Text))
            {
                CheckFunction.Message_Show("Error", "登録できる名前は25文字以下です。");
                return;
            }

            Enable_Toggle();
            DatabaseAccess db = new DatabaseAccess();
            if(db.Search_UserInformation(ui._mid)){
                db.Delete_UserInformation(ui._mid);
                UsersInformation add_ui = new UsersInformation(mid_TEXT.Text, userName_TEXT.Text,int.Parse(balance_TEXT.Text),((ComboBoxItem)permisson_TEXT.SelectedItem).Content as string);
                db.Insert_UserInformation(add_ui);
                CheckFunction.Message_Show(add_ui._user_name + " の情報を更新しました。", "");
                Frame.Navigate(typeof(UserListEditScreen));
            }
            else{
                CheckFunction.Message_Show("Error","DB上に対象となるmIDが存在しません。");
                Enable_Toggle();
                return;
            }
        }

        // 戻る
        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(UserListEditScreen));
        }

        // 削除実行メソッド
        private async void User_Delete_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            var msgs = new ContentDialog();
            msgs.Title = ui._user_name +" のデータを削除します。";
            msgs.Content = "この操作は取り消しできません。";
            msgs.PrimaryButtonText = "OK";
            msgs.SecondaryButtonText = "Cancel";
            var result = await msgs.ShowAsync();
            if(result == ContentDialogResult.Primary){
                // OKの場合
                DatabaseAccess db = new DatabaseAccess();
                if(db.Search_UserInformation(ui._mid)){
                    db.Delete_UserInformation(ui._mid);
                    CheckFunction.Message_Show(ui._user_name + " の削除に成功しました。","");
                    Frame.Navigate(typeof(UserListEditScreen));
                }
                else{
                    CheckFunction.Message_Show("Error", "DB上に対象のmIDが存在しません。");
                }
            }else if(result == ContentDialogResult.Secondary){
                // Cancelの場合
                Enable_Toggle();
                return;
            }else{
                Enable_Toggle();
                return;
            }
            Enable_Toggle();
            return;
        }

        // ページ遷移完了後に実行されるイベントハンドラー
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            mid_TEXT.Text = ui._mid;
            userName_TEXT.Text = ui._user_name;
            balance_TEXT.Text = ui._balance.ToString();
            if (ui._permission == "管理者") permisson_TEXT.SelectedIndex = 3;
            else if (ui._permission == "仕入者") permisson_TEXT.SelectedIndex = 2;
            else if (ui._permission == "利用者") permisson_TEXT.SelectedIndex = 1;
        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle(){
            if(Back_Button.IsEnabled == true)
            {
                Back_Button.IsEnabled = false ;
                User_Delete_Decide_Button.IsEnabled = false;
                User_Edit_Decide_Button.IsEnabled = false;
            }else{
                Back_Button.IsEnabled = true;
                User_Delete_Decide_Button.IsEnabled = true;
                User_Edit_Decide_Button.IsEnabled = true;
            }
        }
    }
}
