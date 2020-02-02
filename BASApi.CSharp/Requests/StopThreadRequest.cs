using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BASRemote.Requests
{
    class StopThreadRequest : Request
    {
        public StopThreadRequest(string name) : base("stop_thread")
        {
        }
    }
}
