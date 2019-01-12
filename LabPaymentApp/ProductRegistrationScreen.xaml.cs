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
    public sealed partial class ProductRegistrationScreen : Page
    {
        // アイテムリストの定義(編集可能)
        private ObservableCollection<Item> Items = new ObservableCollection<Item>();
        // カテゴリリストの定義(編集不可)
        private List<Category> categoryList = Categories.categoryList;


        public ProductRegistrationScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetItem();
        }

        // リストコレクション初期設定メソッド
        private ObservableCollection<Item> GetItem()
        {
            Items.Add(new Item() { _janCode = "40352231964", _itemName = "ファンタグレープ 500ml", _categoryId = 0, _num = 24, _price = 80 });
            return Items;
        }

        private void Registration_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductRegistrationConfirmationScreen));
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MenuScreen));
        }

        // 各レコードの削除ボタンで読み込まれるメソッド
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // イベントを発生させたボタンの取得
            Button b = (Button)sender;
            // DataGridの取得
            DataGrid dg = FindName("dataGrid") as DataGrid;
            try
            {
                // Itemsのうち、削除ボタンのtagに付与したJANコードと一致するレコードを検索し、Itemsから削除する
                Items.Remove(Items.First(x => x._janCode == (string)b.Tag));
            }
            catch
            {
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }

        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            dg.SelectedItem = null;
        }
    }
}
