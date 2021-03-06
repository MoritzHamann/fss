namespace FSS

open System
open Markdig

[<AutoOpen>]
module DomainTypes = 

    /// ======================
    /// Document related types
    /// ======================
    
    type FrontMatter() =
        member val title = "" with get, set
        member val date = "" with get, set
        member val tags: string[] = [||] with get, set

    type DocumentContent = {
        FrontMatter: FrontMatter;
        MarkdownAST: Syntax.MarkdownDocument;
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

    type TreeContent<'T> =
        | File of 'T
        | Folder of Map<string, TreeContent<'T>>


    /// =======================
    /// Rendering related types
    /// =======================
    
    type RenderTarget =
        | DetailDoc of Template * Document
        | Summary of Template * Document list
        | StaticFile of Path