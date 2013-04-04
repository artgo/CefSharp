using NLog;
using System;
using System.Diagnostics;

namespace AppDirect.WindowsClient.Common.Log
{
    public class NLogLogger : ILogger
    {
        private readonly Logger _log;

        public NLogLogger()
        {
            _log = GetCurrentLogger();
        }

        public NLogLogger(Logger log)
        {
            _log = log;
        }

        public NLogLogger(string loggerName)
        {
            _log = LogManager.GetLogger(loggerName);
        }

        public NLogLogger(Type type)
        {
            _log = LogManager.GetCurrentClassLogger(type);
        }

        private static Logger GetCurrentLogger()
        {
            string loggerName;
            Type declaringType;
            int framesToSkip = 1;
            do
            {
                var frame = new StackFrame(framesToSkip, false);
                var method = frame.GetMethod();
                declaringType = method.DeclaringType;
                if (declaringType == null)
                {
                    loggerName = method.Name;
                    break;
                }

                framesToSkip++;
                loggerName = declaringType.FullName;
            } while (declaringType.Module.Name.Equals("mscorlib.dll", StringComparison.OrdinalIgnoreCase));

            return LogManager.GetLogger(loggerName);
        }

        public static ILogger GetLogger()
        {
            return new NLogLogger(GetCurrentLogger());
        }

        public void TraceException(string message, Exception exception)
        {
            _log.TraceException(message, exception);
        }

        public void Trace(string message, params object[] args)
        {
            _log.Trace(message, args);
        }

        public void DebugException(string message, Exception exception)
        {
            _log.DebugException(message, exception);
        }

        public void Debug(string message, params object[] args)
        {
            _log.Debug(message, args);
        }

        public void ErrorException(string message, Exception exception)
        {
            _log.ErrorException(message, exception);
        }

        public void Error(string message, params object[] args)
        {
            _log.Error(message, args);
        }

        public void FatalException(string message, Exception exception)
        {
            _log.FatalException(message, exception);
        }

        public void Fatal(string message, params object[] args)
        {
            _log.Fatal(message, args);
        }

        public void InfoException(string message, Exception exception)
        {
            _log.InfoException(message, exception);
        }

        public void Info(string message, params object[] args)
        {
            _log.Info(message, args);
        }

        public void WarnException(string message, Exception exception)
        {
            _log.WarnException(message, exception);
        }

        public void Warn(string message, params object[] args)
        {
            _log.Warn(message, args);
        }

        public bool IsTraceEnabled
        {
            get
            {
                return _log.IsTraceEnabled;
            }
        }

        public bool IsDebugEnabled
        {
            get
            {
                return _log.IsDebugEnabled;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return _log.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return _log.IsFatalEnabled;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return _log.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return _log.IsWarnEnabled;
            }
        }
    }
}