using System.Net;
using System.Text;

public static class MJApi
{

    public delegate void RequestCallBack(WebExceptionStatus status, string result);
    public static RequestCallBack _callback;

    private static string serverUrl = "https://192.168.22.19:8000/";
    private static string secretKey = "KQgZFQFLWL0qyRjCbgpEIYUhjYjmZOvbywbdGABb46cGzeevCMQU2LXvornsNkScfeCS9BmZ0KkebfYTvgvfLwUpl0QjR4LL5hHOYzaHxGQcVfvvY2wtiPRRMxGqhxVq";

    public static void Login(string sType, string mail, string tnPass, RequestCallBack callback)
    {
        string api = "V1/login";
        string auth = "Bearer " + secretKey;
        string method = "POST";
        if(sType == "C1")
            mail = StringToUnicode(mail);
        string pdata = "[{\"Type\":\"" + sType + "\", \"Mail\":\"" + mail + "\", \"TnPass\":\"" + tnPass + "\"}]";
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    private static string StringToUnicode(string srcText)
    {
        string dst = "";
        char[] src = srcText.ToCharArray();
        for (int i = 0; i < src.Length; i++)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(src[i].ToString());
            string str = @"\u" + bytes[1].ToString("X2") + bytes[0].ToString("X2");
            dst += str;
        }
        return dst;
    }

    private static string UnicodeToString(string srcText)
    {
        string dst = "";
        string src = srcText;
        int len = srcText.Length / 6;

        for (int i = 0; i <= len - 1; i++)
        {
            string str = "";
            str = src.Substring(0, 6).Substring(2);
            src = src.Substring(6);
            byte[] bytes = new byte[2];
            bytes[1] = byte.Parse(int.Parse(str.Substring(0, 2), System.Globalization.NumberStyles.HexNumber).ToString());
            bytes[0] = byte.Parse(int.Parse(str.Substring(2, 2), System.Globalization.NumberStyles.HexNumber).ToString());
            dst += Encoding.Unicode.GetString(bytes);
        }
        return dst;
    }

    public static void AddMember(string id, string mail, string pass, string name, string stype, RequestCallBack callback)
    {
        string api = "V1/addMember";
        string auth = "Bearer " + secretKey;
        string method = "POST";
        name = StringToUnicode(name);
        string pdata = "[{\"ID\":\"" + id + "\",\"Mail\":\"" + mail + "\", \"Pass\":\"" + pass + "\", \"Name\":\"" + name + "\", \"SType\":\"" + stype + "\"}]";
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    public static void setForgetPwd(string name, string pass, string nick_name, string code, RequestCallBack callback)
    {
        string api = "V1/setForgetPwd";
        string auth = "Bearer " + secretKey;
        string method = "POST";
        string pdata = "[{\"Name\":\"" + name + "\", \"Pass\":\"" + pass + "\", \"NickName\":\"" + nick_name + "\", \"Code\":\"" + code + "\"}]";
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    public static void getAuthCode(string name, RequestCallBack callback)
    {
        string api = "V1/getAuthCode";
        string auth = "Bearer " + secretKey;
        string method = "POST";
        string pdata = "[{\"Name\":\"" + name + "\"}]";
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    public static void setTaskStatus(string token, string name, string taskStatus, RequestCallBack callback)
    {
        string api = "V1/setTaskStatus";
        string auth = "Bearer " + name + ":" + token;
        string method = "PUT";
        string pdata = "[{\"TaskStatus\":\"" + taskStatus + "\"}]";
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    public static void setPlayer(string token, string name, string pdata, RequestCallBack callback)
    {
        string api = "V1/setPlayer";
        string auth = "Bearer " + name + ":" + token;
        string method = "PUT";
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    public static void getPlayer(string token, string name, RequestCallBack callback)
    {
        string api = "V1/getPlayer";
        string auth = "Bearer " + name + ":" + token;
        string method = "GET";
        string pdata = string.Empty;
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    public static void setBPItemNum(string token, string name, string itemNum, RequestCallBack callback)
    {
        string api = "V1/setBPItemNum";
        string auth = "Bearer " + name + ":" + token;
        string method = "PUT";
        string pdata = "[" + itemNum + "]";
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    public static void setBPItemNum(string token, string name, int id, int num, RequestCallBack callback)
    {
        string api = "V1/setOneBPItemNum";
        string auth = "Bearer " + name + ":" + token;
        string method = "PUT";
        string pdata = "[{\"Id\":"+ id +",\"Num\":" +  num +"}]";
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }
    
    public static void getBPItemNum(string token, string name, RequestCallBack callback)
    {
        string api = "V1/getBPItemNum";
        string auth = "Bearer " + name + ":" + token;
        string method = "GET";
        string pdata = string.Empty;
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

    public static void getBPItemNum(string token, string name, int id, RequestCallBack callback)
    {
        string api = "V1/getOneBPItemNum?bpitemid=" + id;
        string auth = "Bearer " + name + ":" + token;
        string method = "GET";
        string pdata = string.Empty;
        LoginClient.Instance.SendRequest(serverUrl + api, auth, method, pdata, callback);
    }

}