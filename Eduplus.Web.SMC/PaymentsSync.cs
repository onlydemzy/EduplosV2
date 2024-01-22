using Eduplos.Services.Contracts;
using Eduplos.Web.SMC.Controllers;
using Eduplos.Web.SMC.ViewModels;
using Newtonsoft.Json;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace Eduplos.Web.SMC
{
    public class PaymentsSyncJob : IJob
    {
        IStudentsAccountsService _studentAccounts;
        IGeneralDutiesService _generalDutiesService;
        IStudentService _studentService;

        public PaymentsSyncJob(IStudentsAccountsService accounts, IGeneralDutiesService gen, IStudentService st)
        {
            _studentAccounts = accounts;
            _generalDutiesService = gen;
            _studentService = st;
        }
        public Task Execute(IJobExecutionContext context)
        {
            return Task.Run(() =>
            {
                //AllCompletedRemitaPayments();
                //DeleteNonCompletedInvoices();
            }
                );
        }
        
        
    }
        public class RemitaDailyPayCheckScheduler
        {
            public static async Task StartCheckingPayments()
            {
                try
                {
                    //Get schedular instance from factory
                    NameValueCollection props = new NameValueCollection
                {
                    { "quartz.serializer.type", "binary" }
                };

                    StdSchedulerFactory factory = new StdSchedulerFactory(props);
                    IScheduler scheduler = await factory.GetScheduler();
                    await scheduler.Start();

                    IJobDetail job = JobBuilder.Create<PaymentsSyncJob>()
                        .WithIdentity("trigger1", "group1").Build();

                   ITrigger trigger = TriggerBuilder.Create()
                        .WithSimpleSchedule(x => x
                            .WithIntervalInMinutes(30)
                            .RepeatForever())
                        .Build();
                        
                    /* ITrigger trigger = TriggerBuilder.Create()
                    .WithDailyTimeIntervalSchedule
                    (s =>
                    s.WithIntervalInHours(24)
                    .OnEveryDay()
                    .StartingDailyAt(TimeOfDay.HourAndMinuteOfDay(0, 0))
                    ).Build();*/


                    await scheduler.ScheduleJob(job, trigger);

                }
                catch (Exception ex)
                {

                }

                
            }

        }
    }