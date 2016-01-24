
using System.Configuration;
public class Config : ConfigurationSection
{
    [ConfigurationProperty("sDBHost")]
    public string sDBHost { get { return (string)base["sDBHost"]; } }

    [ConfigurationProperty("sDB")]
    public string sDB { get { return (string)base["sDB"]; } }

    [ConfigurationProperty("sDBUser")]
    public string sDBUser { get { return (string)base["sDBUser"]; } }

    [ConfigurationProperty("sDBPass")]
    public string sDBPass { get { return (string)base["sDBPass"]; } }

    [ConfigurationProperty("sIP")]
    public string sIP { get { return (string)base["sIP"]; } }

    [ConfigurationProperty("sPORT")]
    public int sPORT { get { return (int)base["sPORT"]; } }

}

