using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabPaymentApp
{
    class Database
    {
        // コンストラクタ
        Database()
        {
            
        }

        // データベースオープン処理
        public SqliteConnection openDB()
        {
            // SQliteデータベース初期化
            // .dbを置くフォルダは C:/Users/{UserName}/AppData/Local/Packages/72458304-e57a-4f5a-8c1e-136be572b2b9/LocalState
            // 参考資料：https://stackoverflow.com/questions/36899829/sqlite-database-location-uwp
            string db_Name = "database.db"; //データベースファイル名
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

        // データベースクローズ処理
        public void closeDB(SqliteConnection db)
        {
            db.Close();
        }
    }
}
