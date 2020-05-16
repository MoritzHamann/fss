module FileTreeTests

open Xunit
open FsUnit
open FSS


[<Fact>]
let ``create empty root node`` () =
    FileTree.Root |> should equal {Files=Map.empty; Folders=Map.empty}


[<Fact>]
let ``Add folder which does not exist`` () =
    let root = FileTree.Root
    let expected = {
        Files=Map.empty;
        Folders=Map.ofList [
            ("MyFolder", {
                Files=Map.empty;
                Folders=Map.empty
            })
        ]
    }

    FileTree.AddFolder "MyFolder" root |> should equal expected


[<Fact>]
let ``Do not add duplicate folder`` () =
    let baseNode = {
        Files=Map.empty;
        Folders=Map.ofList [
            ("MyFolder", {
                Files=Map.ofList[("someFile", "somecontent")];
                Folders=Map.empty
            })
        ]
    }

    FileTree.AddFolder "MyFolder" baseNode |> should equal baseNode


[<Fact>]
let ``Add folder at in existing sub folder`` () =
    let baseNode = {
        Files=Map.empty;
        Folders=Map.ofList [
            ("MyFolder", {
                Files=Map.ofList[("someFile", "somecontent")];
                Folders=Map.empty
            })
        ]
    }
    let expected = {
        Files=Map.empty;
        Folders=Map.ofList [
            ("MyFolder", {
                Files=Map.ofList[("someFile", "somecontent")];
                Folders=Map.ofList [
                    ("MySecondFolder", {
                        Files=Map.empty;
                        Folders=Map.empty
                    })
                ]}
            )
        ]
    }

    let result = FileTree.ModifyAtPath baseNode (Path ["MyFolder"]) (FileTree.AddFolder "MySecondFolder")
    result |> should equal expected

