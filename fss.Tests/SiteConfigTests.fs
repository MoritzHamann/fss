module SiteConfigTests

open Xunit
open FsUnit
open Tomlyn
open FSS

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

            [someUnrelatedCollections.news]
            somekey = 'somevalue'
        """
    let config = Toml.Parse configContent |> fun c -> c.ToModel()

    [<Fact>]
    let ``extract collection names from config`` () =
        let collections = SiteConfig.ParseCollectionTables config

        collections |> should haveLength 2
        collections |> List.map SiteConfig.CollectionName |> should contain "posts"
        collections |> List.map SiteConfig.CollectionName |> should contain "announcements"

    [<Fact>]
    let ``parse collection information into ICollection`` () =
        let postsVerifier (c: CollectionConfig) =
            c.CollectionName  |> should equal "posts"
            c.OutputPath      |> should equal "/posts"
            c.SummaryTemplate |> should equal "posts_summary.hbs"
            c.DetailTemplate  |> should equal (Some "posts_detail.hbs")

        let announcmentsVerifier (c: CollectionConfig) =
            c.CollectionName  |> should equal "announcements"
            c.OutputPath      |> should equal "/announcements"
            c.SummaryTemplate |> should equal "announcements_summary.hbs"
            c.DetailTemplate  |> should equal None

        let collections = SiteConfig.ParseCollectionTables config

        collections |> should haveLength 2
        Assert.Collection(collections,
                          System.Action<_>(postsVerifier),
                          System.Action<_>(announcmentsVerifier))

    [<Fact>]
    let ``extract name from base config`` () =
        let name = SiteConfig.BaseConfig.getName config

        name |> should equal "Test Site"

    [<Fact>]
    let ``extract base URL from base config`` () =
        let baseUrl = SiteConfig.BaseConfig.getBaseUrl config

        baseUrl |> should equal "http://localhost:3000"

    [<Fact>]
    let ``extract default template from base config`` () =
        let template = SiteConfig.BaseConfig.getDefaultTemplate config

        template |> should equal "index.hbs"