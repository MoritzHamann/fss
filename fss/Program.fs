open System
open System.IO
open FSS


/// TODO: decouple the generation of the tree and the rendertargets. replace with
///       1.)  generate render targets from docs, generating map:
///             doc name -> (path * template * (document list)) list
///       2.) generate a tree form the render target
///       3.) render the tree into folder/files, using the name -> rendertargets map for
///           helpers such as `url_for()`
///     => do we still need the tree?

[<EntryPoint>]
let main argv =
    let projectPath = Path.GetFullPath argv.[0]
    let config = SiteConfig.Parse projectPath

    printf "%A\n" config

    let docs = DocumentSrcParser.ParseSrcFolder config
    let engine = OutputGenerator.SingleDetailDocEngine ""
    let filetree = OutputGenerator.Generate docs.TopLevelDocs engine

    let proPath = Path.ofString projectPath
    let outPath = Path.Append proPath "out"
    Renderer.Render outPath filetree

    printf "%A\n" filetree

    0
