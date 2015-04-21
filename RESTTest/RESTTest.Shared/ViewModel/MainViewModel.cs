using System.IO;
using System.Net;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http.Headers;
using System.Net.Http;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RESTTest.Models;
using WinRTXamlToolkit.Controls;
using WinRTXamlToolkit.Controls.Extensions;

namespace RESTTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel()
        {
            Protocol = 0;
            Method = 0;
            WaitVisibility = Visibility.Collapsed;
        }

        private int _protocol;
        public int Protocol
        {
            get { return _protocol; }
            set
            {
                _protocol = value;
                RaisePropertyChanged();
            }
        }
        
        private string _url;
        public string Url
        {
            get { return _url; }
            set
            {
                _url = value;
                RaisePropertyChanged();
            }
        }

        private int _method;
        public int Method
        {
            get { return _method; }
            set
            {
                _method = value;
                RaisePropertyChanged();
            }
        }

        private string _raw;
        public string Raw
        {
            get { return _raw; }
            set
            {
                _raw = value;
                RaisePropertyChanged();
            }
        }

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

        private string _resultCode;
        public string ResultCode
        {
            get
            {
                return _resultCode;
            }
            set
            {
                _resultCode = value;
                RaisePropertyChanged();
            }
        }

        private Visibility _waitVisibility;
        public Visibility WaitVisibility
        {
            get { return _waitVisibility; }
            set
            {
                _waitVisibility = value;
                RaisePropertyChanged();
            }
        }

        private ObservableCollection<RTRequest> _requestsList = new ObservableCollection<RTRequest>();
        public ObservableCollection<RTRequest> RequestsList
        {
            get { return _requestsList; }
            set
            {
                _requestsList = value;
                RaisePropertyChanged();
            }
        } 

        #region Commands

        #region Run Command
        private RelayCommand _runCommand;
        public RelayCommand RunCommand
        {
            get { return _runCommand ?? (_runCommand = new RelayCommand(Run)); }
        }

        private async void Run()
        {
            WaitVisibility = Visibility.Visible;

            Result = "";
            string protocol = "";
            switch (Protocol)
            {
                case RTConsts.PROTOCOL_HTTP:
                    protocol = "http://";
                    break;
                case RTConsts.PROTOCOL_HTTPS:
                    protocol = "https://";
                    break;                
                default:
                    protocol = "http://";
                    break;
            }

            HttpMethod method = HttpMethod.Get;
            switch (Method)
            {
                case RTConsts.METHOD_GET:
                    method = HttpMethod.Get;
                    break;
                case RTConsts.METHOD_POST:
                    method = HttpMethod.Post;
                    break;
            }

            string URI = string.Format("{0}{1}", protocol, Url);
            //string Parameters = Uri.EscapeUriString("");

            HttpClient client = new HttpClient();

            HttpRequestMessage request = new HttpRequestMessage(method, URI);
            if (Raw != null)
            {
                try
                {
                    var jsonedString = JToken.Parse(Raw);
                    client.DefaultRequestHeaders.ExpectContinue = false;
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    request.Content = new StringContent(Raw, System.Text.Encoding.UTF8, "application/json");
                    
                }
                catch (JsonReaderException jex)
                {
                    ResultCode = "JSON MALFORMED";
                    Result = string.Format("{0}", jex.Message);
                    WaitVisibility = Visibility.Collapsed;

                    return;
                }
                catch (Exception ex)
                {
                    ResultCode = "EXCEPTION";
                    Result = string.Format("EXCEPTION {0}", ex.Message);
                    WaitVisibility = Visibility.Collapsed;

                    return;
                }                
            }
            
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
            try
            {
                var result = await client.SendAsync(request);
                var content = await result.Content.ReadAsStringAsync();

                try
                {
                    var jsonedString = JToken.Parse(content);
                    Result = jsonedString.ToString();

                }
                catch (JsonReaderException jex)
                {
                    Result = content;    
                }                
                ResultCode = string.Format("{0}", result.StatusCode);
            }
            catch (Exception exc)
            {
                ResultCode = "EXCEPTION";
                Result = string.Format("EXCEPTION {0}", exc.Message); 
            }
            

            WaitVisibility = Visibility.Collapsed;
        }

        #endregion Run Command

        #region Clean All Command
        private RelayCommand _cleanCommand;
        public RelayCommand CleanCommand
        {
            get { return _cleanCommand ?? (_cleanCommand = new RelayCommand(Clean)); }
        }

        private async void Clean()
        {
            Result = "";
            ResultCode = "";
            Url = "";
            Raw = "";
            Protocol = RTConsts.PROTOCOL_HTTP;
            Method = RTConsts.METHOD_GET;
        }

        #endregion Clean All Command

        #region Save Command
        private RelayCommand _saveCommand;
        public RelayCommand SaveCommand
        {
            get { return _saveCommand ?? (_saveCommand = new RelayCommand(Save)); }
        }

        private async void Save()
        {
            InputDialog dialog = new InputDialog();
            
            var result = await dialog.ShowAsync(
                "Attention",
                "Enter an Identifier for the Connection",
                "Save",
                "Cancel");

            switch (result)
            {
                case "Save":
                    string name = dialog.InputText;

                    RTRequest request = new RTRequest() {Method = Method, Name = name, Protocol = Protocol, Url = Url, Raw = Raw };
                    RequestsList.Add(request);
                    break;
                case "Cancel":
                    break;
            }            
            
        }

        #endregion Save Command

        #region Select Request Command
        private RelayCommand<ItemClickEventArgs>  _selectRequestCommand;
        public RelayCommand<ItemClickEventArgs> SelectRequestCommand
        {
            get { return _selectRequestCommand ?? (_selectRequestCommand = new RelayCommand<ItemClickEventArgs>(SelectRequest)); }
        }

        private async void SelectRequest(ItemClickEventArgs args)
        {
            RTRequest request = args.ClickedItem as RTRequest;
            string message = string.Format("Do you want to load {0}", request.Name);
            MessageDialog dialog = new MessageDialog(message, "Attention!!");
            dialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(YesCommandHandler), request));
            dialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(NoCommandHandler)));
            dialog.DefaultCommandIndex = 0;
            dialog.CancelCommandIndex = 1;
            await dialog.ShowAsync();
        }

        private void NoCommandHandler(IUICommand command)
        {
            // do Nothing
        }

        private void YesCommandHandler(IUICommand command)
        {
            RTRequest request = command.Id as RTRequest;
            Url = request.Url;
            Method = request.Method;
            Protocol = request.Protocol;
            Raw = request.Raw;
        }

        #endregion Select Request Command

        #endregion Commands

        public string GetAsJSON()
        {
            string json = JsonConvert.SerializeObject(RequestsList);

            return json;
        }

        public void LoadFromJSON(string json)
        {
            RequestsList = JsonConvert.DeserializeObject<ObservableCollection<RTRequest>>(json);
        }
    }
}