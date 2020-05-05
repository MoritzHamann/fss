module ProjectConfig

open System
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


type ICollection = {
    Name : string;
    Path : string;
    SummaryTemplate : string;
    DetailTemplate : Option<string>;
}

type IConfig = {
    BaseUrl : string;
    DefaultTemplate : string;
    Name : string;
    Collections : ICollection list;
}


let getName (config: Model.TomlTable) = config.["name"] :?> string
let getUrl (config: Model.TomlTable) = config.["base_url"] :?> string
let getPath (config: Model.TomlTable) = config.["path"] :?> string
let getDefaultTemplate (config: Model.TomlTable) = config.["default_template"] :?> string
let getSummaryTemplate (config: Model.TomlTable) = config.["summary_template"] :?> string
let getDetailTemplate (config: Model.TomlTable) =
    match config.ContainsKey("detail_template") with
    | true -> Some (config.["detail_template"] :?> string)
    | false -> None


let ParseCollection (name: string) (collection: Model.TomlTable) =
    {
        Name = name
        Path = getPath collection
        SummaryTemplate = getSummaryTemplate collection
        DetailTemplate = getDetailTemplate collection
    }

// extract the collections. The TOML parser makes this a little bit tricky since we need to cast to
// TomlTable all the time.
// 1. if set, get the table "collections"
// 2. filter all elements in that table, which are also tables
// 3. extract the KekValuePair<string, obj> into a tuple of (key, TomlTable)
let getTomlCollections (config: Model.TomlTable) =
        let isTomlTable v = v.GetType() = Model.TomlTable().GetType()

        match config.ContainsKey "collections" with
        | true -> config.["collections"] :?> Model.TomlTable
                    |> Seq.filter (fun kvPair -> isTomlTable kvPair.Value)
                    |> Seq.map (fun kvPair -> (kvPair.Key, (kvPair.Value :?> Model.TomlTable)))
                    |> Seq.toList
        | false -> []

let Parse (configContent: string) = 
    // Parse the config file via TOML
    let config = configContent |> Toml.Parse |> fun config -> config.ToModel()
    let collections = getTomlCollections config

    // return the new config object
    {
        Name = getName config
        BaseUrl = getUrl config
        DefaultTemplate = getDefaultTemplate config
        Collections = collections |> List.map (fun (name, table) -> ParseCollection name table)
    }