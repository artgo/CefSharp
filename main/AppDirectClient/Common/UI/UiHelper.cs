using AppDirect.WindowsClient.Common.Log;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace AppDirect.WindowsClient.Common.UI
{
    public class UiHelper : IUiHelper
    {
        private readonly ILogger _log;

        public UiHelper(ILogger log)
        {
            if (log == null)
            {
                throw new ArgumentNullException("log");
            }

            _log = log;
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
        public string GetPrevLocation(int depth)
        {
            var stackTrace = new StackTrace();
            var stackFrame = stackTrace.GetFrame(depth);
            var method = stackFrame.GetMethod();

            return method.ReflectedType.FullName + "." + method.Name + "(" + JoinParameters(method.GetParameters()) + ") ";
        }

        public void PerformInUiThread(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var currentApplication = System.Windows.Application.Current;
            if ((currentApplication == null) || (Thread.CurrentThread == currentApplication.Dispatcher.Thread))
            {
                action.Invoke();
            }
            else
            {
                currentApplication.Dispatcher.Invoke(action);
            }
        }

        public Thread StartAsynchronously(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var resultThread = new Thread(action.Invoke);
            resultThread.Start();

            return resultThread;
        }

        public void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            var startTime = Environment.TickCount;

            if (requiresUiThread)
            {
                PerformInUiThread(action);
            }
            else
            {
                action.Invoke();
            }

            var remainingTime = minimumMillisecondsBeforeReturn - (Environment.TickCount - startTime);

            if (remainingTime > 0)
            {
                Thread.Sleep(remainingTime);
            }
        }

        public void GracefulShutdown()
        {
            _log.Info("Started shutdown");

            if (Application.Current != null)
            {
                PerformInUiThread(() => Application.Current.Shutdown(0));

                _log.Info("Application shutdown complete");
            }

            _log.Info("Shutdown complete");

            Environment.Exit(0);
        }

        public void IgnoreException(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            try
            {
                action.Invoke();
            }
            catch (Exception e)
            {
                _log.ErrorException("Invocation failed", e);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns>Default if fails</returns>
        public T IgnoreException<T>(Func<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException("action");
            }

            try
            {
                return action.Invoke();
            }
            catch (Exception e)
            {
                _log.ErrorException("Invokation failed", e);
                return default(T);
            }
        }

        public void Sleep(int milliseconds)
        {
            Thread.Sleep(milliseconds);
        }

        public void Sleep(TimeSpan sleepTime)
        {
            Thread.Sleep(sleepTime);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message);
        }
    }
}