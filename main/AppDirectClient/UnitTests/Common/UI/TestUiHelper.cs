using AppDirect.WindowsClient.Common.UI;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace AppDirect.WindowsClient.Tests.Common.UI
{
    public class TestUiHelper : IUiHelper
    {
        public virtual bool WasShutdown { get; private set; }

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

        public virtual void PerformInUiThread(Action action)
        {
            action.Invoke();
        }

        public void StartAsynchronously(Action action)
        {
            action.Invoke();
        }

        public virtual void PerformForMinimumTime(Action action, bool requiresUiThread, int minimumMillisecondsBeforeReturn)
        {
            action.Invoke();
        }

        public virtual void GracefulShutdown()
        {
            WasShutdown = true;
        }

        public void IgnoreException(Action action)
        {
            action.Invoke();
        }

        public void Sleep(int milliseconds)
        {
        }

        public void Sleep(TimeSpan sleepTime)
        {
        }

        public void ShowMessage(string message)
        {
        }
    }
}