//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     ANTLR Version: 4.11.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from /Users/pioooooo/Programming/tsundere/Tsundere/Parser/Tsundere.g4 by ANTLR 4.11.1

// Unreachable code detected
#pragma warning disable 0162
// The variable '...' is assigned but its value is never used
#pragma warning disable 0219
// Missing XML comment for publicly visible type or member '...'
#pragma warning disable 1591
// Ambiguous reference in cref attribute
#pragma warning disable 419

namespace Tsundere.Parser {
using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using Antlr4.Runtime;
using Antlr4.Runtime.Atn;
using Antlr4.Runtime.Misc;
using Antlr4.Runtime.Tree;
using DFA = Antlr4.Runtime.Dfa.DFA;

[System.CodeDom.Compiler.GeneratedCode("ANTLR", "4.11.1")]
[System.CLSCompliant(false)]
public partial class TsundereParser : Parser {
	protected static DFA[] decisionToDFA;
	protected static PredictionContextCache sharedContextCache = new PredictionContextCache();
	public const int
		TRUE=1, FALSE=2, AP=3, NEG=4, CONJ=5, DISJ=6, IMPL=7, NEXT=8, ALWAYS=9, 
		EVENTUALLY=10, UNTIL=11, LEFT_PAREN=12, RIGHT_PAREN=13, WHITESPACE=14;
	public const int
		RULE_language = 0, RULE_expression = 1;
	public static readonly string[] ruleNames = {
		"language", "expression"
	};

	private static readonly string[] _LiteralNames = {
		null, "'true'", "'false'", null, "'!'", "'/\\'", "'\\/'", "'->'", "'X'", 
		"'G'", "'F'", "'U'", "'('", "')'"
	};
	private static readonly string[] _SymbolicNames = {
		null, "TRUE", "FALSE", "AP", "NEG", "CONJ", "DISJ", "IMPL", "NEXT", "ALWAYS", 
		"EVENTUALLY", "UNTIL", "LEFT_PAREN", "RIGHT_PAREN", "WHITESPACE"
	};
	public static readonly IVocabulary DefaultVocabulary = new Vocabulary(_LiteralNames, _SymbolicNames);

	[NotNull]
	public override IVocabulary Vocabulary
	{
		get
		{
			return DefaultVocabulary;
		}
	}

	public override string GrammarFileName { get { return "Tsundere.g4"; } }

	public override string[] RuleNames { get { return ruleNames; } }

	public override int[] SerializedAtn { get { return _serializedATN; } }

	static TsundereParser() {
		decisionToDFA = new DFA[_ATN.NumberOfDecisions];
		for (int i = 0; i < _ATN.NumberOfDecisions; i++) {
			decisionToDFA[i] = new DFA(_ATN.GetDecisionState(i), i);
		}
	}

		public TsundereParser(ITokenStream input) : this(input, Console.Out, Console.Error) { }

		public TsundereParser(ITokenStream input, TextWriter output, TextWriter errorOutput)
		: base(input, output, errorOutput)
	{
		Interpreter = new ParserATNSimulator(this, _ATN, decisionToDFA, sharedContextCache);
	}

