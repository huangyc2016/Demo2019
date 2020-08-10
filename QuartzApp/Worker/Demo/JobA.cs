using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzApp.Worker.Demo
{
    /// <summary>
    /// JobA
    /// DisallowConcurrentExecution表示(不能重复执行)
    /// </summary>
    [DisallowConcurrentExecution]
    public class JobA : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Program.loggerInfo.Info("服务A开始执行");
            //具体操作
            try
            {
                await Do();
            }
            catch (Exception ex)
            {
                Program.loggerError.Error($"服务A执行报错:{ex.ToString()}");
            }

            Program.loggerInfo.Info("服务A结束执行");
        }

        private async Task Do()
        {
            for (var i = 0; i < 10; i++)
            {
                await Console.Out.WriteLineAsync($"服务A执行输出: {i.ToString()}");
            }
        }
    }
}
