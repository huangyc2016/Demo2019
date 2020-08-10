using NLog;
using System;

/// <summary>
/// Quartz版本3.1.0
/// </summary>
namespace QuartzApp
{
    class Program
    {
        public static readonly Logger loggerError = LogHelper.GetLogger("ErrorLog", true);
        public static readonly Logger loggerInfo = LogHelper.GetLogger("InfoLog", true);
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            loggerInfo.Info("启动服务");
            //初始化Hander
            InitSchedule.InitHander().GetAwaiter().GetResult();

            Console.WriteLine("Press any key to close the application");
            Console.ReadKey();
        }
    }
}
