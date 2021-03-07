using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Misc.Ebay.Services;
using Nop.Services.Logging;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.Ebay.ScheduleTask
{
    public class OrderDispatchScheduledTask : IScheduleTask
    {
        private readonly ILogger _logger;
        private readonly IOrderDispatchService _dispatchService;

        public OrderDispatchScheduledTask(IOrderDispatchService dispatchService, ILogger logger)
        {
            _dispatchService = dispatchService;
            _logger = logger;
        }

        public void Execute()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _logger.InsertLog(LogLevel.Information,
                "Starting Order scheduled Dispatch service at " + DateTime.Now.ToString());
            _dispatchService.DispatchOrders();
            _logger.InsertLog(LogLevel.Information,
                "Order scheduled Dispatch service took " + stopwatch.Elapsed.Seconds + "seconds");
        }
    }
}
