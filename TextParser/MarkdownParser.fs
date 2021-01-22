module TextParser.MarkdownParser
open System.Text
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
    [ '*'; '_' ]
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
    
let notNewLine =
    let condition ch = (ch <> '\n' && ch <> '\r')
    parse condition

// Parsers a character, escaping it if needed
let characterParser = escapedCharacterParser <|> notNewLine
    
// Parsers a block of text
let text =
    characterParser .>>. parseString0 (conditionParser <!&> characterParser)
    |>> (fun (a,b) -> sprintf "%c%s" a b)
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
    let conditionParser = quote <&> notParser (parseString "**")
    let parser = conditionParser <!&> markdownValue
    quote >>. parseMany1 parser .>> quote
    |>> Italic
    
let heading =
    let start = parseString1 (parseChar '#') .>> parseString1 (parseChar ' ')
    let content = parseMany1 markdownValue
    start .>>. content
    |>> fun (start, content) -> (start.Length, content)
    |>> Heading
    
// Sets the value of markdownValue
markdownValueRef := choice
    [
        bold
        italic
        text
    ]
    
// Parses a line of markdown
let markdownLineContent = ((heading |>> fun x -> [x]) <|> parseMany1 markdownValue)
let endOfLineChar = parseString "\r\n" <|> parseInputEnd

let markdownLine =
    (optional markdownLineContent .>>. endOfLineChar)
    |>> fun (content, endChar) ->
       match content with
       | Some _content -> _content @ [Text endChar]
       | None -> [Text endChar]

// Parsers many markdown lines, combining the lists at the end
let parseMarkdownLines =
    parseMany0 markdownLine
    |>> List.fold (fun acc elem ->
        match elem with
        | [] -> acc @ [ Text "\r\n" ]
        | _ -> acc @ elem
    ) []
    
// Parsers the given input
let parseMarkdown input = run parseMarkdownLines input

// Converts a MarkdownValue list to html string 
let rec markdownToHTML (valueList : MarkdownValue list) : string =
    // Parses each individual item to markdown, then appends to StringBuilder, which is converted to string at the end 
    valueList
    |> List.fold (fun (acc : StringBuilder) item ->
        let value =
            match item with
            | Bold bold -> sprintf "<strong>%s</strong>" (markdownToHTML bold)
            | Italic italic -> sprintf "<em>%s</em>" (markdownToHTML italic)
            | Heading (degree, content) -> sprintf "<h%i>%s</h%i>" degree (markdownToHTML content) degree
            | Text text -> text.Replace("\r\n", "<br>\r\n")
            | _ -> "<p>Value not supported</p>"
        acc.Append(value)
    ) (StringBuilder())
    |> fun x -> x.ToString()