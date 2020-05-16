namespace FSS

open System
open HandlebarsDotNet

module Theme = 
    module parser =

        let AddTemplate context (path:string) =
            let name = IO.Path.GetFileNameWithoutExtension(path)
            let templateContent = IO.File.ReadAllText(path)
            let template = Handlebars.Compile(templateContent)
            {context with Templates = Map.add name template context.Templates}


        let AddPartial context (path: string) =
            let name = IO.Path.GetFileNameWithoutExtension(path)
            let template = IO.File.ReadAllText(path)
            Handlebars.RegisterTemplate(name, template)
            { context with RegisteredPartials = name :: context.RegisteredPartials}


    let Empty = {Templates = Map.empty; RegisteredPartials = []}

    let ParseFolder themePath =
        // small helper deciding if the file is a template file
        let isTemplateFile (file:string) = IO.Path.GetExtension(file) = ".hbs"

        // parse and filter folders
        let partialPath = IO.Path.Combine [|themePath; "partials" |]
        let templateFiles = IO.Directory.GetFiles themePath  |> Array.filter isTemplateFile
        let partialFiles = IO.Directory.GetFiles partialPath |> Array.filter isTemplateFile

        // add each template and partial
        let context = Array.fold parser.AddTemplate Empty templateFiles
        let context = Array.fold parser.AddPartial context partialFiles
        context

    let Templates theme = theme.Templates

    let GetTemplate theme name =
        Map.find name (Templates theme)