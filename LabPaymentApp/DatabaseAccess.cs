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
        ///     users_infomationデータベースから指定したmidのユーザ情報を取得
        /// </summary>
        /// <returns>
        ///     UsersInfomation : UsersInfomationクラスを参照
        /// </returns>
        public UsersInformation Get_UserInformation(string mid)
        {
            if (!Search_UserInformation(mid)) return new UsersInformation(mid,"削除ユーザー",-1,"利用者");
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT * from users_information where @mid = mid", db);
            command.Parameters.AddWithValue("@mid", mid);

            UsersInformation ret = new UsersInformation();
            using (SqliteDataReader query = command.ExecuteReader())
            {
                query.Read();
                ret._mid = query.GetString(0);
                ret._balance = query.GetInt32(1);
                ret._user_name = query.GetString(2);
                ret._permission = query.GetString(3);
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

        /// <summary>
        ///     UserInformationテーブルに引数として与えるmidの持つbalanceが引数price以上あるかどうかチェック
        /// </summary>
        /// <param name="mid"> チェック対象のmid </param>
        /// <param name="price"> リクエスト金額 </param>
        /// <returns>
        ///     検索結果 決済可能->true,決済不可->false
        /// </returns>
        public bool Check_Payment(string mid, int price)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT count(*) from users_information where @mid = mid AND balance >= @price", db);
            command.Parameters.AddWithValue("@mid", mid);
            command.Parameters.AddWithValue("@price", price);

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
        ///     UserInformationテーブルに引数として与えるmidの持つbalanceからpriceを減算
        /// </summary>
        /// <param name="mid"> チェック対象のmid </param>
        /// <param name="price"> リクエスト金額 </param>
        /// <returns>
        ///     検索結果 決済成功->true,決済失敗->false
        /// </returns>
        public bool Exec_Payment(string mid, int price)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("UPDATE users_information SET balance = balance - @price where @mid = mid AND balance >= @price", db);
            command.Parameters.AddWithValue("@mid", mid);
            command.Parameters.AddWithValue("@price", price);

            try{
                SqliteDataReader query = command.ExecuteReader();
                db.Close();
                return true;
            }
            catch{
                db.Close();
                return false;
            }
        }

        /// <summary>
        ///     UserInformationテーブルに引数として与えるmidの持つbalanceにpriceを加算
        /// </summary>
        /// <param name="mid"> チェック対象のmid </param>
        /// <param name="price"> リクエスト金額 </param>
        /// <returns>
        ///     検索結果 決済成功->true,決済失敗->false
        /// </returns>
        public bool Exec_Charge(string mid, int price)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("UPDATE users_information SET balance = balance + @price where @mid = mid", db);
            command.Parameters.AddWithValue("@mid", mid);
            command.Parameters.AddWithValue("@price", price);

            try
            {
                SqliteDataReader query = command.ExecuteReader();
                db.Close();
                return true;
            }
            catch
            {
                db.Close();
                return false;
            }
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
            if (!Search_Item(jan_code)) return new Item(jan_code,"削除商品",0,0,0);
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

        /// <summary>
        ///     Item_informationテーブルに引数として与えるjancodeのquantityを引数numだけ減算する
        /// </summary>
        /// <param name="jan"> JANコード </param>
        /// <param name="num"> リクエスト個数 </param>
        /// <returns>
        ///     検索結果 決済成功->true,決済失敗->false
        /// </returns>
        public bool Reduce_Item(string jan, int num)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("UPDATE Items_information SET quantity = quantity - @num where @jan = jan_code AND quantity >= @num", db);
            command.Parameters.AddWithValue("@jan", jan);
            command.Parameters.AddWithValue("@num", num);

            try
            {
                SqliteDataReader query = command.ExecuteReader();
                db.Close();
                return true;
            }
            catch
            {
                db.Close();
                return false;
            }
        }

        // Item[end] ---------------------------------------------------------------------------------------------


        // Log ---------------------------------------------------------------------------------------------------

        /// <summary>
        ///     all_logテーブルにログを追加
        /// </summary>
        /// <param name="mid"> ログ発生者のmid </param>
        /// <param name="type"> 操作種別 </param>
        /// <returns>
        ///     成功 -> true , 失敗 -> false
        /// </returns>
        private bool Insert_All_Log(string type,string mid)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("INSERT INTO all_log(created_at,mid,operation_type) VALUES(datetime('now','localtime'),@mid,@str)", db);
            command.Parameters.AddWithValue("@mid", mid);
            command.Parameters.AddWithValue("@str", type);
            
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
        ///     purchases_logテーブルにログを追加
        /// </summary>
        /// <param name="mid"> ログ発生者のmid </param>
        /// <param name="jan"> 購入商品のJANコード </param>
        /// <param name="num"> 購入商品の個数 </param>
        /// <param name="price"> 購入商品の単価 </param>
        /// <returns>
        ///     成功 -> true , 失敗 -> false
        /// </returns>
        public bool Insert_Purchase_Log(string mid,string jan,int num,int price)
        {
            Insert_All_Log("購入",mid);
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("INSERT INTO purchases_log(operation_id,jan_code,quantity,price) VALUES((SELECT max(operation_id) from all_log),@jan,@num,@price)", db);
            command.Parameters.AddWithValue("@jan", jan);
            command.Parameters.AddWithValue("@num", num);
            command.Parameters.AddWithValue("@price", price);

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
        ///     charges_logテーブルにログを追加
        /// </summary>
        /// <param name="mid"> ログ発生者のmid </param>
        /// <param name="price"> チャージ額 </param>
        /// <returns>
        ///     成功 -> true , 失敗 -> false
        /// </returns>
        public bool Insert_Charge_Log(string mid, int price)
        {
            if (price <= 0)
            {
                Insert_All_Log("仕入れ", mid);
            }
            else
            {
                Insert_All_Log("チャージ", mid);
            }
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("INSERT INTO charges_log(operation_id,price) VALUES((SELECT max(operation_id) from all_log),@price)", db);
            command.Parameters.AddWithValue("@price", price);

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
        ///     operations_logテーブルにログを追加
        /// </summary>
        /// <param name="mid"> ログ発生者のmid </param>
        /// <param name="str"> 操作内容 </param>
        /// <returns>
        ///     成功 -> true , 失敗 -> false
        /// </returns>
        public bool Insert_Operation_Log(string mid, string str)
        {
            Insert_All_Log("DB操作", mid);
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("INSERT INTO operations_log(operation_id,operation_detail) VALUES((SELECT max(operation_id) from all_log),@str)", db);
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

        /// <summary>
        ///     all_logデータベースから全ての購入・チャージレコードを取得
        /// </summary>
        /// <returns>
        ///     List<PaymentLog> : PaymentLogクラスを参照
        /// </returns>
        public List<PaymentLog> Get_AllPayment()
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT * from all_log where operation_type = '購入' OR operation_type = 'チャージ' OR operation_type = '仕入れ'", db);

            List<PaymentLog> ret = new List<PaymentLog>();
            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    PaymentLog pl = new PaymentLog();
                    pl._id = query.GetInt32(0);
                    pl._date = query.GetString(1);
                    pl._mid = query.GetString(2);
                    pl._type = query.GetString(3);

                    ret.Add(pl);
                }
            }
            foreach (PaymentLog pl in ret) {
                // user_name 取得
                if (Search_UserInformation(pl._mid))
                {
                    command = new SqliteCommand("SELECT * from users_information where @mid = mid", db);
                    command.Parameters.AddWithValue("@mid", pl._mid);
                    using (SqliteDataReader query = command.ExecuteReader())
                    {
                        query.Read();
                        pl._user_name = query.GetString(2);
                    }
                }else{
                    pl._user_name = "削除ユーザー";
                }

                if(pl._type == "購入"){
                    command = new SqliteCommand("SELECT * from purchases_log where @id = operation_id", db);
                    command.Parameters.AddWithValue("@id", pl._id);
                    using (SqliteDataReader query = command.ExecuteReader())
                    {
                        query.Read();
                        pl._janCode = query.GetString(1);
                        pl._num = query.GetInt32(2);
                        pl._price = query.GetInt32(3);
                        pl._total_price = pl._num * pl._price;
                    }

                    if (Search_Item(pl._janCode))
                    {
                        command = new SqliteCommand("SELECT * from Items_information where @janCode = jan_code", db);
                        command.Parameters.AddWithValue("@janCode", pl._janCode);
                        using (SqliteDataReader query = command.ExecuteReader())
                        {
                            query.Read();
                            pl._itemName = query.GetString(1);
                        }
                    }else{
                        pl._itemName = "削除商品";
                    }

                }
                else if(pl._type == "チャージ"|| pl._type == "仕入れ")
                {
                    command = new SqliteCommand("SELECT * from charges_log where @id = operation_id", db);
                    command.Parameters.AddWithValue("@id", pl._id);
                    using (SqliteDataReader query = command.ExecuteReader())
                    {
                        query.Read();
                        pl._total_price = query.GetInt32(1);
                    }
                }

            }
            db.Close();

            return ret;
        }

        /// <summary>
        ///     all_logデータベースから全てのDB操作レコードを取得
        /// </summary>
        /// <returns>
        ///     List<OperationLos> : OperationLogクラスを参照
        /// </returns>
        public List<OperationLog> Get_AllOperation()
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT * from all_log where operation_type = 'DB操作'", db);

            List<OperationLog> ret = new List<OperationLog>();
            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    OperationLog ol = new OperationLog();
                    ol._id = query.GetInt32(0);
                    ol._date = query.GetString(1);
                    ol._mid = query.GetString(2);

                    ret.Add(ol);
                }
            }
            foreach (OperationLog ol in ret)
            {
                // user_name 取得
                if (Search_UserInformation(ol._mid))
                {
                    command = new SqliteCommand("SELECT * from users_information where @mid = mid", db);
                    command.Parameters.AddWithValue("@mid", ol._mid);
                    using (SqliteDataReader query = command.ExecuteReader())
                    {
                        query.Read();
                        ol._user_name = query.GetString(2);
                    }
                }else{
                    ol._user_name = "削除ユーザー";
                }

                // DB操作内容取得
                {
                    command = new SqliteCommand("SELECT * from operations_log where @id = operation_id", db);
                    command.Parameters.AddWithValue("@id", ol._id);
                    using (SqliteDataReader query = command.ExecuteReader())
                    {
                        query.Read();
                        ol._detail = query.GetString(1);
                    }
                }
            }
            db.Close();

            return ret;
        }

        /// <summary>
        ///     all_logテーブルとpurchases_logテーブルから指定したjancodeの売上日時リストを取得する
        /// </summary>
        /// <returns>
        ///     DateTime : 売上日時のリスト
        /// </returns>
        public DateTime[] Get_AllSellingDate(string janCode)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT created_at FROM all_log inner join purchases_log ON all_log.operation_id = purchases_log.operation_id WHERE purchases_log.jan_code = @janCode", db);
            command.Parameters.AddWithValue("@janCode", janCode);

            DateTime[] ret = new DateTime[0];

            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    if (query.GetString(0) != null)
                    {
                        string[] date_1 = query.GetString(0).Split(' ');
                        string[] date_2 = date_1[0].Split('-');
                        string[] date_3 = date_1[1].Split(':');
                        // date 0:西暦,1:月,2:日,3:時,4:分,5:秒 
                        int[] date = { int.Parse(date_2[0]), int.Parse(date_2[1]), int.Parse(date_2[2]), int.Parse(date_3[0]), int.Parse(date_3[1]), int.Parse(date_3[2]) };


                        Array.Resize(ref ret, ret.Length + 1);
                        ret[ret.Length - 1] = new DateTime(date[0], date[1], date[2], date[3], date[4], date[5]);
                    }else{

                    }
                }
            }

            db.Close();

            return ret;
        }

        /// <summary>
        ///     all_logテーブルとoperations_logテーブルから指定したjancodeの仕入れ日時リストを取得する
        /// </summary>
        /// <returns>
        ///     DateTime : 仕入れ日時のリスト
        /// </returns>
        public DateTime[] Get_AllStockingDate(string janCode)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT created_at FROM all_log inner join operations_log ON all_log.operation_id = operations_log.operation_id WHERE (operations_log.operation_detail like @janCode1 OR operations_log.operation_detail like @janCode2)", db);
            command.Parameters.AddWithValue("@janCode1", "商品登録%"+janCode+'%');
            command.Parameters.AddWithValue("@janCode2", "商品在庫更新%" + janCode + '%');

            DateTime[] ret = new DateTime[0];

            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    if (query.GetString(0) != null)
                    {
                        string[] date_1 = query.GetString(0).Split(' ');
                        string[] date_2 = date_1[0].Split('-');
                        string[] date_3 = date_1[1].Split(':');
                        // date 0:西暦,1:月,2:日,3:時,4:分,5:秒 
                        int[] date = { int.Parse(date_2[0]), int.Parse(date_2[1]), int.Parse(date_2[2]), int.Parse(date_3[0]), int.Parse(date_3[1]), int.Parse(date_3[2]) };


                        Array.Resize(ref ret, ret.Length + 1);
                        ret[ret.Length - 1] = new DateTime(date[0], date[1], date[2], date[3], date[4], date[5]);
                    }
                    else
                    {

                    }
                }
            }

            db.Close();

            return ret;
        }

        /// <summary>
        ///     all_logテーブルとoperations_logテーブルから指定したjancodeの累計仕入れ個数リストを取得する
        /// </summary>
        /// <returns>
        ///     int[] : 仕入れ個数の累計リスト
        /// </returns>
        public int[] Get_AllStockingHistory(string janCode)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT operation_detail FROM all_log inner join operations_log ON all_log.operation_id = operations_log.operation_id WHERE (operations_log.operation_detail like @janCode1 OR operations_log.operation_detail like @janCode2)", db);
            command.Parameters.AddWithValue("@janCode1", "商品登録%" + janCode + '%');
            command.Parameters.AddWithValue("@janCode2", "商品在庫更新%" + janCode + '%');

            int[] ret = new int[0];
            using (SqliteDataReader query = command.ExecuteReader())
            {
                while (query.Read())
                {
                    if (query.GetString(0) != null)
                    {
                        string[] splitbuf = query.GetString(0).Split('=');
                        string[] str = splitbuf[splitbuf.Length - 1].Split("→");
                        string num = "", num1 = "";
                        
                        for(int i = 0;i <= str[str.Length - 1].Length - 1; i++){
                            if(str[str.Length - 1][i] == '0' || str[str.Length - 1][i] == '1' || str[str.Length - 1][i] == '2' || str[str.Length - 1][i] == '3' || str[str.Length - 1][i] == '4' || str[str.Length - 1][i] == '5' || str[str.Length - 1][i] == '6' || str[str.Length - 1][i] == '7' || str[str.Length - 1][i] == '8' || str[str.Length - 1][i] == '9'){
                                num += str[str.Length - 1][i];
                            }
                        }
                        if(str.Length > 1){
                            for (int i = 0; i <= str[str.Length - 2].Length - 1; i++)
                            {
                                if (str[str.Length - 2][i] == '0' || str[str.Length - 2][i] == '1' || str[str.Length - 2][i] == '2' || str[str.Length - 2][i] == '3' || str[str.Length - 2][i] == '4' || str[str.Length - 2][i] == '5' || str[str.Length - 2][i] == '6' || str[str.Length - 2][i] == '7' || str[str.Length - 2][i] == '8' || str[str.Length - 2][i] == '9')
                                {
                                    num1 += str[str.Length - 2][i];
                                }
                            }
                        }

                        int ans = 0;
                        if(num1 != ""){
                            ans = int.Parse(num) - int.Parse(num1);
                        }else{
                            ans = int.Parse(num);
                        }


                        Array.Resize(ref ret, ret.Length + 1);
                        ret[ret.Length - 1] = ans;
                    }
                    else
                    {

                    }
                }
            }

            db.Close();

            return ret;
        }
        // Log[end] ----------------------------------------------------------------------------------------------

        // Default -----------------------------------------------------------------------------------------------

        /// <summary>
        ///     default_balanceテーブルに引数として与えるbalanceで更新する
        /// </summary>
        /// <param name="balance"> 設定金額 </param>
        /// <returns>
        ///     検索結果 決済成功->true,決済失敗->false
        /// </returns>
        public bool Set_Balance(int balance)
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("UPDATE default_balance SET balance = @balance", db);
            command.Parameters.AddWithValue("@balance", balance);

            try
            {
                SqliteDataReader query = command.ExecuteReader();
                db.Close();
                return true;
            }
            catch
            {
                db.Close();
                return false;
            }
        }

        /// <summary>
        ///     default_balanceテーブルからbalanceを取得する
        /// </summary>
        /// <returns>
        ///     初期金額
        /// </returns>
        public int Get_Balance()
        {
            SqliteConnection db = this.OpenDB();

            SqliteCommand command = new SqliteCommand("SELECT * from default_balance", db);

            int ret;

            using (SqliteDataReader query = command.ExecuteReader())
            {
                query.Read();
                ret = query.GetInt32(0);
            }

            db.Close();

            return ret;
        }

        // Default[End] ------------------------------------------------------------------------------------------
        // TODO:上みたいな感じの関数を複数書く
    }
}
