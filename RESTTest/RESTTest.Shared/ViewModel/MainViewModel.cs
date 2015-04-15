using System.IO;
using System.Net;
using System.Text;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace RESTTest.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public string Url { get; set; }
        public string Method { get; set; }

        #region Run Command
        private RelayCommand _runCommand;
        public RelayCommand RunCommand
        {
            get { return _runCommand ?? (_runCommand = new RelayCommand(Run)); }
        }

        private void Run()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = Method;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "Accept=text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            /*
            byte[] _byteVersion = Encoding.ASCII.GetBytes(string.Concat("content=", dlc_content));

            request.ContentLength = _byteVersion.Length

            Stream stream = request.GetRequestStream();
            stream.Write(_byteVersion, 0, _byteVersion.Length);
            stream.Close();

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                Console.WriteLine(reader.ReadToEnd());
            }*/
        }

        #endregion Run Command

        public MainViewModel()
        {
            
        }
    }
}