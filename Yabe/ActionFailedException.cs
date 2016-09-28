using System;
using System.Collections.Generic;
using System.Text;

namespace Yabe
{
    class ActionFailedException : Exception
    {
        public string errorMessage { get; set; }
        public int actionId { get; set; }
    }
}
