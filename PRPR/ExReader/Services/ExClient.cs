using PRPR.ExReader.Models.Global;
using PRPR.ExReader.Controls;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static System.Text.RegularExpressions.Regex;
using System.Diagnostics;
using Windows.UI.Core;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using System.Linq;

namespace PRPR.ExReader.Services
{
    public class ExClient
    {
        internal static void SignOut()
        {
            ExSettings.Current.ECookie = "";

            // Remove all cookies used by the app
            HttpBaseProtocolFilter f = new HttpBaseProtocolFilter();
            var cookies = f.CookieManager.GetCookies(new Uri("https://exhentai.org/"));
            foreach (var cookie in cookies)
            {
                f.CookieManager.DeleteCookie(cookie);
            }
        }

        public static async Task<string> GetStringWithExCookie(string uriString, string uconfig = "")
        {
            HttpBaseProtocolFilter f = new HttpBaseProtocolFilter();
            var c2 = f.CookieManager.GetCookies(new Uri("https://exhentai.org/"));
            if (c2.FirstOrDefault(o=>o.Name == "igneous") != null)
            {
                // Remove the igneous:mystery cookie which cause sadpanda for new logins
                if (c2.FirstOrDefault(o => o.Name == "igneous").Value == "mystery")
                {
                    f.CookieManager.DeleteCookie(c2.FirstOrDefault(o => o.Name == "igneous"));
                }
            }

            string str;
            using (var client = new HttpClient(f))
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Cookie", await GetExCookieAsync(uconfig));
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                str = await client.GetStringAsync(new Uri(uriString));
            }
            
            return str;
        }

        


        private static async Task GetECookie(string username, string password)
        {
            // Prepare the login request
            string requestBody = $"UserName={username}&PassWord={password}&CookieDate=1";
            var message = new HttpRequestMessage(new HttpMethod("POST"), new Uri("https://forums.e-hentai.org/index.php?act=Login&CODE=01"))
            {
                Content = new HttpStringContent(requestBody)
            };
            message.Content.Headers.ContentType = new HttpMediaTypeHeaderValue("application/x-www-form-urlencoded");

            // Send request for the response
            HttpResponseMessage response;
            using (var httpClient = new HttpClient(new HttpBaseProtocolFilter()))
            {
                response = await httpClient.SendRequestAsync(message);
            }
            
            // Extract the e-hentai cookie
            string eCookie = response.Headers["Set-Cookie"];
            if (eCookie.IndexOf("expires=") >= 0)
            {
                var start = eCookie.IndexOf("expires=") + ("expires=".Length);
                var end = eCookie.IndexOf(";", start);
                var s = eCookie.Substring(start, end - start);
                try
                {
                    ExSettings.Current.ECookieExpire = DateTimeOffset.Parse(s).ToUniversalTime().DateTime;
                }
                catch (Exception ex)
                {
                    
                }
                ExSettings.Current.ECookie = eCookie;
            }
            else
            {
                throw new Exception("Cannot login");
            }
        }
        


        public static async Task AddToFavorite(string gid, string token, int favcat, string favnote = "")
        {
            string requestBody = $"apply=Add+to+Favorites&favcat={favcat}&favnote={favnote}&update=1";
            byte[] data = Encoding.UTF8.GetBytes(requestBody);
            HttpWebRequest loginRequest = WebRequest.CreateHttp($"https://exhentai.org/gallerypopups.php?gid={gid}&t={token}&act=addfav");
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Headers["Cookie"] = await GetExCookieAsync("");
            using (Stream stream = await loginRequest.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            
            using (HttpWebResponse logResponse = (HttpWebResponse)(await loginRequest.GetResponseAsync()))
            {
                using (Stream stream = logResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                    string responseString = reader.ReadToEnd();
                }
            }
        }

        public static async Task RemoveFromFavorite(string gid)
        {
            string requestBody = $"apply=Apply&ddact=delete&modifygids[]={gid}";
            byte[] data = Encoding.UTF8.GetBytes(requestBody);
            HttpWebRequest loginRequest = WebRequest.CreateHttp($"https://exhentai.org/favorites.php");
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Headers["Cookie"] = await GetExCookieAsync("");
            loginRequest.Headers["Accept-Encoding"] = "gzip, deflate";
            using (Stream stream = await loginRequest.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }
        }

        public static bool HasVaildECookie()
        {
            return ExSettings.Current.ECookie != "" && DateTime.UtcNow.Subtract(ExSettings.Current.ECookieExpire).TotalSeconds < 0;
        }
        
        public static async Task<string> GetExCookieAsync(string uconfig)
        {
            if (!HasVaildECookie())
            {
                var dialog1 = new LoginContentDialog();
                var result = await dialog1.ShowAsync();
                string username = "";
                string password = "";
                if (result == ContentDialogResult.Primary)
                {
                    username = dialog1.Username;
                    password = dialog1.Password;
                }
                else
                {
                    throw new Exception("Not Logged in");
                }

                await GetECookie(username, password);
            }
            else
            {

            }

            string manberid = CheckForMemberID(ExSettings.Current.ECookie);
            string passhash = CheckForPassHash(ExSettings.Current.ECookie);

            var _exCookie = $"ipb_member_id={manberid}; " + passhash + "; " + $"uconfig={uconfig};";
            return _exCookie;
        }

        private static string CheckForPassHash(string cookie)
        {
            string passhashRegexPattern = @"ipb_pass_hash=([^;]*)";
            var passhashRegex = Match(cookie, passhashRegexPattern);
            if (passhashRegex.Success)
            {
                return passhashRegex.Value;
            }
            else
            {
                throw new Exception("Login Error");
            }
        }
        
        public static string CheckForMemberID(string cookie)
        {
            string memberidRegexPattern = @"ipb_member_id=([^;]*)";
            var memberidRegex = Match(cookie, memberidRegexPattern);
            if (memberidRegex.Success)
            {
                return memberidRegex.Value.Replace("ipb_member_id=", "");
            }
            else
            {
                throw new Exception("Login Error");
            }
        }
    }
}
