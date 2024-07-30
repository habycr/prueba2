using System.IO;

public static class PortConfig
{
    private static readonly string configFilePath = "config.txt";

    public static int GetNextPort()
    {
        int port;
        if (File.Exists(configFilePath))
        {
            string lastPortStr = File.ReadAllText(configFilePath);
            if (int.TryParse(lastPortStr, out port))
            {
                port++;
            }
            else
            {
                port = 12345; // Default port if config file is corrupt
            }
        }
        else
        {
            port = 12345; // Default starting port
        }

        File.WriteAllText(configFilePath, port.ToString());
        return port;
    }
}
