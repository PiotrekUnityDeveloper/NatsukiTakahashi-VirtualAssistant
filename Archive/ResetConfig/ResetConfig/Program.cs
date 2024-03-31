using System;

namespace ResetConfig
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Console.WriteLine("Purging your config data...");

            try
            {
                File.Copy(Environment.CurrentDirectory + "\\defconfig\\default.config", Environment.CurrentDirectory + "\\launcher.config", true);
                Console.WriteLine("Configuration file was reset!");
            }
            catch
            {
                Console.WriteLine("REMOVING CONFIG DATA FAILED!");
                Console.WriteLine("To erase your config data, please replace your launcher.config with default.config located in:\n/defconfig/\n\n...or run this task as an administrator");
                Console.WriteLine("\n");
                Console.ReadKey();
            }

            try
            {
                File.WriteAllText(Environment.CurrentDirectory + "\\persist\\AnalogLaunch.txt", "0");
                Console.WriteLine("AnalogLaunch was reset!");
                File.WriteAllText(Environment.CurrentDirectory + "\\persist\\FirstTime.txt", "1");
                Console.WriteLine("FT file was reset!");
            }
            catch
            {
                Console.WriteLine("REMOVING STORAGE DATA FAILED!");
            }

            Console.WriteLine("\nexiting...");
        }
    }
}