# Dopple
This is a personal project, that attempts to automatically detect .net code similarities and duplicates using compiled (CIL) code.

To accomplish this, the program describes an input function with a graph, depicting both the flow of data through the different opcodes
(e.g, Ldarg.0 -> Neg -> Brtrue), and the flow of execution in terms of which jump codes affect which other opcode (Bgt -> Add).
My theory is, that 2 functions that "do similar things" will have similar describing graphs.

I'm currently still developing this project and testing it, but the first set of results (comparing array sorting) look promising.

I plan to add a website for people to try it out and give feedback.
Also I'd like to integrate machine learning to the graph comparing process using a bag-of-words technique,
where the words will be edges from a type of node to another type.
