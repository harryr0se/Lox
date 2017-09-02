using System;

namespace Lox
{
    public class RuntimeError : Exception 
    {
        public Token token;

        public RuntimeError(Token token, string message) 
        : base(message)
        {
            this.token = token;
        }
    }
}