namespace Clox
{
	public abstract class Expr
	{
		public interface IVisitor<T>
		{
			public T VisitBinaryExpr(Binary expr);
			public T VisitGroupingExpr(Grouping expr);
			public T VisitLiteralExpr(Literal expr);
			public T VisitUnaryExpr(Unary expr);
		}

		public abstract T Accept<T>(IVisitor<T> visitor);

		public class Binary : Expr 
		{
			public Expr Left;
			public Token @Operator;
			public Expr Right;

			public Binary(Expr left, Token @operator, Expr right)
			{
				Left = left;
				@Operator = @operator;
				Right = right;
			}

			public override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitBinaryExpr(this);
			}
		}

		public class Grouping : Expr 
		{
			public Expr Expression;

			public Grouping(Expr expression)
			{
				Expression = expression;
			}

			public override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitGroupingExpr(this);
			}
		}

		public class Literal : Expr 
		{
			public System.Object Value;

			public Literal(System.Object value)
			{
				Value = value;
			}

			public override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitLiteralExpr(this);
			}
		}

		public class Unary : Expr 
		{
			public Token @Operator;
			public Expr Right;

			public Unary(Token @operator, Expr right)
			{
				@Operator = @operator;
				Right = right;
			}

			public override T Accept<T>(IVisitor<T> visitor)
			{
				return visitor.VisitUnaryExpr(this);
			}
		}

	}
}
