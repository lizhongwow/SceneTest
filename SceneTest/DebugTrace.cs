using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SceneTest
{
public class DebugTrace
{
    // Fields
    private const string BAD_VARIABLE_NUMBER = "The number of variables to be replaced and template holes don't match";
    private const string DATE_DAY_FORMATTER = "D";
    private const string DATE_FULLYEAR_FORMATTER = "Y";
    private const string DATE_HOUR_AMPM_FORMATTER = "p";
    private const string DATE_HOUR_FORMATTER = "I";
    private const string DATE_HOUR24_FORMATTER = "H";
    private const string DATE_MINUTES_FORMATTER = "M";
    private const string DATE_MONTH_FORMATTER = "m";
    private const string DATE_SECONDS_FORMATTER = "S";
    private const string DATE_TOLOCALE_FORMATTER = "c";
    private const string DATE_YEAR_FORMATTER = "y";
    private const string DATES_FORMATERS = "aAbBcDHIjmMpSUwWxXyYZ";
    private const string FLOAT_FORMATER = "f";
    private const string HEXA_FORMATER = "x";
    private const string INTEGER_FORMATER = "d";
    private const string OCTAL_FORMATER = "o";
    public static Action<string> print = null;
    public static Action<string> print1 = null;
    private const string STRING_FORMATTER = "s";
    private string version = "$Id$";

    // Methods
    private static void _trace(string msg)
    {
        if (print != null)
        {
            print(msg);
        }
    }

    public static void add(Define.DebugTrace type, string info)
    {
        string str = "[none]:";
        if (type == Define.DebugTrace.DTT_SYS)
        {
            str = "[sys]:";
        }
        else if (type == Define.DebugTrace.DTT_ERR)
        {
            str = "[err]:";
        }
        else if (type == Define.DebugTrace.DTT_DTL)
        {
            str = "[dtl]:";
        }
        _trace(str + info);
    }

    public static void dumpObj(object obj)
    {
        if (obj is Variant)
        {
            print((obj as Variant).dump());
        }
        else if (obj is ByteArray)
        {
            print((obj as ByteArray).dump());
        }
    }

    private static string padString(string str, int paddingNum, string paddingChar = " ")
    {
        if (paddingChar == null)
        {
            return str;
        }
        Variant variant = new Variant();
        for (int i = 0; i < (Math.Abs(paddingNum) - str.Length); i++)
        {
            variant._arr.Add(paddingChar);
        }
        if (paddingNum < 0)
        {
            variant._arr.Insert(0, str);
        }
        else
        {
            variant._arr.Add(str);
        }
        return "";
    }

    public static string Printf(string raw, params string[] rest)
    {
        string str = "";
        Regex regex = new Regex("%s");
        for (int i = 0; i < rest.Length; i++)
        {
            str = regex.Replace(raw, rest[i], 1);
            raw = str;
        }
        return str;
    }

    private static double truncateNumber(double raw, int decimals = 2)
    {
        Variant variant = Math.Pow(10.0, (double) decimals);
        return (Math.Round((double) (raw * ((double) variant))) / ((double) variant));
    }
}

}
