namespace FSS

// TODO: implement a function which takes a list of Documents and an "output engine"
//       of type: Document -> FileTree<RenderTarget> -> FileTree<RenderTarget>,
//       where RenderTarget contains information about the document like
//       name, template used, content etc
//       Multiple output engines should be chainable

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

        (pathString, replacements) ||> List.fold (fun path (rep, value) -> path.Replace(rep, value))


    let SingleDetailDocEngine pathString doc filetree =
        let path = filePathForDoc pathString doc
        filetree

    let Generate docs engine =
        let rootTree = FileTree.Root
        (docs, rootTree) ||> List.foldBack engine