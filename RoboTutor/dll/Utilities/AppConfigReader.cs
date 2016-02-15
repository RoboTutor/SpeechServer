using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

/// <summary>
/// 
/// </summary>
public class AppConfigReader
{
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

    static AppSettingsSection AppSettings = null;

    /// <summary>
    /// If you use this function always call it again with a empty-string so the default
    /// configuration-manager-file is used!!
    /// </summary>
    /// <param name="otherappname">root name of the config-file</param>
    public static void SetConfigFile(string otherappname)
    {
        Configuration ConfigFile = null;
        if (otherappname.Length > 0)
        {
            if (File.Exists(Application.StartupPath + @otherappname) == true)
            {
                Debug.WriteLine("Try open " + Application.StartupPath + @otherappname);
                ConfigFile = ConfigurationManager.OpenExeConfiguration(Application.StartupPath
                            + @otherappname);
            }
            else
            {
                MessageBox.Show("Can not find " + otherappname);
                return;
            }
       }
        else
        {
            ConfigFile = ConfigurationManager.OpenExeConfiguration(Application.ExecutablePath);
        }
        AppSettings = ConfigFile.AppSettings;
    }

    public static string GetStringKeyValue(string keyname)
    {
        string CurrentValue="";

        // init the content if necessary;
        if (AppSettings == null)
        {
            SetConfigFile("");
        }
        if (AppSettings.Settings.Count == 0)
        {
            return CurrentValue;
        }
        else
        {
            KeyValueConfigurationElement keypair = AppSettings.Settings[keyname];
            if (keypair != null)
            {
                CurrentValue = AppSettings.Settings[keyname].Value;
            }
        }
        return CurrentValue;

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

