using System;

namespace Lox
{
    public class Interpreter : Expr.Visitor<object> 
    {
        public void interpret(Expr expression) 
        {
            try 
            {
                object value = evaluate(expression);
                Console.WriteLine(stringify(value));
            } 
            catch (RuntimeError error) 
            {
                Lox.runtimeError(error);
            }
        }
        
        public object visitBinaryExpr(Expr.Binary expr)
        {
            object left = evaluate(expr.left);
            object right = evaluate(expr.right); 

            switch (expr.op.type) {
                case TokenType.MINUS:
                    checkNumberOperands(expr.op, left, right);
                    return (double)left - (double)right;
                case TokenType.SLASH:
                    checkNumberOperands(expr.op, left, right);
                    if ((double) right != 0.0)
                    {
                        return (double)left / (double)right;
                    }
                    throw new RuntimeError(expr.op,
                        "Division by zero");
                case TokenType.STAR:
                    checkNumberOperands(expr.op, left, right);
                    return (double)left * (double)right;
                case TokenType.PLUS:
                    if (left is double && right is double) 
                    {
                        return (double)left + (double)right;
                    } 
                    if (left is string && right is string) 
                    {
                        return (string)left + (string)right;
                    }
                    if (left is string && !(right is string))
                    {
                        return (string) left + right;
                    }
                    if (!(left is string) && right is string)
                    {
                        return left + (string) right;
                    }
                    throw new RuntimeError(expr.op,
                        "Operands must be two numbers or two strings.");
                case TokenType.GREATER:
                    checkNumberOperands(expr.op, left, right);
                    return (double)left > (double)right;
                case TokenType.GREATER_EQUAL:
                    checkNumberOperands(expr.op, left, right);
                    return (double)left >= (double)right;
                case TokenType.LESS:
                    checkNumberOperands(expr.op, left, right);
                    return (double)left < (double)right;
                case TokenType.LESS_EQUAL:
                    checkNumberOperands(expr.op, left, right);
                    return (double)left <= (double)right;
                case TokenType.BANG_EQUAL: return !isEqual(left, right);
                case TokenType.EQUAL_EQUAL: return isEqual(left, right);
            }

            // Unreachable.
            return null;
        }

        public object visitGroupingExpr(Expr.Grouping expr)
        {
            return evaluate(expr.expression);
        }

        public object visitLiteralExpr(Expr.Literal expr)
        {
            return expr.value;
        }

        public object visitUnaryExpr(Expr.Unary expr)
        {
            object right = evaluate(expr.right);

            switch (expr.op.type)
            {
                case TokenType.MINUS:
                    checkNumberOperand(expr.op, right);
                    return -(double)right;
                case TokenType.BANG:
                    return !isTruthy(right);
            }

            // Unreachable.
            return null;        
        }
        
        private object evaluate(Expr expr) 
        {
            return expr.accept(this);
        }
        
        private static bool isTruthy(object obj) 
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }
        
        private static bool isEqual(object a, object b) 
        {
            // nil is only equal to nil.
            if (a == null && b == null) return true;
            if (a == null) return false;

            return a.Equals(b);
        }
        
        private static void checkNumberOperand(Token op, object operand) 
        {
            if (operand is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }
        
        private static void checkNumberOperands(Token op, object left, object right) 
        {
            if (left is double && right is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }
        
        private static string stringify(object obj) 
        {
            return obj == null ? "nil" : obj.ToString();
        }
    }
}