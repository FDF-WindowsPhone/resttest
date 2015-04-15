using System.IO;
using System.Net;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Net.Http.Headers;
using System.Net.Http;

namespace RESTTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string Url { get; set; }
        public string Method { get; set; }

        private string _result;
        public string Result { 
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                RaisePropertyChanged();
            }
        }

        #region Run Command
        private RelayCommand _runCommand;
        public RelayCommand RunCommand
        {
            get { return _runCommand ?? (_runCommand = new RelayCommand(Run)); }
        }

        private async void Run()
        {
            string URI = Url;
            string Parameters = Uri.EscapeUriString("");

            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, URI);
            /*
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
            request.Headers.AcceptCharset.Add(new StringWithQualityHeaderValue("utf-8", 0.7));
            request.Headers.AcceptLanguage.Add(new StringWithQualityHeaderValue("en-us", 0.5));
            
            request.Content = new StreamContent(new MemoryStream(System.Text.Encoding.UTF8.GetBytes(Parameters)));
            request.Content.Headers.Add("Content-Type", "application/x-www-form-urlencoded");
             
            request.Headers.Host = "www.indianrail.gov.in";
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Mozilla", "5.0"));
            request.Headers.Referrer = new Uri("http://www.indianrail.gov.in/pnr_stat.html");
            */
            var result = await client.SendAsync(request);
            var content = await result.Content.ReadAsStringAsync();

            Result = content;
        }

        #endregion Run Command

        public MainViewModel()
        {
            
        }
    }
}