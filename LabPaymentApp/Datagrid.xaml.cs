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

using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Collections.ObjectModel;
using LabPaymentApp.Models;
using System.Diagnostics;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace LabPaymentApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    /// 

    public sealed partial class Datagrid : Page
    {
        // アイテムリストの定義(編集可能)
        private ObservableCollection<Item> Items = new ObservableCollection<Item>();
        // カテゴリリストの定義(編集不可)
        private List<Category> categoryList = Categories.categoryList;
        // 権限リストの定義(編集不可)
        // private List<Permission> permissionList = Permissions.permissionList;

        public Datagrid()
        {
            this.InitializeComponent();
            // Bindingで呼び出せる？→メソッドをバインドしているのでBindingで呼び出されるたびに実行される？
            // GetItem()は初期設定時だけ読み込まれ、Itemsがバインド先として保持される
            // GetItem()以外でItemsを編集してもリストに反映される
            DataContext = this.GetItem();
        }

        // リストコレクション初期設定メソッド
        public ObservableCollection<Item> GetItem(){
            Items.Add(new Item() { _janCode = "40352231964", _itemName = "ファンタグレープ 500ml", _categoryId = 0, _num = 24, _price = 80});
            return Items;
        }

        private void Add_Record_Click(object sender, RoutedEventArgs e)
        {
            // レコード追加
            Items.Add(new Item() { _janCode = "12", _itemName = "のりしお", _categoryId = 0, _num = 1, _price = 99 });
            // レコード削除
            DataGrid dg = FindName("dataGrid") as DataGrid;
            var dgs = dg.SelectedItem as Item;
            try
            {
                Items.Remove(Items.First(x => x._janCode == "1"));
            }catch{
                System.Diagnostics.Debug.WriteLine("対象レコードが見つかりません");
            }
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
