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

    /// Represents the configuration for a specific Collection
    type Collection = {
        CollectionName : string;
        OutputPath : string;
        SummaryTemplate : string;
        DetailTemplate : Option<string>;
    }

    type SiteConfig = {
            ProjectBasePath: string;
            BaseUrl : string;
            DefaultTemplate : string;
            Name : string;
            Collections : Collection list;
    }

    module Collection =
        // =========
        // Accessors
        // =========
        let Name collection = collection.CollectionName
        let RenderPath collection = collection.OutputPath
        let SummaryTemplate collection = collection.SummaryTemplate
        let DetailTemplate collection = collection.DetailTemplate

        // ==================
        // computed accessors
        // ==================
        let SourcePath collection = Name collection

        // ======================
        // parser and constructor
        // ======================
        module parser =
            let getOutputPath (config: Model.TomlTable) = config.["path"] :?> string
            let getSummaryTemplate (config: Model.TomlTable) = config.["summary_template"] :?> string
            let getDetailTemplate (config: Model.TomlTable) =
                match config.ContainsKey("detail_template") with
                | true -> Some (config.["detail_template"] :?> string)
                | false -> None
        
        /// Creates a CollectionConfig based on a TomlTable
        let CreateFromTomlTable name table = 
            {
                CollectionName = name
                OutputPath = parser.getOutputPath table
                SummaryTemplate = parser.getSummaryTemplate table
                DetailTemplate = parser.getDetailTemplate table
            }
        

    // ====================================== 
    // Everything config related for the site
    // ====================================== 

    // =========
    // Accessors
    // =========
    let ProjectPath config = config.ProjectBasePath
    let SiteUrl config = config.BaseUrl
    let SiteName config = config.Name
    let DefaultTemplate config = config.DefaultTemplate
    let Collections config = config.Collections

    // ===================
    // Computed attributes
    // ===================
    let SourcePath config = Path.Join [|ProjectPath config; "src"|]
    let ThemePath config = Path.Join [|ProjectPath config; "theme"|]
    let outputPath config = Path.Join [|ProjectPath config; "out"|]

    // ======================
    // Parser and constructor
    // ======================
    module parser =
        let getSiteName (config: Model.TomlTable) = config.["name"] :?> string
        let getSiteUrl (config: Model.TomlTable) = config.["base_url"] :?> string
        let getDefaultTemplate (config: Model.TomlTable) = config.["default_template"] :?> string

        /// Parses all defined collections into Collection configs
        let ParseCollectionTables (baseTable: Model.TomlTable) =
            let isOfTypeTable (kv: KeyValuePair<_, Model.TomlObject>) = kv.Value.Kind = Model.ObjectKind.Table
            let createFromKvPair (kv:KeyValuePair<_, Model.TomlObject>) =
                let name, obj = kv.Deconstruct()
                let tomlObj = obj :?> Model.TomlTable
                Collection.CreateFromTomlTable name tomlObj

            let found, obj = baseTable.TryGetToml("collections")
            match found with
            | false -> []
            | true ->
                let table = obj :?> Model.TomlTable
                table.GetTomlEnumerator()
                    |> Seq.filter isOfTypeTable
                    |> Seq.map createFromKvPair
                    |> Seq.toList
            

        let Parse basePath (fileContent: string) =
            // parse the content into a TOML model
            let config = fileContent |> Toml.Parse |> fun config -> config.ToModel()

            // return the extracted information 
            {
                ProjectBasePath = basePath;
                BaseUrl = getSiteUrl config;
                DefaultTemplate = getDefaultTemplate config;
                Name = getSiteName config;
                Collections = ParseCollectionTables config
            }
    
    /// Parse the config file into a SiteConfig type
    let FromFile (basePath: string) =
        let configPath = Path.Combine [|basePath; "config.toml"|]
        let configContent = File.ReadAllText configPath
        parser.Parse basePath configContent

