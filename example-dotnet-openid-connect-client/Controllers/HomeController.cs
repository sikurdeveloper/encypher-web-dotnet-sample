﻿/*
 * Copyright (C) 2017 Curity AB.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Configuration;
using System.Web.Mvc;
using exampledotnetopenidconnectclient.App_Start;
using Newtonsoft.Json.Linq;

namespace exampledotnetopenidconnectclient.Controllers
{
    
    public class HomeController : Controller
    {
        private static Helpers.Client _client = Helpers.Client.Instance;
        private string api_endpoint = App_Start.AppConfig.Instance.GetApiEndpoint();
        private string iss = App_Start.AppConfig.Instance.GetIssuer();

        static readonly HttpClient client = new HttpClient();

        public ActionResult Index()
        {
            ViewData["client_id"] = AppConfig.Instance.GetClientId();
            ViewData["client_secret"] = AppConfig.Instance.GetClientSecret();
            ViewData["server_name"] = iss;
            ViewData["server_name"] = iss;
            if (Session["error"] != null)
            {
                ViewData["error"] = Session["error"];
                Session["error"] = null;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Salvar(string clientId, string clientSecret)
        {
            AppConfig.Instance.Save(clientId, clientSecret);
            return RedirectToAction("Index");
        }

        public ActionResult Refresh()
        {
            try
            {
               String responseString = _client.Refresh(Session["refresh_token"].ToString());
				JObject jsonObj = JObject.Parse(responseString);
				Session["access_token"] = jsonObj.GetValue("access_token");
				Session["refresh_token"] = jsonObj.GetValue("refresh_token");
            }
			catch (OAuthClientException e)
			{
				Session["error"] = e.Message;
			}

            return Redirect("/");
        }

        public ActionResult Revoke()
        {
            try
            {
                _client.Revoke(Session["refresh_token"].ToString());
                Session["refresh_token"] = null;
            }
            catch (OAuthClientException e)
            {
                Session["error"] = e.Message;
            }
            

            return Redirect("/");
        }

        public ActionResult CallApi()
        {

            String access_token = Session["access_token"].ToString();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", access_token);

            var response = client.GetAsync(api_endpoint).Result;
            Session["api_response_status_code"] = response.StatusCode;

            var responseContent = response.Content;

            string responseString = responseContent.ReadAsStringAsync().Result;
            Session["api_response_data"] = responseString;

            return Redirect("/");
        }
    }
}
