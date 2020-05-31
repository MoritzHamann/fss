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
        Markdown.Parse(fileContent, markdownPipeline)


    let Parse content =
        {
            FrontMatter = ParseFrontMatter content;
            MarkdownAST = ParseMarkdown content;
        }

    let FromFile (path: string) =
        let content = IO.File.ReadAllText path
        let fileProperties = {
                FilePath = path;
                Created = IO.File.GetCreationTimeUtc path
        }
        let fileContent = Parse content

        // return new Document
        {
            Properties = fileProperties;
            Content = fileContent
        }
    
    let FileName doc = IO.Path.GetFileNameWithoutExtension doc.Properties.FilePath
    let PublicationYear doc = doc.Properties.Created.Year
    let PublicationMonth doc = doc.Properties.Created.Month
    let PublicationDay doc = doc.Properties.Created.Day
    let contentToString doc = Markdown.ToHtml


module DocumentSrcParser =

    let ParseFolder path =
        let isMarkdownFile (fileName: string) = (IO.Path.GetExtension fileName) = ".md"

        IO.Directory.GetFiles path
        |> Array.filter isMarkdownFile
        |> Array.map Document.FromFile
        |> Array.toList
    
    let ParseSrcFolder config =
        let path = SiteConfig.SourcePath config
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
