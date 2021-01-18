module TextParser.ParserCombinators
open System

type Result<'a> =
    | Success of 'a
    | Failure of string

type Parser<'T> = Parser of (string ->  Result<'T * string>)

// Runs a parser with the given input
let run parser input =
    let (Parser innerFn) = parser
    innerFn input
    
// Parses a character that meets the given condition
let parse condition =
    let innerFn input =
        if String.IsNullOrEmpty(input) then
            Failure "End of input"
        else
            if condition input.[0] then
                Success (input.[0], input.[1..])
            else
                Failure (sprintf "Unexpected '%c'" input.[0])
    Parser innerFn

// Parses the given character
let parseChar target =
    let condition character = (character = target)
    parse condition
    
// Passes the output of a parser to a parser producing function, creating a new parser
let bindParser fn parser =
    let innerFn input =
        let result1 = run parser input
        match result1 with
        | Success (result, remaining) ->
            let parser2 = fn result
            run parser2 remaining
        | Failure err -> Failure err
    Parser innerFn
    
let ( >>= ) parser fn = bindParser fn parser

// Creates a parser of the given value
let returnParser value =
    let innerFn input =
        Success (value, input)
    Parser innerFn
    
// Applies a function to the value inside a parser
let mapParser fn =
    bindParser (fn >> returnParser)
    
let ( <!> ) = mapParser

let ( |>> ) value fn = mapParser fn value

// Lifts a 2 parameter function to the parser type
let lift2 fn parser1 parser2 =
    returnParser fn >>= (fun fn ->
    parser1 >>= (fun p1 ->
    parser2 >>= (fun p2 ->
        returnParser (fn p1 p2))))
    
// Combines two parsers
let andThen parser1 parser2 =
    parser1 >>= (fun p1 ->
    parser2 >>= (fun p2 ->
        returnParser (p1, p2)))
    
let ( .>>. ) = andThen
    
// Uses a choice of two parsers
let orElse parser1 parser2 =
    let innerFn input =
        let result1 = run parser1 input
        match result1 with
        | Success _ -> result1
        | Failure _ ->
            run parser2 input
    Parser innerFn
    
let ( <|> ) = orElse

// Parser choice from a list of parsers
let choice parserList =
    parserList |> List.reduce orElse

// Creates a parser choice from a list of characters    
let anyOf charList =
    charList
    |> List.map parseChar
    |> choice
    
// Converts a list of parser into a parser of a list
let rec sequence parserList =
    let cons head tail = head::tail
    let consParser = lift2 cons
    match parserList with
    | [] -> returnParser []
    | head::tail -> consParser head (sequence tail)
    
// Parses zero or more of a given parser
let rec parseZeroOrMore parser input =
    let result1 = run parser input
    match result1 with
    | Failure _ -> ([], input)
    | Success (values1, remaining1) ->
        let (values2, remaining2) = parseZeroOrMore parser remaining1
        (values1::values2, remaining2)

let parseMany0 parser =
    let rec innerFn input =
        Success (parseZeroOrMore parser input)
    Parser innerFn

// Parses one or more of a given parser
let parseMany1 parser =
    parser >>= (fun head ->
    parseMany0 parser >>= (fun tail ->
        returnParser (head::tail)))
    
// Parses an optional occurence of a given parser
let optional parser =
    let some = parser |>> Some
    let none = returnParser None
    some <|> none
    
// Applies two parsers, discarding the result of the second 
let ( .>> ) parser1 parser2 =
    parser1 .>>. parser2
    |> mapParser (fun (a,_) -> a)
    
// Applies two parsers, discarding the result of the first
let ( >>. ) parser1 parser2 =
    parser1 .>>. parser2
    |> mapParser (fun (_,b) -> b)
    
// Applies three parsers, discarding the result of the first and third
let between parser1 parser2 parser3 =
    parser1 >>. parser2 .>> parser3
    
// parses a given string
let parseString str =
    str
    |> List.ofSeq
    |> List.map parseChar
    |> sequence
    |> mapParser (fun chars -> String(List.toArray chars))