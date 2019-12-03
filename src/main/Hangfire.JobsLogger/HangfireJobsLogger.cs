﻿using Hangfire.JobsLogger.Helper;
using Hangfire.JobsLogger.Server;
using Hangfire.Server;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Hangfire.JobsLogger
{
    public class HangfireJobsLogger : ILogger
    {
        public PerformContext PfContext { get; private set; }

        public void Log(PerformContext context, LogLevel logLevel, string logMessage)
        {
            try
            {
                var jobId = context.BackgroundJob.Id;
                var item = Util.GetLoggerContextName(jobId);

                if (JobsLoggerFilter.Loggers[item] is LoggerContext loggerContext &&
                    loggerContext.IsEnabled(logLevel))
                {
                    using (var connection = context.Storage.GetConnection())
                    {
                        var jobExpirationTimeout = context.Storage.JobExpirationTimeout;

                        loggerContext.SaveLogMessage(connection, jobId, jobExpirationTimeout, logLevel, logMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine($"Error Write Log. Exception Message = {ex.Message}, StackTrace = {ex.ToString()}");
            }
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (PfContext == null)
                return;

            var message = formatter(state, exception);
            Log(PfContext, logLevel, message);
        }
    }
}
