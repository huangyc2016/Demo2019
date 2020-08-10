using Quartz;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace QuartzApp.Worker.Demo
{
    /// <summary>
    /// JobB
    /// DisallowConcurrentExecution表示(不能重复执行)
    /// </summary>
    [DisallowConcurrentExecution]
    public class JobB : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            Program.loggerInfo.Info("服务B开始执行");
            //具体操作
            try
            {
                await Do();
            }
            catch (Exception ex)
            {
                Program.loggerError.Error($"服务B执行报错:{ex.ToString()}");
            }

            Program.loggerInfo.Info("服务B结束执行");
        }

        private async Task Do()
        {
            for (var i = 0; i < 10; i++)
            {
                await Console.Out.WriteLineAsync($"服务B取执行输出: {i.ToString()}");
            }
        }
    }
}
