using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// カテゴリ定義クラス
/// 使用する際はCategories.categoryListを参照する
/// </summary>

namespace LabPaymentApp
{
    public class Category
    {
        public int _categoryId { get; set; }
        public string _categoryName { get; set; }
    }

    public class Categories
    {
        public static List<Category> categoryList = new List<Category>
            {
                new Category {_categoryId = 0, _categoryName = "未定義"},
                new Category {_categoryId = 1, _categoryName = "水・飲料"},
                new Category {_categoryId = 2, _categoryName = "カップ麺(ラーメン)"},
                new Category {_categoryId = 3, _categoryName = "カップ麺(その他)"},
                new Category {_categoryId = 4, _categoryName = "チョコレート菓子"},
                new Category {_categoryId = 5, _categoryName = "スナック菓子"},
                new Category {_categoryId = 6, _categoryName = "駄菓子"},
                new Category {_categoryId = 7, _categoryName = "ファミリーパック菓子"},
                new Category {_categoryId = 8, _categoryName = "アイス"},
                new Category {_categoryId = 9, _categoryName = "その他"}
            };
    }
}
