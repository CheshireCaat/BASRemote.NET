using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASRemote.Requests
{ 
    public abstract class Request
    {
        protected Request(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
