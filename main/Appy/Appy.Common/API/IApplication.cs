namespace AppDirect.WindowsClient.Common.API
{
    public interface IApplication
    {
        string Id { get; set; }
        string Name { get; set; }
        string ImagePath { get; set; }
        string LocalImagePath { get; set; }
        string Description { get; set; }
        int AlertCount { get; set; }
        bool IsLocalApp { get; set; }
        string UrlString { get; set; }
    }
}