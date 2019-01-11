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

        // TODO:上みたいな感じの関数を複数書く
    }
}
