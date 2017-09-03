using System.Text;

namespace Lox
{
    public class AstPrinter : Expr.Visitor<string> 
    {
        public string print(Expr expr) 
        {
            return expr.accept(this);
        }

        public string visitAssignExpr(Expr.Assign expr)
        {
            return $"Assign {expr.name.lexeme}";
        }

        public string visitBinaryExpr(Expr.Binary expr) {
            return parenthesize(expr.op.lexeme, expr.left, expr.right);
        }

        public string visitGroupingExpr(Expr.Grouping expr) {
            return parenthesize("group", expr.expression);
        }

        public string visitLiteralExpr(Expr.Literal expr)
        {
            return expr.value == null ? "nil" : expr.value.ToString();
        }

        public string visitUnaryExpr(Expr.Unary expr) {
            return parenthesize(expr.op.lexeme, expr.right);
        }

        public string visitVariableExpr(Expr.Variable expr)
        {
            return $"Visit {expr.name.lexeme}";
        }

        private string parenthesize(string name, params Expr[] exprs) {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (Expr expr in exprs) {
                builder.Append(" ");
                builder.Append(expr.accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}