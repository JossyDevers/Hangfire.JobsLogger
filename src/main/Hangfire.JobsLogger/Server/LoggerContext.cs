﻿using Hangfire.Common;
using Hangfire.JobsLogger.Helper;
using Hangfire.JobsLogger.Model;
using Hangfire.Server;
using Hangfire.Storage;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Hangfire.JobsLogger.Server
{
    internal class LoggerContext
    {
        public PerformContext PfContext { get; private set; }
        private JobsLoggerOptions _options;

        private static readonly Logging.ILog HangfireInternalLog = Logging.LogProvider.For<HangfireJobsLogger>();

        private readonly object _lockObj = new object();

        public void SetPerformContext(PerformContext context, 
            JobsLoggerOptions options)
        {
            PfContext = context;
            _options = options;
        }

        public JobsLoggerOptions GetOptions() 
        {
            return _options;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return _options.LogLevel != LogLevel.None &&
                logLevel >= _options.LogLevel;
        }

        public int GetCounterValue(IStorageConnection connection, string jobId, bool plus = false, TimeSpan? jobExpirationTimeout = null) 
        {
            string counterName = Util.GetCounterName(jobId);
            int counterValue = 0;

            if (!plus) 
            {
                var counterHash = connection.GetAllEntriesFromHash(counterName);
                counterValue = counterHash != null && counterHash.Any() ?
                    int.Parse(counterHash.FirstOrDefault().Value) : 0;
            }
            else
            {
                lock (_lockObj)
                {
                    using (var writeTransaction = connection.CreateWriteTransaction())
                    {
                        var counterHash = connection.GetAllEntriesFromHash(counterName);
                        counterValue = counterHash != null && counterHash.Any() ?
                            int.Parse(counterHash.FirstOrDefault().Value) : 0;

                        var dictionaryLogCounter = new Dictionary<string, string>
                        {
                            [counterName] = Convert.ToString(++counterValue)
                        };

                        writeTransaction.SetRangeInHash(counterName, dictionaryLogCounter);

                        if (writeTransaction is JobStorageTransaction jsTransaction)
                            jsTransaction.ExpireHash(counterName, jobExpirationTimeout ?? TimeSpan.MinValue);

                        writeTransaction.Commit();
                    }
                }
            }

            return counterValue;
        }

        public void SaveLogMessage(IStorageConnection connection, string jobId, TimeSpan jobExpirationTimeout, LogLevel logLevel, string logMessage) 
        {
            using (var writeTransaction = connection.CreateWriteTransaction())
            {
                var logMessageModel = new LogMessage
                {
                    LogLevel = logLevel,
                    DateCreation = DateTime.UtcNow,
                    Message = logMessage
                };

                int counterValue = GetCounterValue(connection, jobId, true, jobExpirationTimeout);

                var keyName = Util.GetKeyName(counterValue, jobId);
                var logSerialization = SerializationHelper.Serialize(logMessageModel);

                var dictionaryLog = new Dictionary<string, string>
                {
                    [keyName] = logSerialization
                };

                writeTransaction.SetRangeInHash(keyName, dictionaryLog);

                if (writeTransaction is JobStorageTransaction jsTransaction)
                {
                    jsTransaction.ExpireHash(keyName, jobExpirationTimeout);
                }

                writeTransaction.Commit();
            }
        }

        public IEnumerable<LogMessage> GetLogMessagesByJobId(IStorageConnection connection, string jobId, int from = 1, int count = 10)
        {
            var logMessages = new List<LogMessage>();

            try
            {
                int counterValue = GetCounterValue(connection, jobId);
                int toValue = count > counterValue ? counterValue : count;
                int fromValue = from <= 0 ? 1 : from;

                foreach (int i in Enumerable.Range(fromValue, toValue)) 
                {
                    var logMessageHash = connection.GetAllEntriesFromHash(Util.GetKeyName(i, jobId));

                    if (logMessageHash != null && logMessageHash.Any())
                    {
                        var logMessage = SerializationHelper
                            .Deserialize<LogMessage>(logMessageHash.FirstOrDefault().Value);
                        logMessages.Add(logMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                var logLine = $"Error Read Log Messages. Exception Message = {ex.Message}, StackTrace = {ex.ToString()}";

                HangfireInternalLog.Log(Logging.LogLevel.Error, () => logLine);
                Debug.WriteLine(logLine);
            }

            return logMessages;
        }
    }
}
