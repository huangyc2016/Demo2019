using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using QuartzApp.Entity;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuartzApp
{
    public class InitSchedule
    {
        private static IConfigurationRoot _configuration { get; set; }

        public static async Task InitHander()
        {
            // Grab the Scheduler instance from the Factory
            NameValueCollection props = new NameValueCollection
            {
                { "quartz.serializer.type", "binary" }
            };
            StdSchedulerFactory factory = new StdSchedulerFactory(props);
            IScheduler scheduler = await factory.GetScheduler();
            //
            try
            {
                var configBuilder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("ScheduleEntityConfig.json", optional: false, reloadOnChange: true);
                _configuration = configBuilder.Build();

                //从配置信息里面强类型转换
                var moduleSettings = new List<ScheduleEntity>();
                _configuration.GetSection("ScheduleConfig").Bind(moduleSettings);
                moduleSettings = moduleSettings.FindAll(c => c.Status == EnumType.JobStatus.已启用);

                if (moduleSettings == null || moduleSettings.Count < 1)
                {
                    await Console.Out.WriteLineAsync("没有服务需要启动");
                    return;
                }

                //start scheduler
                await scheduler.Start();

                foreach (var item in moduleSettings)
                {
                    //定义需要反射的文件
                    AssemblyName assemblyName = new AssemblyName(item.AssemblyName);

                    //反射出服务
                    Type jobType = Assembly.Load(assemblyName).GetType(item.JobName);

                    // define the job and tie it to our HelloJob class
                    IJobDetail job = new JobDetailImpl(item.JobName, item.JobGroup, jobType);

                    // 创建触发器
                    ITrigger trigger;
                    //校验是否正确的执行周期表达式
                    if (!string.IsNullOrEmpty(item.Cron) && CronExpression.IsValidExpression(item.Cron))
                    {
                        Console.WriteLine($"成功触发:{item.JobName}服务");
                        trigger = CreateCronTrigger(item);
                    }
                    else
                    {
                        Console.WriteLine($"服务:{item.JobName},Cron[{item.Cron}]表达式错误");
                        continue;
                    }


                    // 告诉Quartz使用我们的触发器来安排作业
                    await scheduler.ScheduleJob(job, trigger);
                }
            }
            catch (SchedulerException se)
            {
                Console.WriteLine($"异常错误信息:{se.ToString()}");
                //and last shut down the scheduler when you are ready to close your program
                await scheduler.Shutdown();
            }

        }



        /// 创建类型Cron的触发器
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        private static ITrigger CreateCronTrigger(ScheduleEntity m)
        {
            // 作业触发器
            return TriggerBuilder.Create()
                   .WithIdentity(m.JobName, m.JobGroup)
                   .StartNow()
                   .WithCronSchedule(m.Cron)//指定cron表达式
                   .ForJob(m.JobName, m.JobGroup)//作业名称
                   .Build();
        }
    }
}
