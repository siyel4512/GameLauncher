using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftCryptography
{
    private int NOT_ARRAY = 9999;

    public string GetNumber(string inputStr)
    {
        DateTime today = DateTime.Today;
        int day = today.Day;

        //string[] array = new string[] {
        //        "1", "2", "3", "4", "5", "6", "7", "8", "9",
        //        "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "y", "z",
        //        "A","B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q","R", "S", "T", "U", "V", "W", "X", "Y", "Z",
        //        "!", "@", "#", "$", "%", "^", "&", "*"
        //    };

             string[] array = new string[] {
                     "S", "e", "9", "H", "&", "n", "B", "p", "3", "*",
              "f", "Z", "5", "t", "#", "7", "Y", "I", "!", "@",
              "g", "K", "l", "T", "E", "m", "8", "s", "a", "O",
              "2", "V", "L", "]", "6", "C", "J", "{", "X", "P",
              "y", "j", "k", "d", "u", "1", "D", "R", "Q", "U",
              "v", "i", "N", "b", "W", "F", "h", "A", "c", "M",
              "o", "q", "^", "z", "w", "G", "r", "[", "4", "}",
              "x", "$", ":", "%"
                 };

        string result = Crypto(inputStr, array, day);
        return result;
    }

    private string Crypto(string inputStr, string[] array, int no)
    {
        char[] inputStrToCharArray = inputStr.ToCharArray();

        for (int idx = 0; idx < inputStrToCharArray.Length; idx++)
        {
            if (idx != no)
            {
                string ch = inputStrToCharArray.GetValue(idx).ToString();

                int curWordIndex = GetCurWordIndex(ch, array);

                if (curWordIndex != NOT_ARRAY)
                {
                    int cryptoWordIndex = curWordIndex - no;

                    if (0 <= cryptoWordIndex)
                    {
                        inputStrToCharArray[idx] = Char.Parse(array[cryptoWordIndex]);
                    }
                    else
                    {
                        cryptoWordIndex = cryptoWordIndex + array.Length;
                        inputStrToCharArray[idx] = Char.Parse(array[cryptoWordIndex]);
                    }
                }
            }
        }

        string temp = string.Empty;
        for (int idx = 0; idx < inputStrToCharArray.Length; idx++)
        {
            temp += inputStrToCharArray[idx];
        }

        return temp;
    }

    private int GetCurWordIndex(string ch, string[] array)
    {
        for (int idx = 0; idx < array.Length; idx++)
        {
            if (array[idx].Equals(ch)) { return idx; }
        }

        return NOT_ARRAY;
    }
}