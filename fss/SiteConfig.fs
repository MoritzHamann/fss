namespace FSS

open System.IO
open Tomlyn

/// Example config file
///
/// name = "My awesome site"
/// base_url = "http://myawesomesite.com"
/// default_template = "index.tpl"
/// 
/// [collections]
///     [posts]
///     detail_template = "posts.tpl"
///     sumary_template = "posts_summary.tpl"
///     path = "posts"
/// 
///     [artwork]
///     detail_template = "artwork.tpl"
///     sumary_template = "artwork_summary.tpl"
///     path = "artworks"

module SiteConfig = 
    open System.Collections.Generic

    module CollectionConfig =
        module parser =
            let getOutputPath (config: Model.TomlTable) = config.["path"] :?> string

            let getSummaryTemplate (config: Model.TomlTable) = config.["summary_template"] :?> string

            let getDetailTemplate (config: Model.TomlTable) =
                match config.ContainsKey("detail_template") with
                | true -> Some (config.["detail_template"] :?> string)
                | false -> None
        
        let CreateFromTable name table = 
            {
                CollectionName = name
                OutputPath = parser.getOutputPath table
                SummaryTemplate = parser.getSummaryTemplate table
                DetailTemplate = parser.getDetailTemplate table
            }
        
    module BaseConfig =
        let getName (config: Model.TomlTable) = config.["name"] :?> string

        let getBaseUrl (config: Model.TomlTable) = config.["base_url"] :?> string

        let getDefaultTemplate (config: Model.TomlTable) = config.["default_template"] :?> string


    let ParseCollectionTables (baseTable: Model.TomlTable) =
        let isOfTypeTable (kv: KeyValuePair<_, Model.TomlObject>) = kv.Value.Kind = Model.ObjectKind.Table
        let createFromKvPair (kv:KeyValuePair<_, Model.TomlObject>) =
            let name, obj = kv.Deconstruct()
            let tomlObj = obj :?> Model.TomlTable
            CollectionConfig.CreateFromTable name tomlObj

        let found, obj = baseTable.TryGetToml("collections")
        match found with
        | false -> []
        | true ->
            let table = obj :?> Model.TomlTable
            table.GetTomlEnumerator()
                |> Seq.filter isOfTypeTable
                |> Seq.map createFromKvPair
                |> Seq.toList
            

    let ParseContent basePath (fileContent: string) =
        printf "%A\n" basePath
        let config = fileContent |> Toml.Parse |> fun config -> config.ToModel()
        
        {
            ProjectBasePath = basePath;
            BaseUrl = BaseConfig.getBaseUrl config;
            DefaultTemplate = BaseConfig.getDefaultTemplate config;
            Name = BaseConfig.getName config;
            Collections = ParseCollectionTables config
        }
    
    let Parse (basePath: string) =
        let configPath = Path.Combine [|basePath; "config.toml"|]
        let configContent = File.ReadAllText configPath
        ParseContent basePath configContent


    let ProjectPath config = config.ProjectBasePath

    let SourcePath config = Path.Combine [|ProjectPath config; "src"|]

    let Collections config = config.Collections
    let CollectionName collection = collection.CollectionName
