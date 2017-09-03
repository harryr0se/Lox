using System;
using System.Collections.Generic;

namespace Lox
{
    public class Environment 
    {
        private readonly Dictionary<string, object> values = new Dictionary<string, object>();
        private readonly Environment enclosing = null;
        private static readonly object unassigned = new object();
        
        public Environment(Environment enclosing) {
            this.enclosing = enclosing;
        }
        
        public void define(string name, object value) 
        {
            values[name] = value;
        }

        public void define(string name)
        {
            values[name] = unassigned;
        }

        public object get(Token name) 
        {
            if (values.ContainsKey(name.lexeme)) 
            {
                object value = values[name.lexeme];
                if (value == unassigned)
                {
                    throw new RuntimeError(name,
                        "Attempted to use unassigned variable '" + name.lexeme + "'.");
                }

                return value;
            }

            if (enclosing != null)
            {
                return enclosing.get(name);
            }

            throw new RuntimeError(name,
                "Undefined variable '" + name.lexeme + "'.");
        }
        
        public void assign(Token name, object value) 
        {
            if (values.ContainsKey(name.lexeme)) 
            {
                values[name.lexeme] = value;
                return;
            }
            
            if (enclosing != null) 
            {
                enclosing.assign(name, value);
                return;
            }

            throw new RuntimeError(name,
                "Undefined variable '" + name.lexeme + "'.");
        }
    }
}