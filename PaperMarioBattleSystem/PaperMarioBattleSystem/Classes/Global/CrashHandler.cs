using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Handles crashes through unhandled exceptions.
    /// <para>This currently works only for debug builds if a debugger is not attached.</para>
    /// </summary>
    public class CrashHandler : ICleanup
    {
        public CrashHandler()
        {
            //If debug is enabled and a debugger is not present, handle the crash
            //If a debugger is present (Ex. IDE) then we can see the cause of the crash directly
            if (Debug.DebugEnabled == true && Debug.DebuggerAttached == false)
            {
                AppDomain.CurrentDomain.UnhandledException -= HandleCrash;
                AppDomain.CurrentDomain.UnhandledException += HandleCrash;
            }
        }

        public void CleanUp()
        {
            AppDomain.CurrentDomain.UnhandledException -= HandleCrash;
        }

        /// <summary>
        /// A crash handler for unhandled exceptions.
        /// </summary>
        /// <param name="sender">The source of the unhandled exception event.</param>
        /// <param name="e">An UnhandledExceptionEventArgs that contains the event data.</param>
        private void HandleCrash(object sender, UnhandledExceptionEventArgs e)
        {
            //Get the exception object
            Exception exc = e.ExceptionObject as Exception;
            if (exc != null)
            {
                //Dump the message, stack trace, and logs to a file
                using (StreamWriter writer = File.CreateText(Debug.DebugGlobals.GetCrashLogPath()))
                {
                    writer.Write($"Message: {exc.Message}\n\nStack Trace:\n");
                    writer.Write($"{exc.StackTrace}\n\n");
                    writer.Write($"Log Dump:\n{Debug.LogDump.ToString()}");

                    writer.Flush();
                }
            }
        }
    }
}
