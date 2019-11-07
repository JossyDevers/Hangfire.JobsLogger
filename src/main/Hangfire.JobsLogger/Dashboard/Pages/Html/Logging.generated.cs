﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Hangfire.JobsLogger.Dashboard.Pages.Html
{
    
    #line 2 "..\..\Dashboard\Pages\Html\Logging.cshtml"
    using System;
    
    #line default
    #line hidden
    using System.Collections.Generic;
    
    #line 3 "..\..\Dashboard\Pages\Html\Logging.cshtml"
    using System.Linq;
    
    #line default
    #line hidden
    using System.Text;
    
    #line 4 "..\..\Dashboard\Pages\Html\Logging.cshtml"
    using System.Text.RegularExpressions;
    
    #line default
    #line hidden
    
    #line 5 "..\..\Dashboard\Pages\Html\Logging.cshtml"
    using Hangfire.Dashboard;
    
    #line default
    #line hidden
    
    #line 6 "..\..\Dashboard\Pages\Html\Logging.cshtml"
    using Hangfire.Dashboard.Pages;
    
    #line default
    #line hidden
    
    #line 7 "..\..\Dashboard\Pages\Html\Logging.cshtml"
    using Hangfire.Dashboard.Resources;
    
    #line default
    #line hidden
    
    #line 8 "..\..\Dashboard\Pages\Html\Logging.cshtml"
    using Hangfire.JobsLogger.Server;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "2.0.0.0")]
    internal partial class Logging : RazorPage
    {
#line hidden

        public override void Execute()
        {


WriteLiteral("\r\n");










            
            #line 10 "..\..\Dashboard\Pages\Html\Logging.cshtml"
  
    Layout = new LayoutPage("Logs");

    var loggerContext = new LoggerContext();
    string jobId = Query("jobId");

    if (!int.TryParse(Query("from"), out int from) ||
        !int.TryParse(Query("count"), out int perPage))
    {
        from = 1;
        perPage = 10;
    }

    var totalLogs = loggerContext.GetCounterValue(Storage.GetConnection(), jobId);
    var jobLogs = loggerContext.GetLogMessagesByJobId(Storage.GetConnection(), jobId, from, perPage);

    var pager = new Pager(from, perPage, totalLogs);


            
            #line default
            #line hidden
WriteLiteral("\r\n<div class=\"row\">\r\n    <div class=\"col-md-3\">\r\n        ");


            
            #line 31 "..\..\Dashboard\Pages\Html\Logging.cshtml"
   Write(Html.JobsSidebar());

            
            #line default
            #line hidden
WriteLiteral("\r\n    </div>\r\n    <div class=\"col-md-9\">\r\n        <h1 class=\"page-header\">Logs</h" +
"1>\r\n");


            
            #line 35 "..\..\Dashboard\Pages\Html\Logging.cshtml"
          
            if (!jobLogs.Any())
            {

            
            #line default
            #line hidden
WriteLiteral("                <div class=\"logs\">\r\n                    There are no logs found y" +
"et.\r\n                </div>\r\n");


            
            #line 41 "..\..\Dashboard\Pages\Html\Logging.cshtml"
            }
            else
            {

            
            #line default
            #line hidden
WriteLiteral("                <div class=\"table-responsive\">\r\n                    ");


            
            #line 45 "..\..\Dashboard\Pages\Html\Logging.cshtml"
               Write(Html.PerPageSelector(pager));

            
            #line default
            #line hidden
WriteLiteral(@"
                    <table class=""table"">
                        <thead>
                            <tr>
                                <th class=""min-width"">Log Level</th>
                                <th>Message</th>
                                <th class=""min-width align-right"">Date</th>
                            </tr>
                        </thead>
                        <tbody>
");


            
            #line 55 "..\..\Dashboard\Pages\Html\Logging.cshtml"
                             foreach (var log in jobLogs)
                            {

            
            #line default
            #line hidden
WriteLiteral("                            <tr class=\"js-jobs-list-row\">\r\n                      " +
"          <td class=\"min-width\">");


            
            #line 58 "..\..\Dashboard\Pages\Html\Logging.cshtml"
                                                 Write(log.LogLevel);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                                <td>");


            
            #line 59 "..\..\Dashboard\Pages\Html\Logging.cshtml"
                               Write(log.Message);

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                                <td class=\"min-width align-right\">");


            
            #line 60 "..\..\Dashboard\Pages\Html\Logging.cshtml"
                                                             Write(Html.RelativeTime(@log.DateCreation));

            
            #line default
            #line hidden
WriteLiteral("</td>\r\n                            </tr>\r\n");


            
            #line 62 "..\..\Dashboard\Pages\Html\Logging.cshtml"
                            }

            
            #line default
            #line hidden
WriteLiteral("                        </tbody>\r\n                    </table>\r\n                 " +
"   ");


            
            #line 65 "..\..\Dashboard\Pages\Html\Logging.cshtml"
               Write(Html.Paginator(pager));

            
            #line default
            #line hidden
WriteLiteral("\r\n                </div>\r\n");


            
            #line 67 "..\..\Dashboard\Pages\Html\Logging.cshtml"
            }
        

            
            #line default
            #line hidden
WriteLiteral("    </div>\r\n</div>");


        }
    }
}
#pragma warning restore 1591
