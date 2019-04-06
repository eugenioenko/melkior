using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Melkior
{
    class MelkiorException: Exception
    {
        public readonly Token token;
        public readonly string message;

        public MelkiorException(string message, Token token = null)
        {
            this.message = message;
            this.token = token;
        }
    }
}
