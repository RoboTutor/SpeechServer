using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

/// <summary>
/// Summary description for Class1
/// </summary>
public class AppConfigReader
{
    public AppConfigReader()
    {
        //configSettings = ConfigurationManager.AppSettings.;
    }
    public static bool GetBoolKeyValue(string keyname)
    {
        return (GetStringKeyValue(keyname).Trim().ToLower().Equals("true") == true);
    }

    public static int GetIntKeyValue(string keyname)
    {
        string strValue = GetStringKeyValue(keyname);
        if (strValue.Length > 0)
        {
            int intValue = 0;
            int.TryParse(strValue, out intValue);
            return intValue;
        }
        else
        {
            return 0;
        }
    }
    public static float GetFloatKeyValue(string keyname)
    {
        string strValue = GetStringKeyValue(keyname);
        if (strValue.Length > 0)
        {
            float floatValue = 0;
            float.TryParse(strValue, out floatValue);
            return floatValue;
        }
        else
        {
            return 0;
        }
    }
    public static string GetStringKeyValue(string keyname)
    {
        string[] values = ConfigurationManager.AppSettings.GetValues(@keyname);
        if (values == null)
            return "";
        else
            return values[0];
    }
    public static double GetDoubleKeyValue(string keyname)
    {
        string strValue = GetStringKeyValue(keyname);
        if (strValue.Length > 0)
        {
            double doubleValue = 0;
            double.TryParse(strValue, out doubleValue);
            return doubleValue;
        }
        else
        {
            return 0;
        }
    }
}

