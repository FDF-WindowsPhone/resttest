using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace RESTTest.Models
{
    public class RTRequest
    {
        public string Name { get; set; }
        public int Protocol { get; set; }
        public string Url { get; set; }
        public int Method { get; set; }
        public string Raw { get; set; }
        public ObservableCollection<RTHeaders> Headers { get; set; }
    }
}
