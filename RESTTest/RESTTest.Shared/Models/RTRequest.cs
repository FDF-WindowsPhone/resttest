using System;
using System.Collections.Generic;
using System.Text;

namespace RESTTest.Models
{
    public class RTRequest
    {
        public string Name { get; set; }
        public int Protocol { get; set; }
        public string Url { get; set; }
        public int Method { get; set; }

    }
}
