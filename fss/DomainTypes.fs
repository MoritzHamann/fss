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

    type DocumentProperties = {
        FilePath: string;
        Created: DateTime;
    }

    type Document = {
        Properties: DocumentProperties;
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
    
    type Path = Path of string list

    type ContentType<'T> =
        | File of 'T
        | Folder of Map<string, ContentType<'T>>


    /// =======================
    /// Rendering related types
    /// =======================
    
    type RenderTarget =
        | DetailDoc of Document
        | Summary of Document list