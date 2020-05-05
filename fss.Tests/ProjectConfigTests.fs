module ProjectConfigTests

open Xunit
open FsUnit
open Tomlyn

type ConfigSetup() =
    let configContent = """
        name = "Test Site"
        base_url = "http://localhost:3000"
        default_template = "index.hbs"

        [collections]
            [collections.posts]
            path = "/posts"
            summary_template = "posts_summary.hbs"
            detail_template = "posts_detail.hbs"

            [collections.announcements]
            path = "/announcements"
            summary_template = "announcements_summary.hbs"

        [someUnrelatedCollection]
            [someUnrelatedCollections.pages]
            somekey = 'somevalue'

            [someUnrelatedCollections.announcements]
            somekey = 'somevalue'
        """
    let config = Toml.Parse configContent |> fun c -> c.ToModel()

    [<Fact>]
    let ``extract collection names from config`` () =
        let collections = ProjectConfig.getTomlCollections config

        collections |> should haveLength 2
        collections |> List.map fst |> should contain "posts"
        collections |> List.map fst |> should contain "announcements"

    [<Fact>]
    let ``extract name from base config`` () =
        let name = ProjectConfig.getName config

        name |> should equal "Test Site"

    [<Fact>]
    let ``extract base URL from base config`` () =
        let baseUrl = ProjectConfig.getUrl config

        baseUrl |> should equal "http://localhost:3000"

    [<Fact>]
    let ``extract default template from base config`` () =
        let template = ProjectConfig.getDefaultTemplate config

        template |> should equal "index.hbs"