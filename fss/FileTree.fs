namespace FSS

module FileTree =

    let Root = Folder (Map.empty) 

    let isFile = function
    | File _ -> true
    | Folder _ -> false

    let isFolder = function
    | File _ -> false
    | Folder _ -> true

    let ChildExists name node =
        match node with
        | File _ -> false
        | Folder content -> Map.containsKey name content

    let GetChild name node =
        match node with
        | File _ -> None
        | Folder content -> Map.tryFind name content
    
    let GetFolder name node =
        Option.filter isFolder (GetChild name node)
    
    let GetFile name node =
        Option.filter isFile (GetChild name node)

    /// Adds or replaces a file in a folder.
    /// Throws if node is a file rather than a folder or if folder already exists
    let AddChild name child node =
        match node with
        | File _ -> failwith "Can not add file to a file"
        | Folder content ->
            match GetChild name node with
            | Some _ -> failwith "File or folder with same name already exists"
            | None -> Folder (Map.add name child content)

    /// Retrieve the subfolder by name, or create an empty one
    /// Throws if file with the same name exists in the folder
    let GetOrCreateSubfolder name node =
        match GetFolder name node with
        | None -> AddChild name (Folder Map.empty) node
        | Some f ->  f

    let rec ModifyAtPath (Path path) modifier node =
        let content = 
            match node with
            | File _ -> failwith "can not modify file, only folders"
            | Folder c -> c

        match path with
        | [] -> modifier node
        | nextFolder::rest -> 
            let subFolder = GetOrCreateSubfolder nextFolder node
            let modifiedSubFolder = ModifyAtPath (Path rest) modifier subFolder 
            Folder (Map.add nextFolder modifiedSubFolder content)
