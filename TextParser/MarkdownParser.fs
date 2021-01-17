module TextParser.MarkdownParser
open ParserCombinators
open StandardParsers

type MarkdownValue =
    | Bold of string
    | Italic of string
    | Heading of int * string
    | Link of string * string
    | Image of string * string
    | BlockQuote of string
    | List of string list
    
let bold =
    let asterisk = parseChar '*'
    let quote = asterisk .>>. asterisk
    let condition ch = (ch <> '*')
    let charParser = parse condition
    quote >>. parseString0 charParser .>> quote
    |>> Bold