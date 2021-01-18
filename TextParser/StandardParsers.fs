module TextParser.StandardParsers
open System
open ParserCombinators

let internal letters = [for i in 65..90 -> char i] @ [for i in 97..122 -> char i]

let internal charListToString charList =
    String(List.toArray charList)
    
// String parsing functions
let parseString0 parser = parseMany0 parser |>> charListToString
let parseString1 parser = parseMany1 parser |>> charListToString

// Parses a whitespace character
let parseWhitespaceChar =
    let condition ch = Char.IsWhiteSpace(ch)
    parse condition
    
// Parses zero or more whitespace characters
let spaces0 = parseString0 parseWhitespaceChar

// Parses one or more whitespace characters
let spaces1 = parseString1 parseWhitespaceChar

// Functions for parsing letters
let parseLetter = anyOf letters
let parseLetters0 = parseString0 parseLetter
let parseLetters1 = parseString1 parseLetter
    
// Functions for parsing letters/numbers
let parseLetterOrNumber =
    letters @ [for i in 48..57 -> char i]
    |> anyOf
    
let parseLettersOrNumbers0 = parseString0 parseLetterOrNumber
let parseLettersOrNumbers1 = parseString1 parseLetterOrNumber

// Creates a forwarded parser
let forwardedParser<'a> =
    let dummyParser =
        let innerFn input : Result<'a * string> = failwith "Unfixed forwarded parser"
        Parser innerFn
    
    let parserReference = ref dummyParser
    
    let innerFn input =
        run !parserReference input
    
    (Parser innerFn, parserReference)
    
// Parses any character
let parseAny = parse (fun _ -> true)