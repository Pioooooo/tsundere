using Antlr4.Runtime;
using Tsundere.Parser;
using Tsundere.TS;
using Tsundere.Visitors;

namespace Tsundere.LTL;

public class LtlNode : Named
{
}

public class Ltl
{
    private LtlNode root;

    public Ltl(String input)
    {
        var antlrInputStream = new AntlrInputStream(input);
        var tsundereLexer = new TsundereLexer(antlrInputStream);
        var commonTokenStream = new CommonTokenStream(tsundereLexer);
        var tsundereParser = new TsundereParser(commonTokenStream);
        var context = tsundereParser.language();
        root = new LtlBuilder().VisitLanguage(context);
    }
}