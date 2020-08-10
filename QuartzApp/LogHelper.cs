using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QuartzApp
{
    public class LogHelper
    {
        public static readonly object _lockobj = new object();

        /// <summary>
        /// 初始化日志
        /// </summary>
        /// <param name="loggername">日志名字</param>
        /// <param name="isConsole">是否输出到控制台</param>
        /// <returns></returns>
        public static Logger GetLogger(string loggername, bool isConsole = false)
        {
            //判断当前target配置是否在日志中已经存在，如果存在直接返回，如果不存在重新加一个配置
            if (LogManager.Configuration.LoggingRules.Count(p => p.LoggerNamePattern.Equals(loggername)) <= 0)
            {
                lock (_lockobj)
                {
                    //新实例化一个nlog文件日志配置
                    FileTarget filetg = new FileTarget();
                    //把新的配置加到当前日志配置中
                    LogManager.Configuration.AddTarget(loggername, filetg);
                    //配置新配置日志路径和日志名称
                    filetg.FileName = $"logs/{loggername}/{"${shortdate}"}.log";
                    //输出日志内容
                    filetg.Layout = "${longdate} ${message} ${stacktrace}";

                    //把新配置加到定义日志的路由规则
                    LoggingRule rule = new LoggingRule(loggername, LogLevel.Info, filetg);
                    LogManager.Configuration.LoggingRules.Add(rule);

                    if (isConsole)
                    {
                        //读取Console配置
                        var ConsoleTarget = LogManager.Configuration.FindTargetByName("Console");
                        LoggingRule rule1 = new LoggingRule(loggername, LogLevel.Info, ConsoleTarget);
                        LogManager.Configuration.LoggingRules.Add(rule1);
                    }

                    //刷新当前配置
                    LogManager.Configuration.Reload();
                }
                //返回
                return LogManager.GetLogger(loggername);
            }
            else
            {
                //直接返回当前已存在的配置
                return LogManager.GetLogger(loggername);
            }
        }
    }
}
