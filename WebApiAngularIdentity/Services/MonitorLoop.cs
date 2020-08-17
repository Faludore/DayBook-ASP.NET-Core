using Castle.Core.Logging;
using DataAccessLibary.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebApiAngularIdentity.Services.TaskQueue;
using WebApiAngularIdentity.Workers;

namespace WebApiAngularIdentity.Services
{
    public class MonitorLoop
    {
        private readonly IServiceProvider _services;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        private readonly CancellationToken _cancellationToken;

        public MonitorLoop(IBackgroundTaskQueue taskQueue,
            ILogger<MonitorLoop> logger, IServiceProvider services)
        {
            _services = services;
            _taskQueue = taskQueue;
            _logger = logger;
         
        }

        public void StartMonitorLoop(Mail mail)
        {
            _logger.LogInformation("Monitor Loop is starting.");

            // Run a console user input loop in a background thread
            Task.Run(() => Monitor(mail));
        }

        public void Monitor(Mail mail)
        {
            while (!_cancellationToken.IsCancellationRequested)
            {
                var processor = _services.GetRequiredService<TaskProcessor>();
                var queue = _services.GetRequiredService<IBackgroundTaskQueue>();

                queue.QueueBackgroundWorkItem(token =>
                {
                    return processor.RunAsync(mail, token);
                });
            }
        }
    }
}
