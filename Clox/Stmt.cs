namespace Clox
{
	public abstract class Stmt
	{
		public interface IVisitor<T>
		{
			public T VisitExpressionStmt(Expression stmt);
			public T VisitPrintStmt(Print stmt);
		}

		public abstract T Accept<T>(IVisitor<T> visitor);

		public class Expression : Stmt 
		{
			public Expr ExpressionValue;

			public Expression(Expr expression)
			{
				ExpressionValue = expression;
			}

			public override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitExpressionStmt(this);
			}
		}

		public class Print : Stmt 
		{
			public Expr ExpressionValue;

			public Print(Expr expression)
			{
				ExpressionValue = expression;
			}

			public override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitPrintStmt(this);
			}
		}
	}
}
