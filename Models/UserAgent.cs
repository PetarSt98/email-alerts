using Microsoft.AspNetCore.Http;
using UAParser;

namespace email_alerts.Models
{
    public class UserIdentity
    {
        public string Username { get; private set; }

        public UserIdentity(IHeaderDictionary headers)
        {
            Username = headers["X-Username"].ToString(); // Assuming 'X-Username' is the header key where username is stored.
        }
    }
}
