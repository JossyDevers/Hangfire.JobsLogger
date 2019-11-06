﻿using Hangfire.Dashboard;
using Hangfire.JobsLogger.Dashboard;
using Hangfire.JobsLogger.Server;
using Hangfire.States;
using System;
using System.Collections.Generic;
using System.Text;
using Hangfire.JobsLogger.Dashboard.Pages.Html;

namespace Hangfire.JobsLogger
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseJobsLogger(this IGlobalConfiguration configuration,
            JobsLoggerOptions options = null)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            options = options ?? new JobsLoggerOptions();

            GlobalJobFilters.Filters.Add(new JobsLoggerFilter(options));

            //TODO: Controllers - UI - Routing
            //JobHistoryRenderer.Register(ProcessingState.StateName, new ProcessingStateRenderer().Render);

            DashboardRoutes.Routes.AddRazorPage("/jobs/logs", x => new Dashboard.Pages.Html.Logging());

            //The next line code is for testing :
            //JobHistoryRenderer.Register(SucceededState.StateName, new ProcessingStateRenderer().Render);
            //JobHistoryRenderer.Register("Loggin", new ProcessingStateRenderer().Render);

            // register server filter for jobs

            return configuration;
        }
    }
}
