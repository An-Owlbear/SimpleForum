// Learn more about F# at http://fsharp.org

open TextParser.MarkdownParser

[<EntryPoint>]
let main argv =
    let result = parseMarkdown "* adas  **asdasdasd**  dasd *"
    printfn "%A" result
    0 // return an integer exit code