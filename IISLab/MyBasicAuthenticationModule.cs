using System;
using System.Security.Principal;
using System.Text;
using System.Web;

namespace IISLab
{
    public class MyBasicAuthenticationModule : IHttpModule
    {
        public const String HttpAuthorizationHeader = "Authorization";  // HTTP1.1 Authorization header 
        public const String     HttpBasicSchemeName = "Basic"; // HTTP1.1 Basic Challenge Scheme Name  
        public const Char       HttpCredentialSeparator = ':'; // HTTP1.1 Credential username and password separator
        public const int        HttpNotAuthorizedStatusCode = 401; // HTTP1.1 Not authorized response status code
        public const String     HttpWWWAuthenticateHeader = "WWW-Authenticate"; // HTTP1.1 Basic Challenge Scheme Name 
        public const String     Realm = "demo"; // HTTP.1.1 Basic Challenge Realm 
        /// <summary>
        /// You will need to configure this module in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpModule Members

        public void Dispose()
        {
            //clean-up code here.
        }

        public void Init(HttpApplication context)
        {
            // Below is an example of how you can handle LogRequest event and provide 
            // custom logging implementation for it
            context.LogRequest += new EventHandler(OnLogRequest);
            context.AuthenticateRequest += Context_AuthenticateRequest;
            context.EndRequest += Context_EndRequest;
        }

        private void Context_EndRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;
            if (context.Response.StatusCode == HttpNotAuthorizedStatusCode)
            {
                context.Response.AddHeader(HttpWWWAuthenticateHeader, $"Basic realm='{Realm}'");
            }
        }

        private void Context_AuthenticateRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            var context = app.Context;
            var authorizationHeader = context.Request.Headers[HttpAuthorizationHeader];

            var username = string.Empty;
            var pwd = string.Empty;
            if(!this.ExtractBasicCredentials(authorizationHeader,ref username,ref pwd))
            {
                return;
            }

            if (!this.ValidateCredentials(username, pwd, null))
            {
                return;
            }

            context.User = new GenericPrincipal(new GenericIdentity(username),null);
        }

        protected virtual bool ExtractBasicCredentials(string authHeader,ref string userName,ref string password)
        {
            if (string.IsNullOrEmpty(authHeader))
            {
                return false;
            }
            if (authHeader.Trim().IndexOf(HttpBasicSchemeName)!=0)
            {
                return false;
            }
            authHeader = authHeader.Substring(HttpBasicSchemeName.Length, authHeader.Length);
            var credentialBase64DecodedArray = Convert.FromBase64String(authHeader);
            var decodedAuthorizationHeader = Encoding.UTF8.GetString(credentialBase64DecodedArray);
            int separatorPosition = decodedAuthorizationHeader.IndexOf(HttpCredentialSeparator);
            if(separatorPosition<0)
            {
                return false;
            }
            userName = decodedAuthorizationHeader.Substring(0, separatorPosition);
            password = decodedAuthorizationHeader.Substring(separatorPosition + 1, decodedAuthorizationHeader.Length - separatorPosition);
            if(string.IsNullOrEmpty(userName)||string.IsNullOrEmpty(password))
            {
                return false;
            }
            // decode the base 64 encodeded credential payload 
            return false;
        }

        protected virtual bool ValidateCredentials(string userName,string password,string realm)
        {
            if (userName.Equals("test") && password.Equals("test"))
            {
                return true;
            }
            return false;
        }

        #endregion

        public void OnLogRequest(Object source, EventArgs e)
        {
            //custom logging logic can go here
        }
    }
}
