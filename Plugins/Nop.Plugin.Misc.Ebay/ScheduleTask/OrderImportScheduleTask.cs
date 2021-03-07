using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Nop.Core.Domain.Logging;
using Nop.Plugin.Misc.Ebay.Services;
using Nop.Services.Logging;
using Nop.Services.Stores;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.Ebay.ScheduleTask
{
    public class OrderImportScheduleTask : IScheduleTask
    {
        #region Fields

        private readonly ILogger _logger;
        private readonly IOrderImportService _orderImportService;

        #endregion

        #region Ctor

        public OrderImportScheduleTask(IOrderImportService gsService, ILogger logger)
        {
            this._orderImportService = gsService;
            _logger = logger;
        }

        #endregion
        #region Method
        public void Execute()
        {

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            _logger.InsertLog(LogLevel.Information,
                "Starting Order scheduled Import service at " + DateTime.Now.ToString());
          _orderImportService.ImportOrder();
          _logger.InsertLog(LogLevel.Information,
              "Order scheduled Import service took " + stopwatch.Elapsed.Seconds + "seconds");

        }

        #endregion
    }

}
