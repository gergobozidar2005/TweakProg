using System.Management;

namespace TweakAppClient
{
    public static class HardwareInfo
    {
        public static string GetMotherboardId()
        {
            try
            {
                var mbs = new ManagementObjectSearcher("Select SerialNumber From Win32_BaseBoard");
                foreach (ManagementObject mo in mbs.Get())
                {
                    // Visszaadjuk az első talált sorozatszámot
                    return mo["SerialNumber"].ToString();
                }
            }
            catch
            {
                // Hiba esetén egyedi, de jelzésértékű azonosítót adunk vissza
                return "UnknownHardwareID";
            }
            return "NoHardwareIDFound";
        }
    }
}