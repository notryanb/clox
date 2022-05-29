using System;
using System.Collections.Generic;

namespace Clox
{
    public class Interpreter : Expr.IVisitor<object>, Stmt.IVisitor<object>
    {
        public void Interpret(IList<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (LoxRuntimeError ex)
            {
                Program.RuntimeError(ex);
            }
        }

        private void Execute(Stmt statement)
        {
            statement.Accept(this);
        }

        private string Stringify(object obj)
        {
            if (obj == null) return "nil";

            if (obj is double)
            {
                string text = obj.ToString();

                if (text.EndsWith(".0"))
                {
                    text = text = text.Substring(0, text.Length - 2);
                }

                return text;
            }

            return obj.ToString();
        }

        public object VisitBinaryExpr(Expr.Binary expr)
        {
            object left = Evaluate(expr.Left);
            object right = Evaluate(expr.Right);

            return expr.Operator.Type switch
            {
                TokenType.BANG_EQUAL => !IsEqual(CheckNumberOperand(expr.Operator, left), CheckNumberOperand(expr.Operator, right)),
                TokenType.EQUAL_EQUAL => !IsEqual(CheckNumberOperand(expr.Operator, left), CheckNumberOperand(expr.Operator, right)),
                TokenType.GREATER => CheckNumberOperand(expr.Operator, left) > CheckNumberOperand(expr.Operator, right),
                TokenType.GREATER_EQUAL => CheckNumberOperand(expr.Operator, left) >= CheckNumberOperand(expr.Operator, right),
                TokenType.LESS => CheckNumberOperand(expr.Operator, left) < CheckNumberOperand(expr.Operator, right),
                TokenType.LESS_EQUAL => CheckNumberOperand(expr.Operator, left) <= CheckNumberOperand(expr.Operator, right),
                TokenType.MINUS => CheckNumberOperand(expr.Operator, left) - CheckNumberOperand(expr.Operator, right),
                TokenType.PLUS => HandlePlus(expr.Operator, left, right),
                TokenType.SLASH => CheckNumberOperand(expr.Operator, left) / CheckDivideByZero(expr.Operator, CheckNumberOperand(expr.Operator, right)),
                TokenType.STAR => CheckNumberOperand(expr.Operator, left) * CheckNumberOperand(expr.Operator, right),
                _ => null, // this should be unreachable
            };
        }

        public object VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object VisitUnaryExpr(Expr.Unary expr)
        {
            object right = Evaluate(expr.Right);

            return expr.Operator.Type switch
            {
                TokenType.MINUS => -(CheckNumberOperand(expr.Operator, right)),
                TokenType.BANG => !IsTruthy(right),
                _ => null, // should be unreachable, maybe throw an exception?
            };
        }

        private object Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private bool IsTruthy(object obj)
        {
            if (obj == null) return false;
            if (obj is bool) return (bool)obj;
            return true;
        }

        private bool IsEqual(object left, object right)
        {
            if (left == null && right == null) return true;
            if (left == null) return false;
            return left.Equals(right);
        }

        private object HandlePlus(Token tokenOp, object left, object right)
        {

            if (left is double && right is double)
            {
                return (double)left + (double)right;
            }

            if (left is string && right is string)
            {
                return (string)left + (string)right;
            }

            if (left is string && right is double)
            {
                return (string)left + right.ToString();
            }

            if (left is double && right is string)
            {
                return left.ToString() + (string)right;
            }

            throw new LoxRuntimeError(tokenOp, "Operands must be numbers or strings.");
        }

        private double CheckNumberOperand(Token tokenOp, object operand)
        {
            if (operand is double) return (double)operand;
            throw new LoxRuntimeError(tokenOp, "Operand must be a number");
        }

        private double CheckDivideByZero(Token tokenOp, double operand)
        {
            if (operand != 0) return operand;
            throw new LoxRuntimeError(tokenOp, "Can't divide by Zero.");
        }

        public object VisitExpressionStmt(Stmt.Expression stmt)
        {
            Evaluate(stmt.ExpressionValue);
            return null;
        }

        public object VisitPrintStmt(Stmt.Print stmt)
        {
            object value = Evaluate(stmt.ExpressionValue);
            Console.WriteLine(Stringify(value));
            return null;
        }
    }
}
