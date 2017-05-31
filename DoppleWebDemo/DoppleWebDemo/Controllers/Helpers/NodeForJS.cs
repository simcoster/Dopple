using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mono.Cecil;

namespace DoppleWebDemo.Controllers.Helpers
{
    public class NodeForJS
    {
        public int key { get; set; }
        public string text { get; set; }
        public string color { get; set; }
        public string method { get; internal set; }
    }
}