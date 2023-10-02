// See https://aka.ms/new-console-template for more information
//using Login;
using System.Net.Http.Json;
using System.Numerics;
using System.Xml.Linq;
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
            Paint_type_measurement_pots_get_api Paint_type_measurement_pots_get_api = new Paint_type_measurement_pots_get_api();
            Film_thckness_measurement_plan_get_api Film_thckness_measurement_plan_get_api = new Film_thckness_measurement_plan_get_api();
            Film_thckness_measurement_plan_sokuten_change_api Film_thckness_measurement_plan_sokuten_change_api = new Film_thckness_measurement_plan_sokuten_change_api();
            Film_thckness_measurement_add_plan_sokuten_kojo_api Film_thckness_measurement_add_plan_sokuten_kojo_api = new Film_thckness_measurement_add_plan_sokuten_kojo_api();

            await login_api.LoginAsync();
            await search_select_items_api.Search_select_items();
            await Paint_type_measurement_pots_get_api.Paint_type_measurement_pots_get();
            await Film_thckness_measurement_plan_get_api.Film_thckness_measurement_plan_get();
            await Film_thckness_measurement_plan_sokuten_change_api.Film_thckness_measurement_plan_sokuten_change();
            await Film_thckness_measurement_add_plan_sokuten_kojo_api.Film_thckness_measurement_add_plan_sokuten_kojo();
        }
    }


    #region ログイン
    class Login_api
    {
        
        public static HttpClient client = new HttpClient();
        
        public static bool isLogin = false;　//staticを付けることでグローバル変数のような扱いができる
        public static int Koji_id = 0;

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

                if (responseBody?.koj_id != null)
                {
                    // Koji_id = (int)responseBody?.koj_id;
                    Koji_id = responseBody?.koj_id ?? -1; //koj_idがnullだったら-1,null以外だったらkoj_idを代入
                }
                

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
    #endregion

    #region 担当工事取得
    class Search_select_items_api
    {
        public async Task Search_select_items()
        {
            if (Login_api.isLogin) // ログイン中の場合
            {
                // 担当工事取得
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
    #endregion

    #region 塗装系、測定時点取得
    class Paint_type_measurement_pots_get_api
    {
        public async Task Paint_type_measurement_pots_get()
        {
            if (Login_api.isLogin) // ログイン中の場合
            {
                // 塗装系、測定時点取得
                var response = await Login_api.client.PostAsJsonAsync(
                    "ios/makuatsu/search_select_items",
                    new RequestBody_item_set
                    {
                        koj_id = Login_api.Koji_id
                    }
                ); // レスポンスのステータスコードが成功していたら Answer の値を出力


                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("OK2!!!!");
                    var responseBody = await response.Content.ReadFromJsonAsync<Response_item_set>();

                    if (responseBody != null)
                    {
                        if (responseBody?.item_set != null)
                        {
                            if (responseBody?.item_set.paint_types != null)
                            {
                                for (int i = 0; i < responseBody?.item_set.paint_types.Length; i++)
                                {
                                    Console.WriteLine(responseBody?.item_set.paint_types[i].name);
                                } 
                            }
                            if (responseBody?.item_set.measurement_pots != null)
                            {
                                for (int i = 0; i < responseBody?.item_set.measurement_pots.Length; i++)
                                {
                                    Console.WriteLine(responseBody?.item_set.measurement_pots[i].name);
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR2!!!!");
                }
            }

        }

    }

    #endregion

    #region 膜厚測定計画取得
    class Film_thckness_measurement_plan_get_api
    {
        public async Task Film_thckness_measurement_plan_get()
        {
            if (Login_api.isLogin) // ログイン中の場合
            {
                // 膜厚測定計画取得
                var response = await Login_api.client.PostAsJsonAsync(
                    "ios/makuatsu/plans",
                    new RequestBody_plans
                    {
                        koj_id = Login_api.Koji_id
                    }
                ); // レスポンスのステータスコードが成功していたら Answer の値を出力

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("OK3!!!!");
                    var responseBody = await response.Content.ReadFromJsonAsync<Response_plans>();

                    if (responseBody != null)
                    {
                        if (responseBody?.plans != null)
                        {
                            for (int i = 0; i < responseBody?.plans.Length; i++)
                            {
                                Console.WriteLine(responseBody?.plans[i].sokuteijiten_name);

                                if (responseBody?.plans[i].bodies != null)
                                {
                                    // if文でbodiesがnullではない時と言っているのにbodiesが「CS8602 - null 参照の可能性があるものの逆参照です。」とワーニングが出るため!を使用
                                    for (int j = 0; j < responseBody?.plans[j].bodies!.Length; j++)
                                    {
                                        Console.WriteLine(responseBody?.plans[i].bodies![j].shanai_yti_day);

                                        if (responseBody?.plans[i].bodies![j].results_in_company != null)
                                        {
                                            for (int k = 0; k < responseBody?.plans[i].bodies![j].results_in_company!.Length; k++)
                                            {
                                                Console.WriteLine(responseBody?.plans[i].bodies![j].results_in_company![k].no);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR3!!!!");
                }


            }
        }
    }

    #endregion

    #region 膜厚測定結果送信(複数)

    #endregion

    #region 膜厚測定計画測点変更
    class Film_thckness_measurement_plan_sokuten_change_api
    {
        public async Task Film_thckness_measurement_plan_sokuten_change()
        {
            if (Login_api.isLogin) // ログイン中の場合
            {
                var request_data = new data
                {
                    site_id = 27,
                    plan_header_id = 301,
                    plan_body_id = 1502,
                    name = "測点-27-25",
                    is_marker_target = true
                };

                // 膜厚測定計画測点変更
                var response = await Login_api.client.PostAsJsonAsync(
                    "ios/makuatsu/update_sokuten",
                    new RequestBody_data
                    {
                        data = request_data
                    }
                ); // レスポンスのステータスコードが成功していたら Answer の値を出力

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("OK4!!!!");
                    var responseBody = await response.Content.ReadFromJsonAsync<Response_data>();

                    if (responseBody != null)
                    {
                        if (responseBody?.result != null)
                        {
                            Console.WriteLine(responseBody?.result);
                        }
                        if (responseBody?.message != null)
                        {
                            Console.WriteLine(responseBody?.message);
                        }
                    }
                }
                else
                {
                    Console.WriteLine("ERROR4!!!!");
                }
            }
        }
    }


    #endregion

    #region 膜厚測定計画追加(工場モード)
    class Film_thckness_measurement_add_plan_sokuten_kojo_api
    {
        public async Task Film_thckness_measurement_add_plan_sokuten_kojo()
        {
            if (Login_api.isLogin) // ログイン中の場合
            {
                var request_item_header = new item_header
                {
                    ten_id = 202,   // きっと工場モードでは要らない
                    koj_id = 50,    // 新宿駅前歩道橋工事(工事名略称：テスト現場-24)
                    tosoukei_id = 0,    // 指定
                    sokuteijiten_id = 0,    // 指定
                    lot_no = 2, // 塗料のロット番号、1ロットあたり1点5箇所×5点=25箇所計測
                    sokutei_flg = false,    // 指定
                    tomakuatsu = 2002,  // 塗膜厚を計測した値
                };

                var request_item_body = new item_body
                {
                    sokutei_plan_header_id = 0, // 指定
                    skt_id = 0,
                    shanai_yti_day = "2023-10-01",
                    hinsitsu_yti_day = "2023-10-01",
                    tachiai_yti_day = "2023-10-01",
                    sokutei_flg = false // 指定
                };

                var request_tosoukei_new_item = new tosoukei_new_item
                {
                    name = "塗装系名テスト5"
                };

                var request_sokuteijiten_new_item = new sokuteijiten_new_item
                {
                    name = "測定時点名テスト5"
                };

                var request_tomakuatsu_new_item = new tomakuatsu_new_item
                {
                    name = "2023"
                };

                var request_sokuten_new_item = new sokuten_new_item
                {
                    name = "測点名テスト5"
                };

                // 膜厚測定計画追加(工場モード)
                var response = await Login_api.client.PostAsJsonAsync(
                    "works/apis/sokutei_plan/store_plant",
                    new RequestBody_add_plan
                    {
                        item_header = request_item_header,
                        item_body = request_item_body,
                        plant_id = 4,
                        tosoukei_new_item = request_tosoukei_new_item,
                        sokuteijiten_new_item = request_sokuteijiten_new_item,
                        tomakuatsu_new_item = request_tomakuatsu_new_item,
                        sokuten_new_item = request_sokuten_new_item
                    }
                ); // レスポンスのステータスコードが成功していたら Answer の値を出力

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("OK5!!!!");
                }
                else
                {
                    Console.WriteLine("ERROR5!!!!");
                }
            }
        }
    }

    #endregion


    #region ログイン　リクエスト・レスポンス
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
    #endregion

    #region 担当工事取得　リクエスト・レスポンス
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
    #endregion

    #region 塗装系、測定時点取得　リクエスト・レスポンス
    class RequestBody_item_set
    {
        public int? koj_id { get; set; }
    }

    class Response_item_set
    {
        public item_set? item_set { get; set; }
    }

    class item_set
    {
        public paint_types[]? paint_types { get; set; }
        public measurement_pots[]? measurement_pots { get; set; }

    }

    class paint_types
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public int? site_id { get; set; }
        public int? plant_id { get; set; }
    }

    class measurement_pots
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public int? site_id { get; set; }
        public int? plant_id { get; set; }
    }

    #endregion

    #region 膜厚測定計画取得 リクエスト・レスポンス
    class RequestBody_plans
    {
        public int? koj_id { get; set; }
    }

    class Response_plans
    {
        public plans[]? plans { get; set; }
    }

    class plans
    {
        public int? id { get; set; }
        public int? site_id { get; set; }
        public int? tosoukei_id { get; set; }
        public string? tosoukei_name{ get; set; }
        public int? sokuteijiten_id { get; set; }
        public string? sokuteijiten_name { get; set; }
        public int? lot_no { get; set; }
        public int? tomakuatsu_id { get; set; }
        public int? tomakuatsu { get; set; }
        public bool? sokutei_flg { get; set; }
        public bodies[]? bodies { get; set; }
    }

    class bodies
    {
        public int? id { get; set; }
        public int? header_id { get; set; }
        public int? sokuten_id { get; set; }
        public string? sokuten_name { get; set; }
        public bool? is_marker_target { get; set; }
        public int? ccid { get; set; }
        public string? shanai_yti_day { get; set; }
        public string? hinsitsu_yti_day { get; set; }
        public string? tachiai_yti_day { get; set; }
        public bool? sokutei_flg { get; set; }
        public bool? hinsitsu_sokutei_flg { get; set; }
        public bool? tachiai_sokutei_flg { get; set; }
        public double? last_average_result { get; set; }
        public double? last_minimum_result { get; set; }
        public double? last_std_dev_result { get; set; }
        public double? hinsitsu_last_average_result { get; set; }
        public double? hinsitsu_last_minimum_result { get; set; }
        public double? hinsitsu_last_std_dev_result { get; set; }
        public double? tachiai_last_average_result { get; set; }
        public double? tachiai_last_minimum_result { get; set; }
        public double? tachiai_last_std_dev_result { get; set; }
        public results_in_company[]? results_in_company { get; set; }
    }

    class results_in_company
    {
        public int? no { get; set; }
        public double? result { get; set; }
    }

    #endregion

    #region 膜厚測定結果送信(複数)　リクエスト・レスポンス
    class RequestBody_result_datas
    {
        public result_datas[]? plans { get; set; }
    }

    class result_datas
    {
        public int? site_id { get; set; }
        public int? plan_header_id { get; set; }
        public int? plan_body_id { get; set; }
        public working_datas[]? working_datas  { get; set; }
        public string? sokuteisha_type { get; set; }
        public string? sokuteisha_name { get; set; }
        public string? note { get; set; }
        public string? tempertature { get; set; }
        public string? humidity { get; set; }
        public string? weather { get; set; }
        public string? signature { get; set; }
        public string? created_at { get; set; }
    }

    class working_datas
    {
        public int? no { get; set; }
        public string? updated_at { get; set; }
        public double? result { get; set; }
        public bool? is_use { get; set; }
    }


    class Response_result_datas
    {
        public bool? result { get; set; }
        public string? message {  get; set; }
    }

    #endregion

    #region 膜厚測定計画測点変更　リクエスト・レスポンス
    class RequestBody_data
    {
        public data? data { get; set; }
    }

    class data
    {
        public int? site_id { get; set; }
        public int? plan_header_id { get; set; }
        public int? plan_body_id { get; set; }
        public string? name { get; set; }
        public bool? is_marker_target { get; set; }
    }

    class Response_data
    {
        public bool? result { get; set; }
        public string? message { get; set; }
    }

    #endregion

    #region 膜厚測定計画追加(工場モード)　リクエスト・レスポンス
    class RequestBody_add_plan
    {
        public item_header? item_header { get; set; }
        public item_body? item_body { get; set; }
        public int? plant_id { get; set; }
        public tosoukei_new_item? tosoukei_new_item { get; set; }
        public sokuteijiten_new_item? sokuteijiten_new_item { get; set; }
        public tomakuatsu_new_item? tomakuatsu_new_item { get; set; }
        public sokuten_new_item? sokuten_new_item { get; set; }
    }

    class item_header
    {
        public int? ten_id { get; set; }
        public int? koj_id { get; set; }
        public int? tosoukei_id { get; set; }
        public int? sokuteijiten_id { get; set; }
        public int? lot_no { get; set; }
        public bool? sokutei_flg { get; set; }
        public int? tomakuatsu { get; set; }
    }

    class item_body
    {
        public int? sokutei_plan_header_id { get; set; }
        public int? skt_id { get; set; }
        public string? shanai_yti_day { get; set; }
        public string? hinsitsu_yti_day { get; set; }
        public string? tachiai_yti_day { get; set; }
        public bool? sokutei_flg { get; set; }
    }

    class tosoukei_new_item
    {
        public string? name { get; set; }
    }

    class sokuteijiten_new_item
    {
        public string? name { get; set; }
    }

    class tomakuatsu_new_item
    {
        public string? name { get; set; }
    }

    class sokuten_new_item
    {
        public string? name { get; set; }
    }

    #endregion






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