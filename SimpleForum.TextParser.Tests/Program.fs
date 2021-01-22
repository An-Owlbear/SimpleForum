open System.Diagnostics
open System.IO
open System.Text
open FSharp.Formatting.Markdown
open SimpleForum.TextParser.ParserCombinators
open SimpleForum.TextParser.MarkdownParser

let generateFile content =
    [
        "<!DOCTYPE html>"
        "<head>"
        "\t<title>Markdown test</title>"
        "</head>"
        "<body>"
        sprintf "\t%s" content
        "</body>"
        "</html>"
    ] |> List.fold (fun (sb : StringBuilder) -> sb.Append) (StringBuilder())
    |> fun x -> x.ToString()


[<EntryPoint>]
let main argv =
    let timer = Stopwatch()

    let text = File.ReadAllText("Sample.md")
    
    timer.Start()
    let result = parseMarkdown text
    
    match result with
    | Success (value, _) -> File.WriteAllText("Sample.html", generateFile (markdownToHTML value))
    | Failure _ -> printfn "Failure"
    timer.Stop()
    printfn "%f" timer.Elapsed.TotalMilliseconds
    
    timer.Restart()
    let parsed = Markdown.Parse(text)
    let html = Markdown.ToHtml(parsed)
    File.WriteAllText("OtherSample.html", html)
    timer.Stop()
    printfn "%f" timer.Elapsed.TotalMilliseconds    
    0