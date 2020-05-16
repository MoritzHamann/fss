open System
open FSS


[<EntryPoint>]
let main argv =
    let projectPath = IO.Path.GetFullPath argv.[0]
    let config =
        IO.File.ReadAllText (IO.Path.Combine [|projectPath; "config.toml"|])
        |> SiteConfig.Parse

    let themePath = IO.Path.Combine [|projectPath; "theme"|]
    let theme = Theme.ParseFolder themePath
    printf "%A\n" theme

    // Map.iter (fun name _ -> printf "%A\n" name) theme.Templates

    0
