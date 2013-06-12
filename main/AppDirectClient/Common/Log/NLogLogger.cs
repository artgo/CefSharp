using System.Reflection;
using System.Runtime.CompilerServices;
using NLog;
using System;
using System.Diagnostics;

namespace AppDirect.WindowsClient.Common.Log
{
    public class NLogLogger : ILogger
    {
        private readonly object _syncObject = new object();
        private readonly ILogInitializer _logInitializer;
        private volatile Logger _log = null;

        private class CurrentLogInitilizer : ILogInitializer
        {
            public Logger CreateLogger()
            {
                return GetCurrentLogger();
            }
        }

        private class NamedLogInitilizer : ILogInitializer
        {
            private readonly string _loggerName;

            internal NamedLogInitilizer(string loggerName)
            {
                _loggerName = loggerName;
            }

            public Logger CreateLogger()
            {
                return LogManager.GetLogger(_loggerName);
            }
        }

        private class TypeLogInitilizer : ILogInitializer
        {
            private readonly Type _type;

            public TypeLogInitilizer(Type type)
            {
                _type = type;
            }

            public Logger CreateLogger()
            {
                return LogManager.GetCurrentClassLogger(_type);
            }
        }

        private Logger Log
        {
            get
            {
                lock (_syncObject)
                {
                    if (_log == null)
                    {
                        _log = _logInitializer.CreateLogger();
                    }

                    return _log;
                }
            }
        }

        public NLogLogger(ILogInitializer logInitializer)
        {
            _logInitializer = logInitializer;
        }

        public NLogLogger()
            : this(new CurrentLogInitilizer())
        {
        }

        public NLogLogger(Logger log)
        {
            _log = log;
            _logInitializer = null;
        }

        public NLogLogger(string loggerName)
            : this(new NamedLogInitilizer(loggerName))
        {
        }

        public NLogLogger(Type type)
            : this(new TypeLogInitilizer(type))
        {
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

        private string JoinParameters(ParameterInfo[] parameters)
        {
            var result = "";
            for (int i = 0; i < parameters.Length; i++)
            {
                if (i > 0)
                {
                    result += ", ";
                }

                var parameterInfo = parameters[i];
                result += parameterInfo.ParameterType;
            }

            return result;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentLocation()
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(2);
            var method = stackFrame.GetMethod();

            return method.ReflectedType.FullName + "." + method.Name + "(" + JoinParameters(method.GetParameters()) + ") ";
        }

        public void TraceException(string message, Exception exception)
        {
            Log.TraceException(GetCurrentLocation() + message, exception);
        }

        public void Trace(string message, params object[] args)
        {
            Log.Trace(GetCurrentLocation() + message, args);
        }

        public void DebugException(string message, Exception exception)
        {
            Log.DebugException(GetCurrentLocation() + message, exception);
        }

        public void Debug(string message, params object[] args)
        {
            Log.Debug(GetCurrentLocation() + message, args);
        }

        public void ErrorException(string message, Exception exception)
        {
            Log.ErrorException(GetCurrentLocation() + message, exception);
        }

        public void Error(string message, params object[] args)
        {
            Log.Error(GetCurrentLocation() + message, args);
        }

        public void FatalException(string message, Exception exception)
        {
            Log.FatalException(GetCurrentLocation() + message, exception);
        }

        public void Fatal(string message, params object[] args)
        {
            Log.Fatal(GetCurrentLocation() + message, args);
        }

        public void InfoException(string message, Exception exception)
        {
            Log.InfoException(GetCurrentLocation() + message, exception);
        }

        public void Info(string message, params object[] args)
        {
            Log.Info(GetCurrentLocation() + message, args);
        }

        public void WarnException(string message, Exception exception)
        {
            Log.WarnException(GetCurrentLocation() + message, exception);
        }

        public void Warn(string message, params object[] args)
        {
            Log.Warn(GetCurrentLocation() + message, args);
        }

        public bool IsTraceEnabled
        {
            get
            {
                return Log.IsTraceEnabled;
            }
        }

        public bool IsDebugEnabled
        {
            get
            {
                return Log.IsDebugEnabled;
            }
        }

        public bool IsErrorEnabled
        {
            get
            {
                return Log.IsErrorEnabled;
            }
        }

        public bool IsFatalEnabled
        {
            get
            {
                return Log.IsFatalEnabled;
            }
        }

        public bool IsInfoEnabled
        {
            get
            {
                return Log.IsInfoEnabled;
            }
        }

        public bool IsWarnEnabled
        {
            get
            {
                return Log.IsWarnEnabled;
            }
        }
    }
}