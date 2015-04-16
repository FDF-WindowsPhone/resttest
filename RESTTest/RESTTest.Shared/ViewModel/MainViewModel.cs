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
using Windows.UI.Xaml.Controls;
using RESTTest.Models;
using WinRTXamlToolkit.Controls;
using WinRTXamlToolkit.Controls.Extensions;

namespace RESTTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {

        public MainViewModel()
        {
            Protocol = PROTOCOLS.HTTP;
            Method = 0;
        }

        private PROTOCOLS _protocol;

        public PROTOCOLS Protocol
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
            Result = "";
            string protocol = "";
            switch (Protocol)
            {
                case PROTOCOLS.HTTPS:
                    protocol = "https://";
                    break;
                case PROTOCOLS.HTTP:
                    protocol = "http://";
                    break;
                default:
                    protocol = "http://";
                    break;
            }

            string URI = string.Format("{0}{1}", protocol, Url);
            //string Parameters = Uri.EscapeUriString("");

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
            ResultCode = string.Format("STATUS: {0}", result.StatusCode);
        }

        #endregion Run Command

        #region Clean Command
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
            Protocol = PROTOCOLS.HTTP;
            Method = 0;
        }

        #endregion Clean Command

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
                    RTRequest request = new RTRequest() {Method = Method, Name = name, Protocol = Protocol, Url = Url};
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
        }

        #endregion Select Request Command

        #endregion Commands
    }
}