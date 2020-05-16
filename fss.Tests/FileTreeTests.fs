module FileTreeTests

open Xunit
open FsUnit
open FileTree


// [<Fact>]
// let ``create empty root node`` () =
//     FileTree.Create "/" |> should equal (Folder ("/", []))

// [<Fact>]
// let ``find node by path`` () =
//     let tree = Folder ("/", [
//         File "index.html";
//         Folder ("posts", [
//             File "first_post.html";
//             File "second_post.html"
//         ])
//     ])

//     let postsFolder = FileTree.GetPath "/posts" tree
//     let expected = 
//         Folder ("posts", [
//             File "first_post.html";
//             File "second_post.html"
//         ])
//     postsFolder |> should equal expected