open System


[<EntryPoint>]
let main argv =
    let projectPath = IO.Path.GetFullPath argv.[0]
    let config = ProjectConfig.Parse projectPath

    let themePath = IO.Path.Combine [|projectPath; "theme"|]
    let theme = Theme.ParseFolder themePath

    Map.iter (fun name _ -> printf "%A\n" name) theme.Templates

    0
