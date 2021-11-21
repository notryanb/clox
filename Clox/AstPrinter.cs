using System.Collections.Generic;
using System.Text;

namespace Clox
{
    public class AstPrinter : Expr.IVisitor<string>
    {
        public string Print(Expr exr)
        {
            return exr.Accept(this);
        }

        public string VisitBinaryExpr(Expr.Binary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, new Expr[] { expr.Left, expr.Right });
        }

        public string VisitGroupingExpr(Expr.Grouping expr)
        {
            return Parenthesize("group", new Expr[] { expr.Expression });
        }

        public string VisitLiteralExpr(Expr.Literal expr)
        {
            if (expr == null) return "nil";
            return expr.Value.ToString();
        }

        public string VisitUnaryExpr(Expr.Unary expr)
        {
            return Parenthesize(expr.Operator.Lexeme, new Expr[] { expr.Right });
        }

        private string Parenthesize(string name, IEnumerable<Expr> exprs)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append('(').Append(name);
            foreach (var expr in exprs)
            {
                sb.Append(" ");
                sb.Append(expr.Accept(this));
            }

            sb.Append(')');

            return sb.ToString();
        }
    }
}
