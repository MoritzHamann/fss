namespace FSS

open System

[<AutoOpen>]
module DomainTypes = 

    /// ============
    /// site config
    /// ============
    
    type CollectionConfig = {
        CollectionName : string;
        OutputPath : string;
        SummaryTemplate : string;
        DetailTemplate : Option<string>;
    }
    
    type SiteConfig = {
        BaseUrl : string;
        DefaultTemplate : string;
        Name : string;
        Collections : CollectionConfig list;
    }

    /// ======================
    /// Document related types
    /// ======================
    
    type FrontMatter() =
        member val title = "" with get, set
        member val date = "" with get, set
        member val tags: string[] = [||] with get, set

    type DocumentContent = {
        FrontMatter: FrontMatter;
        HtmlContent: string;
    }

    type Document = {
        FileName: string;
        Content: DocumentContent;
    }

    type SiteDocuments = {
        TopLevelDocs: Document list;
        CollectionDocs: Map<string, Document list>
    }


    /// ===================
    /// Theme related types
    /// ===================
    
    type TemplateName = string
    type Template = Func<obj, string>

    type Theme = {
        Templates: Map<TemplateName, Template>;
        RegisteredPartials: TemplateName list;
    }

    /// =========
    /// File tree
    /// =========
    
    type FileTree<'T> =
        | File of 'T
        | Folder of FileTreeFolder<'T> 
    
    and FileTreeFolder<'T> =
        {
            Name: string;
            Content: FileTree<'T> list
        }