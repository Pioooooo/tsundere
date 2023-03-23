grammar Tsundere;

language : expression? ;

expression
    : TRUE
    | FALSE
    | AP
    | LEFT_PAREN expression RIGHT_PAREN
    | NEG expression
    | ALWAYS expression
    | EVENTUALLY expression
    | NEXT expression
    | expression CONJ expression
    | expression DISJ expression
    | expression IMPL expression
    | expression UNTIL expression; 


fragment LOWERCASE  : [a-z] ;
fragment DIGIT      : [0-9] ;

TRUE                : 'true' ;
FALSE               : 'false' ;
AP                  : (LOWERCASE | DIGIT)+ ;

NEG                 : '!' ;
CONJ                : '/\\' ;
DISJ                : '\\/' ;
IMPL                : '->' ;
NEXT                : 'X' ;
ALWAYS              : 'G' ;
EVENTUALLY          : 'F' ;
UNTIL               : 'U' ;

LEFT_PAREN          : '(';
RIGHT_PAREN         : ')';
WHITESPACE          : [ \n\t\r]+ -> skip ;
