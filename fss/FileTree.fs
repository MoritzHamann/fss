module FileTree

type FileTree<'T> =
    | Doc of 'T
    | Node of (string * FileTree<'T> list)


let Create = Node ("root", [])
