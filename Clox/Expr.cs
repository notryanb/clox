namespace Clox
{
	public abstract class Expr
	{
		public interface IVisitor
		{
			public void VisitBinaryExpr(Binary expr);
			public void VisitGroupingExpr(Grouping expr);
			public void VisitLiteralExpr(Literal expr);
			public void VisitUnaryExpr(Unary expr);
		}

		public abstract void Accept(IVisitor visitor);

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

			public override void Accept(IVisitor visitor)
			{
				visitor.VisitBinaryExpr(this);
			}
		}

		public class Grouping : Expr 
		{
			public Expr Left;
			public Token @Operator;
			public Expr Right;

			public Grouping(Expr left, Token @operator, Expr right)
			{
				Left = left;
				@Operator = @operator;
				Right = right;
			}

			public override void Accept(IVisitor visitor)
			{
				visitor.VisitGroupingExpr(this);
			}
		}

		public class Literal : Expr 
		{
			public Expr Left;
			public Token @Operator;
			public Expr Right;

			public Literal(Expr left, Token @operator, Expr right)
			{
				Left = left;
				@Operator = @operator;
				Right = right;
			}

			public override void Accept(IVisitor visitor)
			{
				visitor.VisitLiteralExpr(this);
			}
		}

		public class Unary : Expr 
		{
			public Expr Left;
			public Token @Operator;
			public Expr Right;

			public Unary(Expr left, Token @operator, Expr right)
			{
				Left = left;
				@Operator = @operator;
				Right = right;
			}

			public override void Accept(IVisitor visitor)
			{
				visitor.VisitUnaryExpr(this);
			}
		}

	}
}
