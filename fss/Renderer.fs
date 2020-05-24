namespace FSS

open System.IO

module Renderer =

    /// TODO: get template and render with frontmatter + content
    let renderDetail (writer: StreamWriter) doc =
        writer.Write(doc.Content.HtmlContent)
        writer.Flush()

    let renderSummary writer docs = ()

    let renderStatic (writer: StreamWriter) assetPath =
        let stringPath = Path.toString assetPath
        let assetFile = File.Open(stringPath, FileMode.Create)
        writer.Write(assetFile)

    let renderFile (writer: StreamWriter) renderTarget =
        match renderTarget with
        | DetailDoc doc -> renderDetail writer doc
        | Summary docs -> renderSummary writer docs
        | StaticFile assetPath -> renderStatic writer assetPath

    let rec renderNodes path currentNode =
        match currentNode with
        | File renderTarget ->
            let writer = File.CreateText (Path.toString path)
            renderFile writer renderTarget
        | Folder content ->
            let renderChild entryName entryNode =
                let newPath = Path.Append path entryName
                renderNodes newPath entryNode

            Directory.CreateDirectory (Path.toString path) |> ignore
            Map.iter renderChild content


    let Render outputBaseFolder fileTree =
        let stringPath = Path.toString outputBaseFolder
        if Directory.Exists stringPath then
            Directory.Delete(stringPath, true)
        Directory.CreateDirectory stringPath |> ignore

        renderNodes outputBaseFolder fileTree
        
        