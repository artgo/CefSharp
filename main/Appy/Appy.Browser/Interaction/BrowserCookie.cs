using Gecko;

namespace AppDirect.WindowsClient.Browser.Interaction
{
    public class BrowserCookie
    {
        private readonly long _creationTime;
        private readonly long _expiry;
        private readonly string _host;
        private readonly bool _isDomain;
        private readonly bool _isHttpOnly;
        private readonly bool _isSecure;
        private readonly bool _isSession;
        private readonly long _lastAccessed;
        private readonly string _name;
        private readonly string _path;
        private readonly string _rawHost;
        private readonly string _value;

        public BrowserCookie(Cookie intCookie)
        {
            _creationTime = intCookie.CreationTime;
            _expiry = intCookie.Expiry;
            _host = intCookie.Host;
            _isDomain = intCookie.IsDomain;
            _isHttpOnly = intCookie.IsHttpOnly;
            _isSecure = intCookie.IsSecure;
            _isSession = intCookie.IsSession;
            _lastAccessed = intCookie.LastAccessed;
            _name = intCookie.Name;
            _path = intCookie.Path;
            _rawHost = intCookie.RawHost;
            _value = intCookie.Value;
        }

        public long CreationTime { get { return _creationTime; } }
        public long Expiry { get { return _expiry; } }
        public string Host { get { return _host; } }
        public bool IsDomain { get { return _isDomain; } }
        public bool IsHttpOnly { get { return _isHttpOnly; } }
        public bool IsSecure { get { return _isSecure; } }
        public bool IsSession { get { return _isSession; } }
        public long LastAccessed { get { return _lastAccessed; } }
        public string Name { get { return _name; } }
        public string Path { get { return _path; } }
        public string RawHost { get { return _rawHost; } }
        public string Value { get { return _value; } }
    }
}