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
    public sealed partial class ProductPurchaseConfirmationScreen : Page
    {
        // 前画面からのアイテムリスト(編集不可)
        private ObservableCollection<Item> cItems = new ObservableCollection<Item>();
        // アイテムリストの定義(編集可能)
        private ObservableCollection<Item> Items = new ObservableCollection<Item>();
        // カテゴリリストの定義(編集不可)
        private List<Category> categoryList = Categories.categoryList;
        // 決済金額
        private int total_price = 0;

        public ProductPurchaseConfirmationScreen()
        {
            this.InitializeComponent();
            DataContext = this.GetItem();
        }

        // リストコレクション初期設定メソッド
        private ObservableCollection<Item> GetItem()
        {
            return Items;
        }

        // 別のページから遷移してきた時に呼び出されるメソッド
        // 画面描画は完了していないのでLoadedメソッドを利用？
        // 参照 : ttps://qiita.com/nagasakulllo/items/0b06ccc66b9fe0909b3f
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter != null)
            {
                cItems = e.Parameter as ObservableCollection<Item>;
            }
        }

        private void Purchase_Decide_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            DatabaseAccess db = new DatabaseAccess();
            if (db.Check_Payment(StaticParam._mID, total_price)){
                db.Exec_Payment(StaticParam._mID, total_price);
                foreach(Item item in Items){
                    db.Insert_Purchase_Log(StaticParam._mID,item._janCode,item._num,item._price);
                    db.Reduce_Item(item._janCode,item._num);
                }
                CheckFunction.Message_Show("決済に成功しました。","");
                Frame.Navigate(typeof(MenuScreen));
            } else{
                CheckFunction.Message_Show("Error","残高が不足しています。");
                Enable_Toggle();
                return;
            }
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Enable_Toggle();
            Frame.Navigate(typeof(ProductPurchaseScreen),Items);
        }

        private void DataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            DataGrid dg = (DataGrid)sender;
            dg.SelectedItem = null;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            foreach (Item it in cItems)
            {
                Items.Add(it);
                total_price += it._price * it._num;
            }
            TOTALPRICE_TEXT.Text =  total_price +"円";
            BALANCE_TEXT.Text = StaticParam._balance.ToString() + "円";
            AFTER_TEXT.Text = (StaticParam._balance - total_price).ToString() + "円";
        }

        // ボタン類のトグルメソッド
        private void Enable_Toggle()
        {
            if (Back_Button.IsEnabled == true)
            {
                Back_Button.IsEnabled = false;
                Purchase_Decide_Button.IsEnabled = false;
            }
            else
            {
                Back_Button.IsEnabled = true;
                Purchase_Decide_Button.IsEnabled = true;
            }
        }
    }
}
