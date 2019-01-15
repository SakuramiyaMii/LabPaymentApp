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
    public sealed partial class ProductListEditScreen : Page
    {
        // アイテムリストの定義(編集不可)
        private ObservableCollection<Item> Items = new ObservableCollection<Item>();
        // カテゴリリストの定義(編集不可)
        private List<Category> categoryList = Categories.categoryList;

        public ProductListEditScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetItems();
        }

        // ユーザーリスト初期化メソッド
        private ObservableCollection<Item> GetItems()
        {
            DatabaseAccess db = new DatabaseAccess();
            var all_items = db.Get_AllItem();
            foreach (var item in all_items)
            {
                Items.Add(item);
            }
            return Items;
        }



        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
        }

        // デザイン用仮イベント
        // 実装時は削除して下さい。
        private void Pre_Edit_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductInformationEditScreen));
        }

        // 各レコードの編集ボタンで読み込まれるメソッド
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // イベントを発生させたボタンの取得
            Button b = (Button)sender;
            try
            {
                // 編集ボタンのtagに付与したjan_codeを編集する商品とし、編集画面へ遷移する
                string jan_code = (string)b.Tag;
                Frame.Navigate(typeof(ProductInformationEditScreen), jan_code);
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
