module TextParser.MarkdownParser
open ParserCombinators
open StandardParsers

type MarkdownValue =
    | Text of string
    | Bold of MarkdownValue list
    | Italic of MarkdownValue list
    | Heading of int * MarkdownValue list
    | Link of MarkdownValue list * string
    | Image of string * string
    | BlockQuote of string
    | List of string list
    
let (markdownValue, markdownValueRef) = forwardedParser<MarkdownValue>

let conditionParser =
    [ '*'; '_'; '#' ]
    |> List.map parseChar
    |> choice
    
// Creates a parser for escaped characters
let escapedCharacterParser =
    [
        ("\\*", '*')
        ("\\_", '_')
        ("\\#", '#')
        ("\\\\", '\\')
    ]
    |> List.map (fun (target, result) -> parseString target >>% result)
    |> choice
    
// Parsers a block of text
let text =
    conditionParser <!&>
    (escapedCharacterParser <|> parseAny)
    |> parseString1
    |>> Text
    
// Parses a block of bold text
let bold =
    let quote = parseString "**"
    let parser = quote <!&> markdownValue
    quote >>. parseMany1 parser .>> quote
    |>> Bold
    
// Parses a block of italic text
let italic =
    let quote = parseChar '*'
    // TODO - Fix parser to allow bold blocks
    let parser = quote <!&> markdownValue
    quote >>. parseMany1 parser .>> quote
    |>> Italic
    
markdownValueRef := choice
    [
        bold
        italic
        text
    ]
    
let parseMarkdown input = run (parseMany0 markdownValue) input