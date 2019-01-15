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
    public sealed partial class ProductRegistrationConfirmationScreen : Page
    {
        // アイテムリストの定義(編集不可)
        private ObservableCollection<Item> Items = new ObservableCollection<Item>();
        private ObservableCollection<Item> cItems = new ObservableCollection<Item>();
        // カテゴリリストの定義(編集不可)
        private List<Category> categoryList = Categories.categoryList;

        public ProductRegistrationConfirmationScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetItem();
        }

        // 別のページから遷移してきた時に呼び出されるメソッド
        // 画面描画は完了していないのでLoadedメソッドを利用？
        // 参照 : ttps://qiita.com/nagasakulllo/items/0b06ccc66b9fe0909b3f
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            cItems = e.Parameter as ObservableCollection<Item>;
        }

        // リストコレクション初期設定メソッド
        private ObservableCollection<Item> GetItem()
        {
            return Items;
        }


        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            dg.SelectedItem = null;
        }

        // 登録確定イベント
        private void Registration_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DatabaseAccess db = new DatabaseAccess();
                // １件ずつ処理
                foreach (Item regItem in Items)
                {
                    if (db.Search_Item(regItem._janCode))
                    {
                        // DB上に存在している場合  
                        // →既登録されている在庫数を今回追加するデータに加えて上書き
                        // →流れとしては登録データを削除、在庫数他を更新して再登録
                        Item oldItem = db.Get_Item(regItem._janCode);
                        regItem._num += oldItem._num;
                        db.Delete_Item(regItem._janCode);
                        db.Insert_Item(regItem);
                    }
                    else
                    {
                        // DB上に存在していない場合
                        // →普通に登録
                        db.Insert_Item(regItem);
                    }
                }
                CheckFunction.Message_Show("商品の登録に成功しました。","");
                Frame.Navigate(typeof(MenuScreen));
            }
            catch{
                CheckFunction.Message_Show("Error","DBの登録に失敗しました。");
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ProductRegistrationScreen),Items);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Item it in cItems)
            {
                Items.Add(it);
            }
        }
    }
}
