using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clox
{
    public class LoxRuntimeError : Exception
    {
       public readonly Token Token;
        public LoxRuntimeError(Token token)
        {
            Token = token;
        }
        public LoxRuntimeError(Token token, string message) : base(message) 
        {
            Token = token;
        }

        public LoxRuntimeError(Token token, string message, Exception inner) : base(message, inner) 
        {
            Token = token;
        }
    }
}
