module FileTree

type FileTree<'T> =
    | File of 'T
    | Folder of string * FileTree<'T> list


let Root = Folder ("/", [])