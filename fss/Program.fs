open System
open System.IO
open FSS

[<EntryPoint>]
let main argv =
    let projectPath = Path.GetFullPath argv.[0]
    let config = SiteConfig.Parse projectPath

    printf "%A\n" config

    let docs = DocumentSrcParser.ParseSrcFolder config
    let engine = OutputGenerator.SingleDetailDocEngine ""
    let filetree = OutputGenerator.Generate docs.TopLevelDocs engine

    printf "%A\n" filetree


    0
