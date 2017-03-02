using PRPR.ExReader.Models.Global;
using PRPR.ExReader.Views.Controls;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using static System.Text.RegularExpressions.Regex;
using System.Diagnostics;
using Windows.UI.Core;

namespace PRPR.ExReader.Services
{
    public class ExClient : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        // NotifyPropertyChanged will raise the PropertyChanged event, 
        // passing the source property that is being updated.
        public async void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                try
                {
                    CoreDispatcher dispatcher = CoreWindow.GetForCurrentThread().Dispatcher;
                    await dispatcher.RunAsync(CoreDispatcherPriority.Low, () =>
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                    });

                }
                catch (Exception ex)
                {
                    while (ex != null)
                    {
                        Debug.WriteLine(ex.Message);
                        ex = ex.InnerException;
                    }
                }
            }
        }

        #endregion





        internal static void SignOut()
        {
            ExSettings.Current.ECookie = "";
        }


        public async Task SignInAsync(string id, string password)
        {
            try
            {
                var handler = new HttpClientHandler { AllowAutoRedirect = true };
                HttpClient client = new HttpClient(handler);


                var response = await client.GetByteArrayAsync("http://exhentai.org/");
                var s =  Encoding.UTF8.GetString(response, 0, response.Length);

                
                //return await client.GetStringAsync(uri);
            }
            catch (Exception ex)
            {
                
            }
        }

        


        public static async Task<string> GetStringWithExCookie(string uriString, string uconfig = "")
        {
            using (var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None, UseCookies = false, })
            using (var client = new HttpClient(handler))
            {
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Add("Cookie", await GetExCookieAsync(uconfig));
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                var str = await client.GetStringAsync(uriString);
                return str;
            }
        }

        
        private static async Task GetECookie(string username, string password)
        {
            string requestBody = $"UserName={username}&PassWord={password}&CookieDate=1";
            byte[] data = Encoding.UTF8.GetBytes(requestBody);
            HttpWebRequest loginRequest = WebRequest.CreateHttp("https://forums.e-hentai.org/index.php?act=Login&CODE=01");
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            using (Stream stream = await loginRequest.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }
            string eCookie = null;
            using (HttpWebResponse logResponse = (HttpWebResponse)(await loginRequest.GetResponseAsync()))
            {
                eCookie = logResponse.Headers["Set-Cookie"];
            }

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
            HttpWebRequest loginRequest = WebRequest.CreateHttp($"http://exhentai.org/gallerypopups.php?gid={gid}&t={token}&act=addfav");
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Headers["Cookie"] = await GetExCookieAsync("");
            //loginRequest.Headers["Accept-Encoding"] = "gzip, deflate";
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
            HttpWebRequest loginRequest = WebRequest.CreateHttp($"http://exhentai.org/favorites.php");
            loginRequest.Method = "POST";
            loginRequest.ContentType = "application/x-www-form-urlencoded";
            loginRequest.Headers["Cookie"] = await GetExCookieAsync("");
            loginRequest.Headers["Accept-Encoding"] = "gzip, deflate";
            using (Stream stream = await loginRequest.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }


            //using (HttpWebResponse logResponse = (HttpWebResponse)(await loginRequest.GetResponseAsync()))
            //{

            //}
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
            //string igneous = await CheckCookieForAccess(manberid, passhash);


            var _exCookie = $"ipb_member_id={manberid};" + passhash + ";" + $"uconfig={uconfig};";
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

        private static async Task<string> CheckCookieForAccess(string manberid, string passhash)
        {
            HttpWebRequest webRequest = WebRequest.CreateHttp("http://exhentai.org/");
            webRequest.Headers["Cookie"] = manberid + ";" + passhash;
            string imgCookie = "";
            using (HttpWebResponse webResponse = await webRequest.GetResponseAsync() as HttpWebResponse)
            {
                if (webResponse.ContentType == "image/gif")
                {
                    throw new Exception("No Access");
                }
                imgCookie = webResponse.Headers["Set-Cookie"];
            }
            string igneousRegexPattern = @"igneous=([^;]*)";
            var igneousRegex = Match(imgCookie, igneousRegexPattern);
            var igneous = igneousRegex.Value;
            return igneousRegex.Success ? igneous : "igneous=";
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
