module FileTreeTests

open Xunit
open FsUnit
open FSS


[<Fact>]
let ``create empty root node`` () =
    FileTree.Root |> should equal (Folder Map.empty)


[<Fact>]
let ``AddChild adds non existing folder`` () =
    let root = FileTree.Root
    let expected = Folder (Map.ofList[
        ("MyFolder", Folder Map.empty)
    ])
    FileTree.AddChild "MyFolder" (Folder Map.empty) root |> should equal expected


[<Fact>]
let ``AddChild adds folder and file`` () =
    let root = FileTree.Root
    let expected = Folder (Map.ofList[
        ("MyFolder", Folder Map.empty);
        ("MyFile", File "someFile")
    ])
    FileTree.AddChild "MyFolder" (Folder Map.empty) root
    |> FileTree.AddChild "MyFile" (File "someFile")
    |> should equal expected


[<Fact>]
let ``AddChild will not override existing folder`` () =
    let baseNode = Folder (Map.ofList [
        ("MyFolder", Folder (Map.ofList [
            ("SomeName", File "someFile")
        ]))
    ])

    (fun () -> FileTree.AddChild "MyFolder" (Folder Map.empty) baseNode |> ignore)
        |> should throw typeof<System.Exception>



// [<Fact>]
// let ``Add folder at in existing sub folder`` () =

//     let result = FileTree.ModifyAtPath baseNode (Path ["MyFolder"]) (FileTree.AddFolder "MySecondFolder")
//     result |> should equal expected

