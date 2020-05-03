module Theme

open System
open HandlebarsDotNet
open Document


type RenderContext = {
    Templates: Map<string, Func<obj, string>>;
    RegisteredPartials: List<string>
}

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


let EmptyContext =
    {Templates = Map.empty; RegisteredPartials = []}


let ParseFolder themePath =
    // small helper deciding if the file is a template file
    let isTemplateFile (file:string) = IO.Path.GetExtension(file) = ".hbs"

    // parse and filter folders
    let partialPath = IO.Path.Combine [|themePath; "partials" |]
    let templateFiles = IO.Directory.GetFiles themePath  |> Array.filter isTemplateFile
    let partialFiles = IO.Directory.GetFiles partialPath |> Array.filter isTemplateFile

    // add each template and partial
    let context = Array.fold AddTemplate EmptyContext templateFiles
    let context = Array.fold AddPartial context partialFiles
    context


// let Render context doc templateName =
//     let template = context.Templates.Item templateName
//     template.Invoke(doc.FrontMatter)
