using System.Diagnostics;
using Tsundere.LTL;
using Tsundere.Parser;
using Tsundere.TS;

namespace Tsundere.Visitors;

public class LtlBuilder : TsundereBaseVisitor<LtlNode>
{
    private readonly Dictionary<string, AtomicProposition> _propsByString;
    private readonly Dictionary<string, Variable> _varsByString = new();

    public LtlBuilder(Dictionary<string, AtomicProposition> propsByString) => _propsByString = propsByString;

    private Variable GetVar(string name) =>
        _varsByString.TryGetValue(name, out var value)
            ? value
            : _varsByString[name] = new Variable(_propsByString[name]);

    public override LtlNode VisitExpression(TsundereParser.ExpressionContext context)
    {
        LtlNode VisitChild(int i = 0) => context.expression(i).Accept(this);
        if (context.TRUE() != null) return LtlNode.TrueNode;
        if (context.FALSE() != null) return LtlNode.FalseNode;
        if (context.AP() != null) return GetVar(context.AP().GetText());
        if (context.LEFT_PAREN() != null) return VisitChild();
        if (context.NEG() != null) return VisitChild().Neg();
        if (context.EVENTUALLY() != null) return LtlNode.Eventually(VisitChild());
        if (context.ALWAYS() != null) return LtlNode.Always(VisitChild());
        if (context.NEXT() != null) return new Next(VisitChild());
        if (context.CONJ() != null) return new Conjuncture(VisitChild(), VisitChild(1));
        if (context.DISJ() != null) return LtlNode.Disjuncture(VisitChild(), VisitChild(1));
        if (context.IMPL() != null) return LtlNode.Imply(VisitChild(), VisitChild(1));
        if (context.UNTIL() != null) return new Until(VisitChild(), VisitChild(1));

        throw new UnreachableException();
    }
}