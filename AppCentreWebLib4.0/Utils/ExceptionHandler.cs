﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace ACG.EA.AppCentre.Utils
{

    public sealed class ExceptionHandler
    {

        private ExceptionHandler() { }

        private const string _APPSOURCE = "ACG.EA.AppCentre.Web";
        private const string _LOGFILE = "App_Data/ErrorLog.txt";

        public static void LogException(Exception ex, string source)
        {
            StreamWriter sw;
            try
            {
                if (!EventLog.SourceExists(_APPSOURCE))
                    EventLog.CreateEventSource(_APPSOURCE, "Application");

                EventLog.WriteEntry(_APPSOURCE, "Source: \n" + ex.ToString(), EventLogEntryType.Error);
            
                // Include enterprise logic for logging exceptions
                // Get the absolute path to the log file
                string logFile;
                logFile = HttpContext.Current.Server.MapPath(_LOGFILE);

                // Open the log file for append and write the log
                 sw = new StreamWriter(logFile, true);
                sw.WriteLine("********** {0} **********", DateTime.Now);
                if (ex.InnerException != null)
                {
                    sw.Write("Inner Exception Type: ");
                    sw.WriteLine(ex.InnerException.GetType().ToString());
                    sw.Write("Inner Exception: ");
                    sw.WriteLine(ex.InnerException.Message);
                    sw.Write("Inner Source: ");
                    sw.WriteLine(ex.InnerException.Source);
                    if (ex.InnerException.StackTrace != null)
                    {
                        sw.WriteLine("Inner Stack Trace: ");
                        sw.WriteLine(ex.InnerException.StackTrace);
                    }
                }
                sw.Write("Exception Type: ");
                sw.WriteLine(ex.GetType().ToString());
                sw.WriteLine("Exception: " + ex.Message);
                sw.WriteLine("Source: " + source);
                sw.WriteLine("Stack Trace: ");
                if (ex.StackTrace != null)
                {
                    sw.WriteLine(ex.StackTrace);
                    sw.WriteLine();
                }
                sw.Close();
            }
            catch
            {
            }
            finally
            {
                sw=null;
            }
        }

        // Notify System Operators about an exception
        public static void NotifySystemOps(Exception exc)
        {
            // Include code for notifying IT system operators
        }

    
    }
    
    
}

