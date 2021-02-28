module SimpleForum.TextParser.XamarinParser
open System
open Xamarin.Essentials
open Xamarin.Forms
open MarkdownParser

// Represents the attributes of a span
type TextAttributes = {
    IsBold : bool
    IsItalic : bool
    IsLink : bool
    IsQuote : bool
    IsHeading : bool
    IsImage : bool
    HeadingValue : int
    URL : string
    ImageURL : string
}

// Converts the attributes to a string
let returnAttributes (textAttributes:TextAttributes) =
    match (textAttributes.IsBold, textAttributes.IsItalic) with
    | (true, true) -> FontAttributes.Bold ||| FontAttributes.Italic
    | (true, false) -> FontAttributes.Bold
    | (false, true) -> FontAttributes.Italic
    | _ -> FontAttributes.None
    
let linkCommand = Command<string>(fun url -> async { do! Launcher.OpenAsync(url) |> Async.AwaitTask } |> Async.StartImmediate ) 
    
let renderBlock (text: string) (formattedString: FormattedString) (textAttributes: TextAttributes) =
    let span = Span()
    span.FontAttributes <- returnAttributes textAttributes
    span.Text <- text
    if textAttributes.IsLink then
        let tapGestureRecognizer = TapGestureRecognizer()
        tapGestureRecognizer.Command <- linkCommand
        tapGestureRecognizer.CommandParameter <- textAttributes.URL
        span.GestureRecognizers.Add(tapGestureRecognizer)
        span.TextColor <- Color.Blue
    formattedString.Spans.Add(span)
    
let rec renderFormattedString markdownValueList (formattedString: FormattedString) textAttributes =
    markdownValueList
    |> List.iter (fun i ->
        match i with
        | Bold bold ->
            let newAttributes = { textAttributes with IsBold = true }
            renderFormattedString bold formattedString newAttributes
        | Italic italic ->
            let newAttributes = { textAttributes with IsItalic = true }
            renderFormattedString italic formattedString newAttributes
        | Heading (degree, content) ->
            let newAttributes = { textAttributes with IsHeading = true; HeadingValue = degree }
            renderFormattedString content formattedString newAttributes
        | Image (alt, url) ->
            let newAttributes = { textAttributes with IsImage = true; ImageURL = url }
            renderBlock alt formattedString newAttributes
        | Link (title, url) ->
            let newAttributes = { textAttributes with IsLink = true; URL = url }
            renderFormattedString title formattedString newAttributes
        | BlockQuote blockQuote ->
            let newAttributes = { textAttributes with IsQuote = true }
            renderFormattedString blockQuote formattedString newAttributes
        | Text text -> renderBlock text formattedString textAttributes
    )
    
let renderObject markdownValueList =
    let formattedString = FormattedString()
    let textAttributes = {
        IsBold = false; IsItalic = false; IsLink = false; IsQuote = false; IsHeading = false; IsImage = false
        URL = String.Empty; ImageURL = String.Empty; HeadingValue = 0
    }
    renderFormattedString markdownValueList formattedString textAttributes
    formattedString
    
let RenderFormattedString markdownValueList =
    markdownValueList
    |> List.ofSeq
    |> renderObject