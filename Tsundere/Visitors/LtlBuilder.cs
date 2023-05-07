using System.Diagnostics;
using Tsundere.LTL;
using Tsundere.Parser;

namespace Tsundere.Visitors;

public class LtlBuilder : TsundereBaseVisitor<LtlNode>
{
    public override LtlNode VisitExpression(TsundereParser.ExpressionContext context)
    {
        LtlNode VisitChild(int i = 0) => context.expression(i).Accept(this);
        if (context.TRUE() != null) return LtlNode.TrueNode;
        if (context.FALSE() != null) return LtlNode.FalseNode;
        if (context.AP() != null) return Variable.Get(context.AP().GetText());
        if (context.LEFT_PAREN() != null) return VisitChild();
        if (context.NEG() != null) return VisitChild().Neg;
        if (context.EVENTUALLY() != null) return LtlNode.Eventually(VisitChild());
        if (context.ALWAYS() != null) return LtlNode.Always(VisitChild());
        if (context.NEXT() != null) return Next.Get(VisitChild());
        if (context.CONJ() != null) return Conjuncture.Get(VisitChild(), VisitChild(1));
        if (context.DISJ() != null) return LtlNode.Disjuncture(VisitChild(), VisitChild(1));
        if (context.IMPL() != null) return LtlNode.Imply(VisitChild(), VisitChild(1));
        if (context.UNTIL() != null) return Until.Get(VisitChild(), VisitChild(1));

        throw new UnreachableException();
    }
}