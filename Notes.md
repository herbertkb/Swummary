# Notes on Swum.NET

Swum’s rule system maps a ProgramElementNode (abstract) to a PhraseNode (abstract) based on the logic of the Rule and the context from the ProgramElementNode..

Swum.NET already has ProgramElementNodes for:
- Method declarations: `MethodDeclarationNode`
    - `toString(): “{Action} | {Theme} {Arguments}”`
        - action is the verb of the method
        - theme is the “target” (noun phrase) of the action
        - arguments are modifiers of the action or noun phrase (adverbs or adjectives?)

- Fields within a class: `FieldDeclarationNode
`
    - Field is an attribute of the class (variable of the class)
- Arguments to a method: `ArgumentNode` 
- Types: `TypeNode`
- Variable names: `VariableDeclarationNode`
- Genres of methods via accessor methods of `MethodDeclarationNode`
    - reactive methods (event handler, callback, etc): `isReactive()`
    - constructor: `isConstructor()`
    - destructor: `isDestructor()`

Maybe use a template system to map s_units into sentences

    original: cheeseburger = new Hamburger(cheese);
    SWUM:     cheeseburger(noun), hamburger(noun) cheese(modifier)
    Mapped:   A cheeseburger is a hamburger with cheese. 

Challenge: when to apply which rule?
    - Apply all the rules to all the relevant ProgramElementNode
    - There should be a confidence score on each mapping
    - Collect the mappings with highest confidence scores
    - I have a bad feeling the Viterbi algorithm will be involved somehow
