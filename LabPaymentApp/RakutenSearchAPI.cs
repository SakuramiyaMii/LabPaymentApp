using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;

/// <summary>
/// コールバック用URL(多分使わない) ttps://webservice.rakuten.co.jp/
/// アプリID 1022536285796999236
/// application_secret 14cffaeb1d1ffeff20148f1bdf1f1dd21a2c624b
/// アフィID 17662581.59cb4511.17662583.a6157867
/// https://app.rakuten.co.jp/services/api/IchibaItem/Search/20170706?format=xml&keyword=4901777300545&applicationId=1022536285796999236
/// </summary>
/// 

/// 注意点
/// 指定するキーワードを空にすると例外発生(調査したほうがいいかもしれない)
/// 指定するキーワードを1文字とかにするとAPIの仕様上正しい結果を返さない

namespace LabPaymentApp
{
    class RakutenSearchAPI
    {
        // アプリケーションID
        static string appID = "1022536285796999236";
        // 取得頻出単語上位件数
        static int GET_NUM = 10;

        /// <summary>
        /// 引数のjanCodeを元に楽天キーワード検索を行い、上位キーワード７個を\n区切りで返す
        /// </summary>
        /// <param name="janCode"></param>
        /// <returns>string "<キーワード>\n<キーワード>\n<キーワード>\n<キーワード>..."</returns>
        public static async Task<string> JAN_Search(string janCode){
            // GETリクエストURL
            string requstUrl = "https://app.rakuten.co.jp/services/api/IchibaItem/Search/20170706?format=xml&keyword=" 
                                 + janCode
                                 + "&applicationId="
                                 + appID;
            // GETリクエスト変数
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requstUrl);
            // メソッドの形式
            req.Method = "GET";
            //リクエストを行い、結果をstringに変換し、responseBodyに格納。
            var res = await req.GetResponseAsync();
            var stream = res.GetResponseStream();
            var reader = new StreamReader(stream);
            var responseBody = await reader.ReadToEndAsync();

            // 切り分け処理
            responseBody = string_split(responseBody);

            return responseBody;
        }

        /// <summary>
        /// 頻出単語解析メソッド
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string string_split(string str){
            // 改行区切りで分割
            string[] buff = Regex.Split(str,"\n");
            // 分割後の格納リスト
            List<string> dic = new List<string>();
            
            // 一行ずつ処理
            foreach (string buf in buff){ 
                // itemNameが含まれている行のみを抽出
                if(buf.Contains("itemName")){
                    // 余計な文字列の削除
                    string bu;
                    bu = buf.Replace("     <itemName>", "");
                    bu = bu.Replace("</itemName>", "");
                    dic.Add(bu);
                }
            }

            // 頻出単語管理辞書
            IDictionary<string, int> wordMap = new Dictionary<string, int>();
            // 格納されている商品名ごとに処理
            foreach(string buf in dic){
                // 商品名を全角空白か半角空白で分割
                string[] words = Regex.Split(buf,"( |　)");
                foreach(string word in words){
                    // 単語が辞書に登録されていなければ登録
                    if (!wordMap.ContainsKey(word)){
                        wordMap.Add(word, 0);
                    }
                    // 単語のカウント
                    wordMap[word]++;
                }
            }
            // 頻出単語の降順ソート
            var mostWords = wordMap.OrderByDescending((x) => x.Value);
            // リターン用変数
            string result = "";
            // カウント変数
            int count = 0;

            // 頻出上位の単語から処理
            foreach(var buf in mostWords){
                // 頻出上位に謎の単語が含まれるのでそれの除外分岐
                if(!(buf.Key == "" || buf.Key == " " || buf.Key == "　")){
                    count++;
                    // 上位何件までを返すか
                    if (count > GET_NUM) break;
                    // 文字列の追記
                    result += buf.Key + "\n";
                }
            }
            return result;
        }
    }
}
