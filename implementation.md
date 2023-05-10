# Implementation

## Parser

### Parsing Transition System

It is trivial to read the transition system from input. ([`Tsundere.TS.TransitionSystem.Parse`](Tsundere/TS/TS.cs))

### Parsing LTL Formulas

Uses [Antlr4](https://github.com/antlr/antlr4) to generate LTL recognizer. The grammar file is located
at [Tsundere.g4](Tsundere/Parser/Tsundere.g4). A LTL formula is then constructed from the syntax tree. ([`Tsundere.Visitors.LtlBuilder.VisitExpression`](Tsundere/Visitors/LtlBuilder.cs))

Rules to elimate $\textbf{false},\neg\neg,\vee,\rightarrow,\square,\lozenge$:

- $\textbf{false}\equiv\neg\textbf{true}$
- $\neg\neg\varphi\equiv\varphi$
- $\varphi\vee\psi\equiv\neg(\neg\varphi\wedge\neg\psi)$
- $\varphi\rightarrow\psi\equiv\neg(\varphi\wedge\neg\psi)$
- $\square\varphi\equiv\neg(\textbf{true}\mathsf{U}\neg\varphi)$
- $\lozenge\varphi\equiv\textbf{true}\mathsf{U}\varphi$

## GNBA for LTL formula

The GNBA is constructed following the proof of Theorem 5.37 (p.278, *Principles of Model Checking*). ([`Tsundere.BA.Gnba.FromLtl`](Tsundere/BA/GNBA.cs))

Elementary cover generation for LTL formula $\varphi$ is done by:
1. Generate the powerset $2^{closure(\varphi)\cap AP}$.
2. Gradually decide by length whether $\psi\in closure\setminus AP$ belongs to the generated set and add all possiblities to construct a new set.
3. Repeat step 2 until all subformulas are considered.

The remaining part is straight forward.

## From GNBA to NBA

The NBA is constructed following the proof of Theorem 4.56 (p.195, *Principles of Model Checking*). ([`Tsundere.BA.Nba.FromGnba`](Tsundere/BA/GNBA.cs))

## Product of Transition System and NBA

The product of transition system and NBA is constructed following Definition 4.62 (p.200, *Principles of Model Checking*). ([`Tsundere.TS.TransitionSystem.Product`](Tsundere/TS/TS.cs))

## Persistence Checking using Cycle Detection

The persistence checking is done following the nested depth-first search algorithm (p.211, *Principles of Model Checking*). ([`Tsundere.TS.TransitionSystem.Persistent`](Tsundere/TS/TS.cs))
