using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;

public class CheckInput : MonoBehaviour {
    public static CheckInput instance;

    void Start() {
        instance = this;
    }

    //檢查密碼格式 (6-14 小寫英文+數字)
    public bool CheckPass(string str)
    {
        //bool isN = IsNumeric(str); //不可全是數字
        //bool isL = IsLetter(str);  //不可全是字母
        return (str.Length < 6) ? false : true; //帳號長度必須 > 6
    }

    //檢查信箱格式
    public bool CheckEmail(string Str)
    {
        return (Str.IndexOf("@") == -1 || Str.IndexOf("@") < 1 || Str.EndsWith("@") ||
                Str.IndexOf(".") == -1 || Str.IndexOf(".") < 1 || Str.EndsWith(".") ||
                Str.IndexOf(".@") > -1 || Str.IndexOf("@.") > -1 || Str == "") ? false : true;
    }

    //定義一個函數,作用:判斷strNumber是否為數字,是數字返回True,不是數字返回False
    public bool IsNumeric(string strNumber)
    {
        Regex NumberPattern = new Regex("[^0-9.-]");
        return !NumberPattern.IsMatch(strNumber);
    }

    public bool IsLetter(string strLetter)
    {
        Regex LetterPattern = new Regex("[^a-zA-Z]");
        return !LetterPattern.IsMatch(strLetter);
    }
}
