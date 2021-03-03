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
    
// Creates span from the given text
let renderBlock (text: string) (formattedString: FormattedString) (textAttributes: TextAttributes) =
    // Adds text and attributes
    let span = Span()
    span.FontAttributes <- returnAttributes textAttributes
    span.Text <- text
    
    // Adds link action if required
    if textAttributes.IsLink then
        let tapGestureRecognizer = TapGestureRecognizer()
        tapGestureRecognizer.Command <- linkCommand
        tapGestureRecognizer.CommandParameter <- textAttributes.URL
        span.GestureRecognizers.Add(tapGestureRecognizer)
        span.TextColor <- Color.Blue
        
    // Adds heading sizing
    if textAttributes.IsHeading then
        span.FontSize <-
            match textAttributes.HeadingValue with
            | 1 -> 28.0
            | 2 -> 21.0
            | 3 -> 18.2
            | 4 -> 14.0
            | 5 -> 11.2
            | 6 -> 9.8
            | _ -> 14.0
            
    formattedString.Spans.Add(span)
    
// Recursively parses MarkdownValues, setting the correct FontAttributes
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
    
// Renders MarkdownValueList
let renderObject markdownValueList =
    let formattedString = FormattedString()
    let textAttributes = {
        IsBold = false; IsItalic = false; IsLink = false; IsQuote = false; IsHeading = false; IsImage = false
        URL = String.Empty; ImageURL = String.Empty; HeadingValue = 0
    }
    renderFormattedString markdownValueList formattedString textAttributes
    formattedString
    
// C# compatibility method
let RenderFormattedString markdownValueList =
    markdownValueList
    |> List.ofSeq
    |> renderObject