using System.Diagnostics;
using Tsundere.Parser;

namespace Tsundere.Visitors;

public class Printer : TsundereBaseVisitor<int>
{
    public override int VisitExpression(TsundereParser.ExpressionContext context)
    {
        if (context.TRUE() != null)
        {
            Write("TRUE");
            return 0;
        }

        if (context.FALSE() != null)
        {
            Write("FALSE");
            return 0;
        }

        if (context.AP() != null)
        {
            Write(context.AP().GetText());
            return 0;
        }

        if (context.LEFT_PAREN() != null)
        {
            Write("(");
            var res = context.expression(0).Accept(this);
            Write(")");
            return res;
        }

        if (context.NEG() != null)
        {
            Write("!");
            return context.expression(0).Accept(this);
        }

        if (context.EVENTUALLY() != null)
        {
            Write("F");
            return context.expression(0).Accept(this);
        }

        if (context.ALWAYS() != null)
        {
            Write("G");
            return context.expression(0).Accept(this);
        }

        if (context.NEXT() != null)
        {
            Write("X");
            return context.expression(0).Accept(this);
        }

        if (context.CONJ() != null)
        {
            var res = context.expression(0).Accept(this);
            Write("/\\");
            res += context.expression(1).Accept(this);

            return res;
        }

        if (context.DISJ() != null)
        {
            var res = context.expression(0).Accept(this);
            Write("\\/");
            res += context.expression(1).Accept(this);

            return res;
        }

        if (context.IMPL() != null)
        {
            var res = context.expression(0).Accept(this);
            Write("->");
            res |= context.expression(1).Accept(this);

            return res;
        }

        if (context.UNTIL() != null)
        {
            var res = context.expression(0).Accept(this);
            Write("U");
            res |= context.expression(1).Accept(this);

            return res;
        }

        throw new UnreachableException();
    }

    private static void Write(string ln) => Console.Write(ln);
}