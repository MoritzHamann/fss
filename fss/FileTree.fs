module FileTree

type FileTree<'T> =
    | Doc of 'T
    | Node of FileTree<'T> list


let Create = Node []
