using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

/// <summary>
/// コールバック用URL(多分使わない) ttps://webservice.rakuten.co.jp/
/// アプリID 1022536285796999236
/// application_secret 14cffaeb1d1ffeff20148f1bdf1f1dd21a2c624b
/// アフィID 17662581.59cb4511.17662583.a6157867
/// https://app.rakuten.co.jp/services/api/IchibaItem/Search/20170706?format=xml&keyword=4901777300545&applicationId=1022536285796999236
/// </summary>
/// 

namespace LabPaymentApp
{
    class RakutenSearchAPI
    {
        // アプリケーションID
        static string appID = "1022536285796999236";

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
            //リクエストを行い、結果をresに格納。
            var res = await req.GetResponseAsync();
            var stream = res.GetResponseStream();
            var reader = new StreamReader(stream);
            var responseBody = await reader.ReadToEndAsync();
            responseBody = string_split(responseBody);

            return responseBody;
        }
        public static string string_split(string str){
            string[] buff = Regex.Split(str,"\n");
            List<string> dic = new List<string>();
            foreach (string buf in buff){ 
                if(buf.Contains("itemName")){
                    string bu;
                    bu = buf.Replace("     <itemName>", "");
                    bu = bu.Replace("</itemName>", "");
                    dic.Add(bu);
                }
            }
            IDictionary<string, int> wordMap = new Dictionary<string, int>();
            foreach(string buf in dic){
                string[] words = Regex.Split(buf,"( |　)");
                foreach(string word in words){
                    if (!wordMap.ContainsKey(word)){
                        wordMap.Add(word, 0);
                    }
                    wordMap[word]++;
                }
            }
            var mostWords = wordMap.OrderByDescending((x) => x.Value);
            string result = "";
            int count = 0;
            foreach(var buf in mostWords){
                if(!(buf.Key == "" || buf.Key == " " || buf.Key == "　")){
                    count++;
                    if (count == 8) break;
                    result += buf.Key + "\n";
                }
            }
            return result;
        }
    }
}
