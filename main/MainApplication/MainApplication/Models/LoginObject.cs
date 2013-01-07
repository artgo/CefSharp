using System.Xml.Serialization;

namespace AppDirect.WindowsClient.Models
{
    ///<summary>
    ///Represents the User's Login Credentials 
    ///</summary>
    public class LoginObject
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
