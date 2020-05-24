namespace FSS


module OutputGenerator =

    /// TODO: Decide what to do if the pathString does not contain an %n for the document name
    /// Generate the final path for the document. pathString can contain the following replacments:
    /// %m -> month
    /// %d -> day
    /// %y -> year
    /// %n -> name
    let filePathForDoc (pathString: string) doc =
        let name = Document.FileName doc 
        let year = Document.PublicationYear doc |> string
        let month = Document.PublicationMonth doc |> string
        let day = Document.PublicationDay doc |> string
        let replacements = [
            (@"%d", day);
            (@"%m", month);
            (@"%y", year);
            (@"%n", name);
        ]
        let path = (pathString, replacements)
                    ||> List.fold (fun path (rep, value) -> path.Replace(rep, value))

        path.Split "/" |> Array.toList |> List.filter (fun p -> p <> "") |> Path


    let SingleDetailDocEngine pathString doc filetree =
        let path = filePathForDoc pathString doc
        let target = DetailDoc doc
        let fileName = Document.FileName doc + ".html"
        let insertModification = FileTree.AddChild fileName (File target)
        
        FileTree.ModifyAtPath path insertModification filetree

    let Generate docs engine =
        let rootTree = FileTree.Root
        (docs, rootTree) ||> List.foldBack engine
