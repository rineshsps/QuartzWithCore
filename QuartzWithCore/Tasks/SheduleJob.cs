using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace QuartzWithCore.Tasks
{
    public class SchedueleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var jobKey = context.JobDetail.Key;
            Debug.WriteLine($"Hello - executed at {DateTime.Now} | jobKey- {jobKey}");
            return Task.FromResult(0);
        }
    }
}
