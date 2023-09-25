// See https://aka.ms/new-console-template for more information
//using Login;
using System.Net.Http.Json;
//using Web_api;

Console.WriteLine("Hello, World!");

Web_api.main mainyobidashi = new Web_api.main();
await mainyobidashi.Main();




#region API
namespace Web_api
{
    public class main
    {
        public async Task Main()
        {
            Login_api login_api = new Login_api();
            Search_select_items_api search_select_items_api = new Search_select_items_api();

            await login_api.LoginAsync();
            await search_select_items_api.Search_select_items();
        }
    }
    

    class Login_api
    {
        
        public static HttpClient client = new HttpClient();
        
        public static bool isLogin = false;　//staticを付けることでグローバル変数のような扱いができる

        public async Task LoginAsync()
        {
            
            client.BaseAddress = new Uri("http://ec2-52-192-250-220.ap-northeast-1.compute.amazonaws.com/");

            // Web API を呼ぶための HttpClient を作る
            // var client = new HttpClient();


            // POST メソッドで JSON の Body のリクエストを投げる
            var response = await client.PostAsJsonAsync(
                " ios/login", //Login API
                new RequestBody
                {
                    username = " k-user-1",
                    password = " k-password-1"
                }
            ); // レスポンスのステータスコードが成功していたら Answer の値を出力

            if (response.IsSuccessStatusCode) // リクエストの結果判定(ログインが成功)
            {
                var responseBody = await response.Content.ReadFromJsonAsync<Response_login>();
                Console.WriteLine(responseBody?.result);
                Console.WriteLine(responseBody?.token);
                Console.WriteLine(responseBody?.webtoken);
                Console.WriteLine(responseBody?.ios_plant_flg);
                Console.WriteLine(responseBody?.is_admin);
                Console.WriteLine(responseBody?.is_plant);
                Console.WriteLine(responseBody?.koj_id);
                Console.WriteLine(responseBody?.plant_id);

                //リクエストヘッダにトークンをセット
                client.DefaultRequestHeaders.Add("Authorization", responseBody?.token);

                isLogin = true;
            }
            else
            {
                isLogin = false;
            }
            
        }
    }

    class Search_select_items_api
    {
        public async Task Search_select_items()
        {
            if (Login_api.isLogin) // ログイン中の場合
            {
                //担当工事取得
                var response = await Login_api.client.PostAsync(
                    "masters/apis/kouji/tnt_koujis", null);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("OK!!!!");
                    var responseBody = await response.Content.ReadFromJsonAsync<Response_koujis>();
                    if (responseBody != null)
                    {
                        if (responseBody?.koujis != null)
                        {
                            for (int i = 0; i < responseBody?.koujis.Length; i++)
                                Console.WriteLine(responseBody?.koujis[i].name);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR!!!!");
                }



            }
        }
    }

    // リクエストとレスポンス
    class RequestBody
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }

    class Response_login
    {
        public string? result { get; set; }
        public string? token { get; set; }
        public string? webtoken { get; set; }
        public bool? ios_plant_flg { get; set; }
        public bool? is_admin { get; set; }
        public bool? is_plant { get; set; }
        public int? koj_id { get; set; }
        public int? plant_id { get; set; }
    }

    class Response_koujis
    {
        public koujis[]? koujis { get; set; }
    }

    class koujis
    {
        public int? id { get; set; }
        public int? ten_id { get; set; }
        public string? name { get; set; }
        public int? sit_id { get; set; }
        public string? kouban { get; set; }
        public string? start_day { get; set; }
        public string? end_day { get; set; }
        public string? orderer { get; set; }
        public int? loc_id { get; set; }
        public string? kura_domain { get; set; }
        public string? api_key { get; set; }
        public string? company { get; set; }
        public string? kouji_type { get; set; }
        public int? cals_id { get; set; }
        public string? api_key2 { get; set; }
        public string? simple_name { get; set; }
        public int? ccid { get; set; }
        public int? plant_id { get; set; }
        public bool? edaban_flg { get; set; }
        public bool? is_closed { get; set; }
        public string? kaigyo_name { get; set; }
        public int? kanri_no_id { get; set; }
        public int? ent_tnt_id { get; set; }
        public string? ent_at { get; set; }
        public int? upd_tnt_id { get; set; }
        public string? upd_at { get; set; }
    }
}

#endregion



//#region 工事情報取得

//namespace Search_select_items
//{
//    class Search_select_items_api
//    {
//        private async Task Search_select_items()
//        {
//            Response login_response = new Response();

//            // POST メソッドで JSON の Body のリクエストを投げる
//            var response = await Login.Login_api.client.PostAsJsonAsync(
//                " http://ec2-52-192-250-220.ap-northeast-1.compute.amazonaws.com/ios/makuatsu/search_select_items", //塗装系、測定時点取得 API
//                new RequestBody
//                {
                    
//                }
//            ); // レスポンスのステータスコードが成功していたら Answer の値を出力
//        }
//    }

//    // リクエストとレスポンス
//    class RequestBody
//    {
//        public int? koj_id { get; set; }
//    }

//    class Response
//    {
        
//    }

//}

//#endregion