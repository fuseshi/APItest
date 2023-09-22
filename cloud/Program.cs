// See https://aka.ms/new-console-template for more information
using System.Net.Http.Json;

Console.WriteLine("Hello, World!");



#region 池田課長プログラム 20230921

// Web API を呼ぶための HttpClient を作る
var client = new HttpClient();

// POST メソッドで JSON の Body のリクエストを投げる
var response = await client.PostAsJsonAsync(
    " http://ec2-52-192-250-220.ap-northeast-1.compute.amazonaws.com/ios/login", //Login API
    new RequestBody
    {
        username = " k-user-1",
        password = " k-password-1"
    }
); // レスポンスのステータスコードが成功していたら Answer の値を出力

if (response.IsSuccessStatusCode) // リクエストの結果判定
{
    var responseBody = await response.Content.ReadFromJsonAsync<Response>();
    Console.WriteLine(responseBody?.result);
}



// リクエストとレスポンス
class RequestBody
{
    public string? username { get; set; }
    public string? password { get; set; }
}

class Response
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