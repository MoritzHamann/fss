namespace FSS

open FileTree

// TODO: implement a function which takes a list of Documents and an "output engine"
//       of type: Document -> FileTree<RenderTarget> -> FileTree<RenderTarget>,
//       where RenderTarget contains information about the document like
//       name, template used, content etc
//       Multiple output engines should be chainable


module OutputGenerator =
    let Generate docs engine =
        let rootTree = FileTree.Root
        rootTree |> List.foldBack engine docs