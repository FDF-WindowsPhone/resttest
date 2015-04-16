using System;
using System.Collections.Generic;
using System.Text;

namespace RESTTest.Models
{
    public enum PROTOCOLS
    {
        HTTP,
        HTTPS
    }

    public class RTRequest
    {
        public string Name { get; set; }
        public PROTOCOLS Protocol { get; set; }
        public string Url { get; set; }
        public int Method { get; set; }

    }
}
