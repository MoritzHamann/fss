namespace FSS

open System.IO

module SrcFiles =
    type FileType =
        | Page
        | Post of SiteConfig.Collection
        | Image
        | Static

    type SrcFiles = {
        Path: string;
        Type: FileType;
    }

    let getFileType (filePath: string) =
        match Path.GetExtension filePath with
        | ".md" -> Page
        | ".png" | ".jpg" | ".jpeg" | ".gif" -> Image
        | _ -> Static
    
    let ToSrcFile filePath =
        {Path = filePath; Type = getFileType filePath}

    let ParseFolder path =
        let files = Directory.GetFiles path
        files |> Array.map ToSrcFile |> Array.toList

    let ParseCollectionFolder basePath collection =
        let collectionPath = SiteConfig.Collection.SourcePath collection
        let files = ParseFolder basePath

        // replace all `Page` type with Post
        files |> List.map (fun file ->
            match file.Type with
            | Page -> {file with Type = Post collection}
            | _ -> file
        )

    /// Collects a list of source files for the project
    let Collect config projectPath =
        let srcFolder = SiteConfig.SourcePath config
        let collections = SiteConfig.Collections config

        // parse top level and collection files
        let topLevelFiles = ParseFolder srcFolder
        let collectionFiles = collections |> List.collect (ParseCollectionFolder srcFolder)

        // return a concated list
        List.append topLevelFiles collectionFiles