	public partial class LanguageContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ExpressionContext expression() {
			return GetRuleContext<ExpressionContext>(0);
		}
		public LanguageContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_language; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ITsundereVisitor<TResult> typedVisitor = visitor as ITsundereVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitLanguage(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public LanguageContext language() {
		LanguageContext _localctx = new LanguageContext(Context, State);
		EnterRule(_localctx, 0, RULE_language);
		int _la;
		try {
			EnterOuterAlt(_localctx, 1);
			{
			State = 5;
			ErrorHandler.Sync(this);
			_la = TokenStream.LA(1);
			if (((_la) & ~0x3f) == 0 && ((1L << _la) & 5918L) != 0) {
				{
				State = 4;
				expression(0);
				}
			}

			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			ExitRule();
		}
		return _localctx;
	}

	public partial class ExpressionContext : ParserRuleContext {
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode TRUE() { return GetToken(TsundereParser.TRUE, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode FALSE() { return GetToken(TsundereParser.FALSE, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode AP() { return GetToken(TsundereParser.AP, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode LEFT_PAREN() { return GetToken(TsundereParser.LEFT_PAREN, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ExpressionContext[] expression() {
			return GetRuleContexts<ExpressionContext>();
		}
		[System.Diagnostics.DebuggerNonUserCode] public ExpressionContext expression(int i) {
			return GetRuleContext<ExpressionContext>(i);
		}
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode RIGHT_PAREN() { return GetToken(TsundereParser.RIGHT_PAREN, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NEG() { return GetToken(TsundereParser.NEG, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode ALWAYS() { return GetToken(TsundereParser.ALWAYS, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode EVENTUALLY() { return GetToken(TsundereParser.EVENTUALLY, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode NEXT() { return GetToken(TsundereParser.NEXT, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode CONJ() { return GetToken(TsundereParser.CONJ, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode DISJ() { return GetToken(TsundereParser.DISJ, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode IMPL() { return GetToken(TsundereParser.IMPL, 0); }
		[System.Diagnostics.DebuggerNonUserCode] public ITerminalNode UNTIL() { return GetToken(TsundereParser.UNTIL, 0); }
		public ExpressionContext(ParserRuleContext parent, int invokingState)
			: base(parent, invokingState)
		{
		}
		public override int RuleIndex { get { return RULE_expression; } }
		[System.Diagnostics.DebuggerNonUserCode]
		public override TResult Accept<TResult>(IParseTreeVisitor<TResult> visitor) {
			ITsundereVisitor<TResult> typedVisitor = visitor as ITsundereVisitor<TResult>;
			if (typedVisitor != null) return typedVisitor.VisitExpression(this);
			else return visitor.VisitChildren(this);
		}
	}

	[RuleVersion(0)]
	public ExpressionContext expression() {
		return expression(0);
	}

	private ExpressionContext expression(int _p) {
		ParserRuleContext _parentctx = Context;
		int _parentState = State;
		ExpressionContext _localctx = new ExpressionContext(Context, _parentState);
		ExpressionContext _prevctx = _localctx;
		int _startState = 2;
		EnterRecursionRule(_localctx, 2, RULE_expression, _p);
		try {
			int _alt;
			EnterOuterAlt(_localctx, 1);
			{
			State = 23;
			ErrorHandler.Sync(this);
			switch (TokenStream.LA(1)) {
			case TRUE:
				{
				State = 8;
				Match(TRUE);
				}
				break;
			case FALSE:
				{
				State = 9;
				Match(FALSE);
				}
				break;
			case AP:
				{
				State = 10;
				Match(AP);
				}
				break;
			case LEFT_PAREN:
				{
				State = 11;
				Match(LEFT_PAREN);
				State = 12;
				expression(0);
				State = 13;
				Match(RIGHT_PAREN);
				}
				break;
			case NEG:
				{
				State = 15;
				Match(NEG);
				State = 16;
				expression(8);
				}
				break;
			case ALWAYS:
				{
				State = 17;
				Match(ALWAYS);
				State = 18;
				expression(7);
				}
				break;
			case EVENTUALLY:
				{
				State = 19;
				Match(EVENTUALLY);
				State = 20;
				expression(6);
				}
				break;
			case NEXT:
				{
				State = 21;
				Match(NEXT);
				State = 22;
				expression(5);
				}
				break;
			default:
				throw new NoViableAltException(this);
			}
			Context.Stop = TokenStream.LT(-1);
			State = 39;
			ErrorHandler.Sync(this);
			_alt = Interpreter.AdaptivePredict(TokenStream,3,Context);
			while ( _alt!=2 && _alt!=global::Antlr4.Runtime.Atn.ATN.INVALID_ALT_NUMBER ) {
				if ( _alt==1 ) {
					if ( ParseListeners!=null )
						TriggerExitRuleEvent();
					_prevctx = _localctx;
					{
					State = 37;
					ErrorHandler.Sync(this);
					switch ( Interpreter.AdaptivePredict(TokenStream,2,Context) ) {
					case 1:
						{
						_localctx = new ExpressionContext(_parentctx, _parentState);
						PushNewRecursionContext(_localctx, _startState, RULE_expression);
						State = 25;
						if (!(Precpred(Context, 4))) throw new FailedPredicateException(this, "Precpred(Context, 4)");
						State = 26;
						Match(CONJ);
						State = 27;
						expression(5);
						}
						break;
					case 2:
						{
						_localctx = new ExpressionContext(_parentctx, _parentState);
						PushNewRecursionContext(_localctx, _startState, RULE_expression);
						State = 28;
						if (!(Precpred(Context, 3))) throw new FailedPredicateException(this, "Precpred(Context, 3)");
						State = 29;
						Match(DISJ);
						State = 30;
						expression(4);
						}
						break;
					case 3:
						{
						_localctx = new ExpressionContext(_parentctx, _parentState);
						PushNewRecursionContext(_localctx, _startState, RULE_expression);
						State = 31;
						if (!(Precpred(Context, 2))) throw new FailedPredicateException(this, "Precpred(Context, 2)");
						State = 32;
						Match(IMPL);
						State = 33;
						expression(3);
						}
						break;
					case 4:
						{
						_localctx = new ExpressionContext(_parentctx, _parentState);
						PushNewRecursionContext(_localctx, _startState, RULE_expression);
						State = 34;
						if (!(Precpred(Context, 1))) throw new FailedPredicateException(this, "Precpred(Context, 1)");
						State = 35;
						Match(UNTIL);
						State = 36;
						expression(2);
						}
						break;
					}
					} 
				}
				State = 41;
				ErrorHandler.Sync(this);
				_alt = Interpreter.AdaptivePredict(TokenStream,3,Context);
			}
			}
		}
		catch (RecognitionException re) {
			_localctx.exception = re;
			ErrorHandler.ReportError(this, re);
			ErrorHandler.Recover(this, re);
		}
		finally {
			UnrollRecursionContexts(_parentctx);
		}
		return _localctx;
	}

	public override bool Sempred(RuleContext _localctx, int ruleIndex, int predIndex) {
		switch (ruleIndex) {
		case 1: return expression_sempred((ExpressionContext)_localctx, predIndex);
		}
		return true;
	}
	private bool expression_sempred(ExpressionContext _localctx, int predIndex) {
		switch (predIndex) {
		case 0: return Precpred(Context, 4);
		case 1: return Precpred(Context, 3);
		case 2: return Precpred(Context, 2);
		case 3: return Precpred(Context, 1);
		}
		return true;
	}

	private static int[] _serializedATN = {
		4,1,14,43,2,0,7,0,2,1,7,1,1,0,3,0,6,8,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
		1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,3,1,24,8,1,1,1,1,1,1,1,1,1,1,1,1,1,1,
		1,1,1,1,1,1,1,1,1,1,1,5,1,38,8,1,10,1,12,1,41,9,1,1,1,0,1,2,2,0,2,0,0,
		52,0,5,1,0,0,0,2,23,1,0,0,0,4,6,3,2,1,0,5,4,1,0,0,0,5,6,1,0,0,0,6,1,1,
		0,0,0,7,8,6,1,-1,0,8,24,5,1,0,0,9,24,5,2,0,0,10,24,5,3,0,0,11,12,5,12,
		0,0,12,13,3,2,1,0,13,14,5,13,0,0,14,24,1,0,0,0,15,16,5,4,0,0,16,24,3,2,
		1,8,17,18,5,9,0,0,18,24,3,2,1,7,19,20,5,10,0,0,20,24,3,2,1,6,21,22,5,8,
		0,0,22,24,3,2,1,5,23,7,1,0,0,0,23,9,1,0,0,0,23,10,1,0,0,0,23,11,1,0,0,
		0,23,15,1,0,0,0,23,17,1,0,0,0,23,19,1,0,0,0,23,21,1,0,0,0,24,39,1,0,0,
		0,25,26,10,4,0,0,26,27,5,5,0,0,27,38,3,2,1,5,28,29,10,3,0,0,29,30,5,6,
		0,0,30,38,3,2,1,4,31,32,10,2,0,0,32,33,5,7,0,0,33,38,3,2,1,3,34,35,10,
		1,0,0,35,36,5,11,0,0,36,38,3,2,1,2,37,25,1,0,0,0,37,28,1,0,0,0,37,31,1,
		0,0,0,37,34,1,0,0,0,38,41,1,0,0,0,39,37,1,0,0,0,39,40,1,0,0,0,40,3,1,0,
		0,0,41,39,1,0,0,0,4,5,23,37,39
	};

	public static readonly ATN _ATN =
		new ATNDeserializer().Deserialize(_serializedATN);


}
} // namespace Tsundere.Parser
