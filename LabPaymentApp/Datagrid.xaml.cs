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

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace LabPaymentApp
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    /// 

    public sealed partial class Datagrid : Page
    {
        ObservableCollection<Item> Items = new ObservableCollection<Item>();
        // カテゴリリストの定義
        public List<Category> categoryList = Categories.categoryList;
        // 権限リストの定義
        public List<Permission> permissionList = Permissions.permissionList;

        public Datagrid()
        {
            this.InitializeComponent();
            // Bindingで呼び出せる？→メソッドをバインドしているのでBindingで呼び出されるたびに実行される？
            DataContext = this.GetItem();
        }

        public ObservableCollection<Item> GetItem(){
            Items.Add(new Item() { _janCode = "aa", _itemName = "bbb", _categoryId = 0, _num = 2, _price = 500});
            Items.Add(new Item() { _janCode = "4902102061599", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999});
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 9999999   });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "9999999999", _itemName = "うすしお", _categoryId = 0, _num = 1, _price = 99999999  });
            Items.Add(new Item() { _janCode = "1", _itemName = "のりしお", _categoryId = 0, _num = 1, _price = 99 });
            return Items;
        }

        private void Add_Record_Click(object sender, RoutedEventArgs e)
        {
            Items.Add(new Item() { _janCode = "12", _itemName = "のりしお", _categoryId = 0, _num = 1, _price = 99 });
            DataGrid dg = FindName("dataGrid") as DataGrid;
            dg.RemoveFocusEngagement();
        }
    }
}
