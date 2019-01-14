using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp
{
    class DatabaseAccess
    {
        // コンストラクタ
        public DatabaseAccess()
        {
            
        }

        // データベースオープン処理
        private SqliteConnection OpenDB()
        {
            // SQliteデータベース初期化
            // .dbを置くフォルダは C:/Users/{UserName}/AppData/Local/Packages/72458304-e57a-4f5a-8c1e-136be572b2b9/LocalState
            // 参考資料：https://stackoverflow.com/questions/36899829/sqlite-database-location-uwp
            string db_Name = "lpa_db.sqlite3"; //データベースファイル名
            string db_Path = "Filename = " + db_Name;
            SqliteConnection db = new SqliteConnection(db_Path);

            try
            {
                db.Open();
            }
            catch (SqliteException e)
            {
                throw e;
            }
            return db;
        }

        // UserInformation -------------------------------------------------------------------------------------

        /// <summary>
        ///     users_infomationデータベースから全てのユーザ情報を取得
        /// </summary>
        /// <returns>
        ///     List<UsersInfomation> : UsersInfomationクラスを参照
        /// </returns>
        public List<UsersInformation> Get_AllUserInformation()
        {
            SqliteConnection db = this.OpenDB();
            
            SqliteCommand command = new SqliteCommand("SELECT * from users_information", db);

            List<UsersInformation> ret = new List<UsersInformation>();
            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    UsersInformation user = new UsersInformation();
                    user._mid = query.GetString(0);
                    user._balance = query.GetInt32(1);
                    user._user_name = query.GetString(2);
                    user._permission = query.GetString(3);
                    ret.Add(user);
                }
            }

            db.Close();

            return ret;
        }

        /// <summary>
        ///     UserInformationデータベースに引数として与えるmidを含むレコードが存在するかどうか検索
        /// </summary>
        /// <param name="mid"> 検索したいmid </param>
        /// <returns>
        ///     検索結果 存在する->true,存在しない->false
        /// </returns>
        public bool Search_UserInformation(string mid)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT count(*) from users_information where @mid = mid", db);
            command.Parameters.AddWithValue("@mid", mid);

            bool ret;

            using (SqliteDataReader query = command.ExecuteReader())
            {
                query.Read();
                if(query.GetInt32(0)==1)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            db.Close();

            return ret;
        }

        /// <summary>
        ///     UserInformationテーブルにデータを挿入
        /// </summary>
        /// <param name="user"> 挿入したいUserのUserInformationオブジェクト </param>
        /// <returns>
        ///     成功 -> true , 失敗(主キー被りなど) -> false
        /// </returns>
        public bool Insert_UserInformation(UsersInformation user)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("INSERT INTO users_information VALUES(@mid,@balance,@user_name,@permission)", db);
            command.Parameters.AddWithValue("@mid", user._mid);
            command.Parameters.AddWithValue("@balance", user._balance);
            command.Parameters.AddWithValue("@user_name", user._user_name);
            command.Parameters.AddWithValue("@permission", user._permission);
            
            try
            {
                command.ExecuteReader();
            }
            catch
            {
                return false;
            }
            finally
            {
                db.Close();
            }

            return true;
        }

        /// <summary>
        ///     引数に与えたmidに該当するレコードを削除
        /// </summary>
        /// <param name="delete_mid"></param>
        /// <returns>
        ///     削除成功 -> true, 削除失敗 -> false
        /// </returns>
        public bool Delete_UserInformation(string delete_mid)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("DELETE FROM users_information WHERE @mid = mid", db);
            command.Parameters.AddWithValue("@mid", delete_mid);

            try
            {
                command.ExecuteReader();
            }
            catch
            {
                return false;
            }
            finally
            {
                db.Close();
            }

            return true;
        }

        // UserInformation[end] ----------------------------------------------------------------------------------

        // Demand ------------------------------------------------------------------------------------------------
        /// <summary>
        ///     demands_logデータベースから全ての要望情報を取得
        /// </summary>
        /// <returns>
        ///     List<DemandsInfomation> : DemandsInfomationクラスを参照
        /// </returns>
        public List<DemandsInformation> Get_AllDemandInformation()
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT * from demands_log", db);

            List<DemandsInformation> ret = new List<DemandsInformation>();
            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    DemandsInformation demand = new DemandsInformation();
                    demand._demand_id = query.GetInt32(0);
                    demand._created_at = query.GetString(1);
                    demand._mid = query.GetString(2);
                    demand._demand_detail = query.GetString(3);
                    ret.Add(demand);
                }
            }

            db.Close();

            return ret;
        }


        /// <summary>
        ///     demands_logテーブルにデータを挿入
        /// </summary>
        /// <param name="mid"> 要望の入力した利用者のmID </param>
        /// <param name="str"> 入力された要望文 </param>
        /// <returns>
        ///     成功 -> true , 失敗 -> false
        /// </returns>
        public bool Insert_Demand(string mid,string str)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("INSERT INTO demands_log(created_at,mid,demand_detail) VALUES(datetime('now','localtime'),@mid,@str)", db);
            command.Parameters.AddWithValue("@mid", mid);
            command.Parameters.AddWithValue("@str", str);

            try
            {
                command.ExecuteReader();
            }
            catch
            {
                return false;
            }
            finally
            {
                db.Close();
            }

            return true;
        }

        // Demand[end] -------------------------------------------------------------------------------------------

        // Item --------------------------------------------------------------------------------------------------

        /// <summary>
        ///     Items_informationデータベースから全ての商品情報を取得
        /// </summary>
        /// <returns>
        ///     List<Item> : Itemクラスを参照
        /// </returns>
        public List<Item> Get_AllItem()
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT * from Items_information", db);

            List<Item> ret = new List<Item>();
            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    Item item = new Item();
                    item._janCode = query.GetString(0);
                    item._itemName = query.GetString(1);
                    item._price = query.GetInt32(2);
                    item._num = query.GetInt32(3);
                    item._categoryId = query.GetInt32(4);
                    ret.Add(item);
                }
            }

            db.Close();

            return ret;
        }

        /// <summary>
        ///     Items_informationデータベースからjan_codeをもとに商品情報を取得
        /// </summary>
        /// <returns>
        ///     Item : Itemクラスを参照
        /// </returns>
        public Item Get_Item(string jan_code)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT * from Items_information where @jan_code = jan_code", db);
            command.Parameters.AddWithValue("@jan_code", jan_code);

            Item ret = new Item();
            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    ret._janCode = query.GetString(0);
                    ret._itemName = query.GetString(1);
                    ret._price = query.GetInt32(2);
                    ret._num = query.GetInt32(3);
                    ret._categoryId = query.GetInt32(4);
                }
            }

            db.Close();

            return ret;
        }


        /// <summary>
        ///     Items_logテーブルにデータを挿入
        /// </summary>
        /// <param name="item"> 挿入したい商品のItemオブジェクト </param>
        /// <returns>
        ///     成功 -> true , 失敗 -> false
        /// </returns>
        public bool Insert_Item(Item item)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("INSERT INTO Items_information(jan_code,product_name,price,quantity,category_id) VALUES(@janCode,@itemName,@price,@num,@categoryId)", db);
            command.Parameters.AddWithValue("@janCode", item._janCode);
            command.Parameters.AddWithValue("@itemName", item._itemName);
            command.Parameters.AddWithValue("@price", item._price);
            command.Parameters.AddWithValue("@num", item._num);
            command.Parameters.AddWithValue("@categoryId", item._categoryId);

            try
            {
                command.ExecuteReader();
            }
            catch
            {
                return false;
            }
            finally
            {
                db.Close();
            }

            return true;
        }

        /// <summary>
        ///     Item_informationデータベースに引数として与えるjan_codeを含むレコードが存在するかどうか検索
        /// </summary>
        /// <param name="jan_code"> 検索したいjan_code </param>
        /// <returns>
        ///     検索結果 存在する->true,存在しない->false
        /// </returns>
        public bool Search_Item(string jan_code)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT count(*) from Items_information where @jan_code = jan_code", db);
            command.Parameters.AddWithValue("@jan_code", jan_code);

            bool ret;

            using (SqliteDataReader query = command.ExecuteReader())
            {
                query.Read();
                if (query.GetInt32(0) == 1)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            db.Close();

            return ret;
        }

        /// <summary>
        ///     Item_informationデータベースに引数として与えるjan_codeの商品がnum個存在するかどうか検索
        /// </summary>
        /// <param name="jan_code"> 検索したいjan_code </param>
        /// <param name="num"> リクエストされた個数 </param>
        /// <returns>
        ///     検索結果 存在する->true,存在しない->false
        /// </returns>
        public bool isStocked_Item(string jan_code,int num)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT count(*) from Items_information where @jan_code = jan_code AND quantity >= @num", db);
            command.Parameters.AddWithValue("@jan_code", jan_code);
            command.Parameters.AddWithValue("@num", num);

            bool ret;

            using (SqliteDataReader query = command.ExecuteReader())
            {
                query.Read();
                if (query.GetInt32(0) == 1)
                {
                    ret = true;
                }
                else
                {
                    ret = false;
                }
            }

            db.Close();

            return ret;
        }

        /// <summary>
        ///     引数に与えたjan_codeに該当するレコードを削除
        /// </summary>
        /// <param name="jan_code"></param>
        /// <returns>
        ///     削除成功 -> true, 削除失敗 -> false
        /// </returns>
        public bool Delete_Item(string jan_code)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("DELETE FROM Items_information WHERE @jan_code = jan_code", db);
            command.Parameters.AddWithValue("@jan_code", jan_code);

            try
            {
                command.ExecuteReader();
            }
            catch
            {
                return false;
            }
            finally
            {
                db.Close();
            }

            return true;
        }


        // Item[end] ---------------------------------------------------------------------------------------------

        // TODO:上みたいな感じの関数を複数書く
    }
}
