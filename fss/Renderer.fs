namespace FSS

open System.IO

module Renderer =

    /// TODO: get template and render with frontmatter + content
    let renderDetail (writer: StreamWriter) doc =
        // let frontMatter = Document.frontMatter doc
        // let content = Document.contentToString doc
        // let template = RenderTarget.template
        // let docContent = {
        //     frontMatter = frontMatter;
        //     content = content
        // }
        // let fileContent = Template.Render template [docContent]

        // writer.Write(fileContent)
        // writer.Flush()
        ()

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
            /// helper to call render the child witht he appropriate path
            let renderChild entryName entryNode =
                let newPath = Path.Append path entryName
                renderNodes newPath entryNode

            /// create the current path directory and then render all its childs
            Directory.CreateDirectory (Path.toString path) |> ignore
            Map.iter renderChild content


    let Render outputBaseFolder fileTree =
        let stringPath = Path.toString outputBaseFolder
        if Directory.Exists stringPath then
            Directory.Delete(stringPath, true)
        Directory.CreateDirectory stringPath |> ignore

        renderNodes outputBaseFolder fileTree
        
        