module TextParser.StandardParsers
open System
open ParserCombinators

let internal letters = [for i in 65..90 -> char i] @ [for i in 97..122 -> char i]

let internal charListToString charList =
    String(List.toArray charList)

// Parses a whitespace character
let parseWhitespaceChar =
    let condition ch = Char.IsWhiteSpace(ch)
    parse condition
    
// Parses zero or more whitespace characters
let spaces0 = parseMany0 parseWhitespaceChar

// Parses one or more whitespace characters
let spaces1 = parseMany1 parseWhitespaceChar

// Functions for parsing letters
let parseLetter = anyOf letters
let parseLetters0 = parseMany0 parseLetter |>> charListToString
let parseLetters1 = parseMany1 parseLetter |>> charListToString
    
// Functions for parsing letters/numbers
let parseLetterOrNumber =
    letters @ [for i in 48..57 -> char i]
    |> anyOf
    
let parseLettersOrNumbers0 = parseMany0 parseLetterOrNumber |>> charListToString
let parseLettersOrNumbers1 = parseMany1 parseLetterOrNumber |>> charListToString