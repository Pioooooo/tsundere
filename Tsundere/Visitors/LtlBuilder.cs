using Tsundere.LTL;
using Tsundere.Parser;

namespace Tsundere.Visitors;

public class LtlBuilder : TsundereBaseVisitor<LtlNode>
{
    public override LtlNode VisitLanguage(TsundereParser.LanguageContext context)
    {
        return VisitChildren(context);
    }

    public override LtlNode VisitExpression(TsundereParser.ExpressionContext context)
    {
        return VisitChildren(context);
    }
}