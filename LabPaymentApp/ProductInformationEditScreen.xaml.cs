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
    public sealed partial class ProductInformationEditScreen : Page
    {
        Item item = new Item();
        public ProductInformationEditScreen()
        {
            this.InitializeComponent();
        }

        // 別のページから遷移してきた時に呼び出されるメソッド
        // 画面描画は完了していないのでLoadedメソッドを利用？
        // 参照 : ttps://qiita.com/nagasakulllo/items/0b06ccc66b9fe0909b3f
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                string parent_jan_code = e.Parameter as string;
                DatabaseAccess db = new DatabaseAccess();
                item = db.Get_Item(parent_jan_code);
            }
        }

        private void Edit_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            // 入力パラメータチェック
            if (janCode_TEXT.Text == "")
            {
                // JANコードが未入力です。
                CheckFunction.Message_Show("Error", "JANコードが未入力です。");
                return;
            }
            else if (itemName_TEXT.Text == "")
            {
                // 商品名が未入力です。
                CheckFunction.Message_Show("Error", "商品名が未入力です。");
                return;
            }
            else if (price_TEXT.Text == "")
            {
                // 価格が未入力です。
                CheckFunction.Message_Show("Error", "価格が未入力です。");
                return;
            }
            else if (num_TEXT.Text == "")
            {
                // 在庫が未選択です。
                CheckFunction.Message_Show("Error", "在庫が未入力です。");
                return;
            }
            else if (category_TEXT.SelectedIndex == 0)
            {
                // カテゴリが未選択です。
                CheckFunction.Message_Show("Error", "カテゴリが未選択です。");
                return;

            }

            // フォーマットチェック
            if (!CheckFunction.JANCODE_Integrity_Check(janCode_TEXT.Text))
            {
                CheckFunction.Message_Show("Error", "JANコードのフォーマットが間違っています。なんでこのエラー出たの？");
                return;
            }
            else if (!CheckFunction.itemName_Integrity_Check(itemName_TEXT.Text))
            {
                CheckFunction.Message_Show("Error", "登録できる商品名は５０文字以下です。");
                return;
            }
            else if (!CheckFunction.price_Integrity_Check(price_TEXT.Text))
            {
                CheckFunction.Message_Show("Error", "登録できる価格は0～5000の値です。");
                return;
            }
            else if (!CheckFunction.num_Integrity_Check(num_TEXT.Text))
            {
                CheckFunction.Message_Show("Error", "登録できる在庫数は0～200の値です。");
                return;
            }

            Enable_Toggle();
            DatabaseAccess db = new DatabaseAccess();
            if (db.Search_Item(item._janCode))
            {
                db.Delete_Item(item._janCode);
                Item add_item = new Item(janCode_TEXT.Text,itemName_TEXT.Text,category_TEXT.SelectedIndex,int.Parse(price_TEXT.Text),int.Parse(num_TEXT.Text));
                db.Insert_Item(add_item);
                CheckFunction.Message_Show(add_item._itemName + " の情報を更新しました。", "");
                Frame.Navigate(typeof(ProductListEditScreen));
            }
            else
            {
                CheckFunction.Message_Show("Error", "DB上に対象となるJANコードが存在しません。");
                Enable_Toggle();
                return;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductListEditScreen));
        }

        private async void Product_Delete_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            var msgs = new ContentDialog();
            msgs.Title = item._itemName + " のデータを削除します。";
            msgs.Content = "この操作は取り消しできません。";
            msgs.PrimaryButtonText = "OK";
            msgs.SecondaryButtonText = "Cancel";
            var result = await msgs.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                // OKの場合
                DatabaseAccess db = new DatabaseAccess();
                if (db.Search_Item(item._janCode))
                {
                    db.Delete_Item(item._janCode);
                    CheckFunction.Message_Show(item._itemName + " の削除に成功しました。", "");
                    Frame.Navigate(typeof(ProductListEditScreen));
                }
                else
                {
                    CheckFunction.Message_Show("Error", "DB上に対象のJANコードが存在しません。");
                }
            }
            else if (result == ContentDialogResult.Secondary)
            {
                // Cancelの場合
                Enable_Toggle();
                return;
            }
            else
            {
                Enable_Toggle();
                return;
            }
            Enable_Toggle();
            return;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            janCode_TEXT.Text = item._janCode;
            itemName_TEXT.Text = item._itemName;
            price_TEXT.Text = item._price.ToString();
            num_TEXT.Text = item._num.ToString();
            category_TEXT.SelectedIndex = item._categoryId;

        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Back_Button.IsEnabled == true)
            {
                Back_Button.IsEnabled = false;
                Product_Delete_Button.IsEnabled = false;
                Edit_Decide_Button.IsEnabled = false;
            }
            else
            {
                Back_Button.IsEnabled = true;
                Product_Delete_Button.IsEnabled = true;
                Edit_Decide_Button.IsEnabled = true;
            }
        }
    }
}
