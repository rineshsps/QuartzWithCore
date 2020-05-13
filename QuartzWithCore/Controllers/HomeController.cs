﻿using Microsoft.AspNetCore.Mvc;
using Quartz;
using QuartzWithCore.Models;
using QuartzWithCore.Tasks;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using WorkflowSteps.Tasks;

namespace QuartzWithCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IScheduler _scheduler;

        public HomeController(IScheduler factory)
        {
            _scheduler = factory;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Repeat(int intervalinSec = 5)
        {
            ITrigger trigger = TriggerBuilder.Create()
             .WithIdentity($"Check Availability-{DateTime.Now}")
             .StartNow()
             .WithSimpleSchedule(scheduleBuilder =>
                scheduleBuilder
                    .WithIntervalInSeconds(intervalinSec)
                    .RepeatForever())
             .Build();
           
            IDictionary<string, object> map = new Dictionary<string, object>()
            {
                {"Current Date Time", $"{DateTime.Now}" },
                {"Tickets needed", $"5" },
                {"Concert Name", $"Rock" }
            };

            IJobDetail job = JobBuilder.Create<SchedueleJob>()
                        .WithIdentity($"Check Availability#{intervalinSec}-{DateTime.Now}")
                        .SetJobData(new JobDataMap(map))
                        .Build();

            //Finally schedule the job
            await _scheduler.ScheduleJob(job, trigger);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CheckAvailability()
        {
            ITrigger trigger = TriggerBuilder.Create()
             .WithIdentity($"Check Availability 1-{DateTime.Now}")
             .StartAt(new DateTimeOffset(DateTime.Now.AddSeconds(15)))
             .WithPriority(1)
             .Build();

            IDictionary<string, object> map = new Dictionary<string, object>()
            {
                {"Current Date Time", $"{DateTime.Now}" },
                {"Tickets needed", $"5" },
                {"Concert Name", $"Rock" }
            };

            IJobDetail job = JobBuilder.Create<CheckAvailabilityTask>()
             //.WithIdentity("Check Availability")
             .WithIdentity($"Check Availability 1-{DateTime.Now}")
                        .SetJobData(new JobDataMap(map))
                        .Build();


            await _scheduler.ScheduleJob(job, trigger);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> ReserveTickets()
        {
            ITrigger trigger = TriggerBuilder.Create()
             .WithIdentity($"Reserve Tickets-{DateTime.Now}")
             .StartNow()
             .WithPriority(1)
             .Build();

            IDictionary<string, object> map = new Dictionary<string, object>()
            {
            };

            IJobDetail job = JobBuilder.Create<ReserveTicketsTask>()
                        .WithIdentity($"Reserve Tickets:{DateTime.Now.Ticks}")
                        .SetJobData(new JobDataMap(map))
                        .Build();

            await _scheduler.ScheduleJob(job, trigger);
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
