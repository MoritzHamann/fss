namespace FSS

open System
open Markdig
open YamlDotNet.Core
open YamlDotNet.Core.Events


module Document =

    /// Yaml and Markdown parser configurations
    let yamlDecoder =
        YamlDotNet.Serialization.DeserializerBuilder().IgnoreUnmatchedProperties().Build()
    let markdownPipeline =
        MarkdownPipelineBuilder().UseAdvancedExtensions().UseYamlFrontMatter().Build()


    /// The YamlParser does not allow for documents with two tribble dashes in a document, as in
    ///
    /// ---
    /// key: value
    /// ---
    /// ... some markdown content
    ///
    /// since the second tribble dash indicates a new yaml document, which in this case is
    /// not valid. We manually control the Parser to only read the first document in the file
    let ParseFrontMatter fileContent =
        use stringReader = new IO.StringReader(fileContent)
        let parser = YamlDotNet.Core.Parser(stringReader)

        // consume the first document (the front matter)
        parser.Consume<StreamStart>() |> ignore
        yamlDecoder.Deserialize<FrontMatter>(parser)


    // TODO: manually parse and then render to HTML, in order to allow relative links within documents
    let ParseMarkdown fileContent =
        Markdown.ToHtml(fileContent, markdownPipeline)


    let Parse content =
        {
            FrontMatter = ParseFrontMatter content;
            HtmlContent = ParseMarkdown content;
        }

    let FromFile (path: string) =
        let name = IO.Path.GetFileNameWithoutExtension path
        let content = IO.File.ReadAllText path

        {
            FileName = name;
            Content = Parse content
        }


module DocumentSrcParser =

    let ParseFolder path =
        let isMarkdownFile (fileName: string) = (IO.Path.GetExtension fileName) = ".md"

        IO.Directory.GetFiles path
        |> Array.filter isMarkdownFile
        |> Array.map Document.FromFile
        |> Array.toList
    
    let ParseSrcFolder config path =
        let topLevelDocs = ParseFolder path
        let collectionDocs =
            (SiteConfig.Collections config, Map.empty)
            ||> List.foldBack (fun collection state ->
                let name = SiteConfig.CollectionName collection
                let collectionPath = IO.Path.Combine [|path; name|]
                Map.add name (ParseFolder collectionPath) state
            )
        {
            TopLevelDocs = topLevelDocs;
            CollectionDocs = collectionDocs;
        }
