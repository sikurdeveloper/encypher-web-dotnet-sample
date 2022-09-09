using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json.Linq;

namespace exampledotnetopenidconnectclient.App_Start
{

    public class AppConfig
    {

        private static AppConfig instance;

        private static String client_id;
        private static String client_secret;
        private static String redirect_uri;
        private static String scope;
        private static String authorization_endpoint;
        private static String token_endpoint;
        private static String logout_endpoint;
        private static String revocation_endpoint;
        private static String jwks_uri;
        private static String issuer;
        private static String api_endpoint;
        private static String base_url;
        private static String return_url;
        private static String response_type;

        private AppConfig()
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            redirect_uri = ConfigurationManager.AppSettings["redirect_uri"];
            scope = ConfigurationManager.AppSettings["scope"];
            authorization_endpoint = ConfigurationManager.AppSettings["authorization_endpoint"];
            token_endpoint = ConfigurationManager.AppSettings["token_endpoint"];
            logout_endpoint = ConfigurationManager.AppSettings["logout_endpoint"];
            revocation_endpoint = ConfigurationManager.AppSettings["revocation_endpoint"];
            jwks_uri = ConfigurationManager.AppSettings["jwks_uri"];
            issuer = ConfigurationManager.AppSettings["issuer"];
            api_endpoint = ConfigurationManager.AppSettings["api_endpoint"];
            base_url = ConfigurationManager.AppSettings["base_url"];
            return_url = ConfigurationManager.AppSettings["return_url"];
            response_type = ConfigurationManager.AppSettings["response_type"];

            if (!String.IsNullOrEmpty(issuer))
            {
                var discoveryClient = new HttpClient();

                var response = discoveryClient.GetAsync(issuer + "/.well-known/openid-configuration").Result;
                if (response.IsSuccessStatusCode)
                {
                    string responseString = response.Content.ReadAsStringAsync().Result;
                    JObject responseJson = JObject.Parse(responseString);

                    authorization_endpoint = responseJson["authorization_endpoint"].ToString();
                    token_endpoint = responseJson["token_endpoint"].ToString();
                    revocation_endpoint = responseJson["revocation_endpoint"].ToString();
                    logout_endpoint = responseJson["end_session_endpoint"].ToString();
                    jwks_uri = responseJson["jwks_uri"].ToString();

                }
            }

            string configLocation = HttpContext.Current.Server.MapPath("~/App_Data/config.json");
            string json = System.IO.File.ReadAllText(configLocation);
            var config = Newtonsoft.Json.JsonConvert.DeserializeObject<ClientConfig>(json);
            client_id = config.ClientId;
            client_secret = config.ClientSecret;
        }


        public String GetLogoutEndpoint()
        {
            return logout_endpoint;
        }

        public String GetClientId()
        {
            return client_id;
        }

        public String GetClientSecret()
        {
            return client_secret;
        }

        public String GetRedirectUri()
        {
            return redirect_uri;
        }

        public String GetScope()
        {
            return scope;
        }

        public String GetAuthorizationEndpoint()
        {
            return authorization_endpoint;
        }

        public String GetTokenEndpoint()
        {
            return token_endpoint;
        }

        public String GetRevocationEndpoint()
        {
            return revocation_endpoint;
        }

        public String GetJwksUri()
        {
            return jwks_uri;
        }

        public String GetIssuer()
        {
            return issuer;
        }

        public String GetApiEndpoint()
        {
            return api_endpoint;
        }

        public String GetBaseUrl()
        {
            return base_url;
        }

        public String GetReturnUri()
        {
            return return_url;
        }

        public String GetResponseType()
        {
            return response_type;
        }

        public void Save(string clientId, string clientSecret)
        {
            string configLocation = HttpContext.Current.Server.MapPath("~/App_Data/config.json");
            var config = new ClientConfig
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            string json = Newtonsoft.Json.JsonConvert.SerializeObject(config);
            System.IO.File.WriteAllText(configLocation, json);
            client_id = config.ClientId;
            client_secret = config.ClientSecret;
        }

        public static AppConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AppConfig();
                }
                return instance;
            }
        }
    }
}
