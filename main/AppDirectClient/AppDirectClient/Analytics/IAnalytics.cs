namespace AppDirect.WindowsClient.Analytics
{
    public interface IAnalytics
    {
        void Notify(string action, string label, int? value);
        void NotifySimpleAction(string action);
    }
}