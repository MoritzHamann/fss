module Document

open System
open Markdig
open YamlDotNet.Core
open YamlDotNet.Core.Events


// Using a class here, since the yaml parser does not work with Records
type FrontMatter() =
    // member val will allow for mutable properties (needed my yaml parser)
    member val title = "" with get, set
    member val date = "" with get, set
    member val tags: string[] = [||] with get, set


type Document = {
    FrontMatter: FrontMatter;
    HtmlContent: string;
    SrcPath: string;
}

let yamlDecoder =
    YamlDotNet.Serialization.DeserializerBuilder().IgnoreUnmatchedProperties().Build()


// In order to parse the format
// ---
// key: value
// ---
// we need to manually control the parser, otherwise multiple documents would be parsed and
// we get an exception (since the second --- indicates the start of a new document)
let ParseFrontMatter fileContent =
    use stringReader = new IO.StringReader(fileContent)
    let parser = YamlDotNet.Core.Parser(stringReader)

    // consume the first document (the front matter)
    parser.Consume<StreamStart>() |> ignore
    yamlDecoder.Deserialize<FrontMatter>(parser)


let markdownPipeline =
    MarkdownPipelineBuilder().UseAdvancedExtensions().UseYamlFrontMatter().Build()

// TODO: manually parse and then render to HTML, in order to allow relative links within documents
let ParseMarkdown fileContent =
    Markdown.ToHtml(fileContent, markdownPipeline)


// TODO: move that to async later to potentially enable parallelization
let Parse path = 
    let content = IO.File.ReadAllText path
    let frontMatter = ParseFrontMatter content
    let renderedMarkdown = ParseMarkdown content

    // return final document
    {
        FrontMatter = frontMatter;
        HtmlContent = renderedMarkdown;
        SrcPath = path
    }