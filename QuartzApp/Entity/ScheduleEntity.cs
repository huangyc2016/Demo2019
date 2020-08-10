using System;
using System.Collections.Generic;
using System.Text;

namespace QuartzApp.Entity
{
    public class ScheduleEntity
    {
        /// <summary>
        /// 任务所在DLL对应的程序集名称
        /// </summary>
        public string AssemblyName { get; set; }

        /// <summary>
        /// 任务编号
        /// </summary>
        public int JobId { get; set; }

        /// <summary>
        /// 任务分组
        /// </summary>
        public string JobGroup { get; set; }

        /// <summary>
        /// 任务名称
        /// </summary>
        public string JobName { get; set; }

        /// <summary>
        /// 执行周期表达式
        /// </summary>
        public string Cron { get; set; }

        /// <summary>
        /// 任务状态
        /// </summary>
        public EnumType.JobStatus Status { get; set; }
    }
}
