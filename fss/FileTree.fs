namespace FSS

module FileTree =

    let Root = {Files = Map.empty; Folders =  Map.empty} 

    let HasChildFolder name node =
        Map.containsKey name node.Folders 

    let AddFolder name node =
        match HasChildFolder name node with
        | true -> node
        | false ->
            let folders = Map.add name {Files=Map.empty; Folders=Map.empty} node.Folders
            {node with Folders = folders}
        
    let GetFolderOrCreate name node =
        match Map.tryFind name node.Folders with
        | Some node -> node
        | None -> {Files=Map.empty; Folders=Map.empty}
    
    let AddFile name file node =
        {node with Files = Map.add name file node.Files}


    let rec ModifyAtPath node (Path path) modifier =
        match path with
        | [] -> modifier node
        | nextFolder::rest -> 
            let subFolder = GetFolderOrCreate nextFolder node
            let modifiedSubFolder = ModifyAtPath subFolder (Path rest) modifier
            {node with Folders = Map.add nextFolder modifiedSubFolder node.Folders}
