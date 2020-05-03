module ProjectConfigTests

open Xunit
open FsUnit
open Tomlyn

[<Fact>]
let ``extract collection names from config`` () =
    let configContent = @"
    [collections]
        [collections.posts]
        somekey = 'somevalue'

        [collections.announcements]
        somekey = 'somevalue'

    [someOtherCollection]
        [someOtherCollections.posts]
        somekey = 'somevalue'

        [someOtherCollections.announcements]
        somekey = 'somevalue'
    "
    let config = Toml.Parse configContent |> fun c -> c.ToModel()
    let collections = ProjectConfig.getTomlCollections config

    collections |> should haveLength 2
    collections |> List.map fst |> should contain "posts"
    collections |> List.map fst |> should contain "announcements"

[<Fact>]
let ``extractName from base config`` () =
    let dummyKey = System.Collections.Generic.KeyValuePair<string,obj>("name", "ConfigName")
    let dummyTabel = Model.TomlTable()
    dummyTabel.Add dummyKey

    let name = ProjectConfig.getName dummyTabel

    name |> should equal "ConfigName"