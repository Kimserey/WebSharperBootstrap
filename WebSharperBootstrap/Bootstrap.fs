namespace WebSharperBootstrap

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client
open WebSharper.Forms

[<JavaScript; AutoOpen>]
module Bootstrap =

    type View 
        with
            /// Used to create custom error messages for forms and be able to filter the custom messages for display
            static member CustomErrorId = 5000
            static member ThroughCustomId(view: View<Result<'T>>) : View<Result<'T>> =
                view |> View.Map (fun x -> match x with Success _ -> x | Failure msgs -> Failure (msgs |> List.filter (fun m -> m.Id = View.CustomErrorId)))
            static member ErrorMessages(view: View<Result<'T>>, rv: Var<'U>): View<string list> =
                View.Through(view, rv) |> View.Map (fun x -> match x with Result.Success _ -> [] | Result.Failure err -> err |> List.map(fun e -> e.Text))
                
    /// Extensions to WebSharper attr type.
    type attr
    with
        static member role x = Attr.Create "role" "presentation"
        
        static member dataToggle x = Attr.Create "data-toggle" x
        static member dataTarget x = Attr.Create "data-target" x

        static member ariaControls x = Attr.Create "aria-controls" x
        static member ariaLabel x = Attr.Create "aria-label" x
        static member ariaHidden x = Attr.Create "aria-hidden" x
        static member ariaExpanded x = Attr.Create "aria-expanded" x

    type Header = {
        Text: string
        HeaderType: HeaderType 
        Icon: string option
    } with
        static member Create(text, headerType) =
            { Text = text
              HeaderType = headerType
              Icon = None }
        member x.WithIcon(icon) = { x with Icon = Some icon }
        static member Render x =
            let elements marginRight =
                [ (match x.Icon with Some icon -> iAttr [ attr.``class`` ("fa " + icon); attr.style ("margin-right:" + marginRight) ] [] :> Doc | _ -> Doc.Empty); text x.Text ]
            match x.HeaderType with
            | H1 -> h1 (elements "15px")
            | H2 -> h2 (elements "15px")
            | H3 -> h3 (elements "15px")
            | H4 -> h4 (elements "5px")
            | H5 -> h5 (elements "5px")
            | H6 -> h6 (elements "5px")
        member x.Render() = Header.Render x
    and HeaderType = H1 | H2 | H3 | H4 | H5 | H6
    
    type Label = {
        Text: string
        LabelStyle: LabelStyle
    } with
        static member Create(text, style) =
            { Text = text
              LabelStyle = style }
        static member Render x =
            spanAttr [ attr.style "margin: 3px 3px 3px 0px; display: inline-block;" 
                       attr.``class`` ("label "  + x.LabelStyle.CssClass()) ] [ text x.Text ]
        member x.Render() =
            spanAttr [ attr.style "margin: 3px 3px 3px 0px; display: inline-block;" 
                       attr.``class`` ("label "  + x.LabelStyle.CssClass()) ] [ text x.Text ]
    and LabelStyle =
        | Default
        | Primary
        | Info
        | Success
        | Warning
        | Danger
        with
            member x.CssClass() =
                match x with 
                | Default -> "label-default"
                | Primary -> "label-primary"
                | Info    -> "label-info"
                | Warning -> "label-warning"
                | Danger  -> "label-danger"
                | Success -> "label-link"

    /// Build a Bootstrap grid row with columns
    type GridRow = {
        Columns: GridColumn list
    } with
        static member Create() =
            { Columns = [] }
        static member Create(columns) =
            { Columns = columns }
        static member Render x =
            divAttr [ attr.``class`` "row" ] (x.Columns |> List.map GridColumn.Render |> Seq.cast)
        member x.Render() = GridRow.Render x
        member x.AddColumn(columnContent, columnSizes) = { x with Columns = [ yield! x.Columns
                                                                              yield GridColumn.Create(columnContent, columnSizes) ] }
    and GridColumn = {
        Content: Doc
        GridSizes: GridColumnSize list
    } with
        static member Create(content, gridSizes) =
            { Content = content
              GridSizes = gridSizes }
        static member Render x =
            divAttr [ attr.``class`` (x.GridSizes |> List.map GridColumnSize.CssClass |> String.concat " ") ] [ x.Content ]
        member x.Render() = GridColumn.Render x
    and GridColumnSize =
        /// col-xs-12 
        | ColXs12
        /// col-sm-3
        | ColSm3
        /// col-sm-4
        | ColSm4
        /// col-sm-6
        | ColSm6
        /// col-sm-8
        | ColSm8
        /// col-sm-12
        | ColSm12
        /// col-md-2
        | ColMd2
        /// col-md-3
        | ColMd3
        /// col-md-4
        | ColMd4
        /// col-md-6
        | ColMd6
        /// col-md-9
        | ColMd9
        /// col-md-12
        | ColMd12
        with
            static member CssClass x =
                match x with
                | ColXs12 -> "col-xs-12"
                | ColSm3  -> "col-sm-3"
                | ColSm4  -> "col-sm-4"
                | ColSm6  -> "col-sm-6"
                | ColSm8  -> "col-sm-8"
                | ColSm12 -> "col-sm-12"
                | ColMd2  -> "col-md-2"
                | ColMd3  -> "col-md-3"
                | ColMd4  -> "col-md-4"
                | ColMd6  -> "col-md-6"
                | ColMd9  -> "col-md-9"
                | ColMd12 -> "col-md-12"
            member x.CssClass() = GridColumnSize.CssClass x

    /// Wraps elements in a container (display centered) or container-fluid (display full width).
    type Container = {
        Content: Doc list
        ContainerType: ContainerType
    } with
        static member Create() = { Content = []; ContainerType = ContainerType.Normal }
        static member Create(containerType) = { Content = []; ContainerType = containerType }
        static member Create(content, containerType) = { Content = content; ContainerType = containerType }
        static member Render x =
            divAttr [ attr.``class`` (match x.ContainerType with Normal -> "container" | Fluid -> "container-fluid") ] x.Content
        member x.Render() = Container.Render x
        member x.AddRow row = { x with Content = [ yield! x.Content
                                                   yield GridRow.Render row :> Doc ] }
    and ContainerType =
        | Normal
        | Fluid
        
    type Hyperlink = {
        Action: HyperlinkAction
        Content: HyperlinkContent
        AriaLabel: string option
        Role: string option
        DataToggle: string option
    } with
        static member Create(action, content) =
            { Action = action
              Content = Content content
              AriaLabel = None
              Role = None
              DataToggle = None }
        static member Create(action, content) =
            { Action = action
              Content = Text content
              AriaLabel = None
              Role = None
              DataToggle = None }
        static member Render x =
            aAttr [ yield! (match x.Action with 
                            | Href href -> [ attr.href href ] 
                            | Action action -> [ attr.href "#"
                                                 on.click(fun _ _ -> action()) ]) 
                    yield! (match x.AriaLabel  with Some label  -> [ attr.ariaLabel label ]   | None -> []) 
                    yield! (match x.Role       with Some role   -> [ attr.role role ]         | None -> []) 
                    yield! (match x.DataToggle with Some toggle -> [ attr.dataToggle toggle ] | None -> []) ] 
                  [ (match x.Content with
                     | Content doc -> doc
                     | Text txt -> text txt) ]
        member x.Render() = Hyperlink.Render x
        member x.WithAriaLabel label = { x with AriaLabel = Some label }
        member x.WithRole role = { x with Role = Some role }
        member x.WithDataToggle toggle = { x with DataToggle = Some toggle }
    and HyperlinkAction =
        | Href of string
        | Action of (unit -> unit)
    and HyperlinkContent =
        | Content of Doc
        | Text of string

    type Button = { 
        Title: string
        Action: unit -> unit
        ButtonStyle: ButtonStyle
        CssClass: string option
        ExtraAttrs: Attr list
    } with
        static member Create(title, style, action) = 
            { Title = title
              Action = action
              ButtonStyle = style
              CssClass = None
              ExtraAttrs = [] }
        static member Render x =  Doc.Button x.Title [ attr.``class`` (sprintf "btn %s %s" (x.ButtonStyle.CssClass()) (match x.CssClass with Some cls -> cls | None -> "")) 
                                                       attr.style "margin-bottom: 15px;" ] x.Action
        member x.Render() = Button.Render x
        member x.WithCssClass cls = { x with CssClass = Some cls }
        member x.WithExtraAttrs attrs = { x with ExtraAttrs = attrs }
    and ButtonStyle =
        | Default
        | Primary
        | Info
        | Warning
        | Danger
        | Link
        with
            member x.CssClass() =
                match x with 
                | Default -> "btn-default"
                | Primary -> "btn-primary"
                | Info    -> "btn-info"
                | Warning -> "btn-warning"
                | Danger  -> "btn-danger"
                | Link    -> "btn-link"

    /// Renders a bootstrap breadcrumb nav bar.
    type BreadcrumbBar = {
        Breadcrumbs: Breadcrumb list
    } with
        static member Create breadcrumbs = 
            { Breadcrumbs = breadcrumbs }
        static member Render x = 
            olAttr [ attr.``class`` "breadcrumb" ] (x.Breadcrumbs |> List.map Breadcrumb.Render |> Seq.cast)
        member x.Render() = BreadcrumbBar.Render x
    and Breadcrumb = {
        Title: string
        RelativeUrl: string
        IsActive: bool
        Decoration: BreadCrumbDecoration option
    } with
        static member Create(title, relativeUrl) = 
            { Title = title
              RelativeUrl = relativeUrl
              IsActive = false
              Decoration = None }
        static member CreateActive(title) = 
            { Title = title
              RelativeUrl = ""
              IsActive = true 
              Decoration = None }
        static member Render x =
            let currentElement =
                Doc.Concat (match x.Decoration with
                            | Some (Image src) -> [ imgAttr [ attr.src src
                                                              attr.``class`` "img-circle img-responsive"
                                                              attr.style "width: 25px; height: 25px; margin-right: 5px;display: inline;" ] [] :> Doc
                                                    text x.Title ]
                            | Some (Icon icon) -> [ iAttr [ attr.``class`` ("fa " + icon)
                                                            attr.style "margin-right: 5px;" ] []
                                                    text x.Title ]
                            | None -> [ text x.Title ])

            match x.IsActive with
            | true  -> liAttr [ attr.``class`` "active" ] [ currentElement ]
            | false -> li [ Hyperlink.Create(Href x.RelativeUrl, currentElement).Render() ]
        member x.Render() = Breadcrumb.Render x
        member x.WithDecoration decoration = { x with Decoration = Some decoration }
    and BreadCrumbDecoration =
        | Image of src: string
        | Icon of string

    /// Renders a description list <dl>.
    type DescriptionList = {
        Descriptions: DescriptionTerm list
        Orientation: DescriptionOrientation
        CssClass: string
    } with
        static member Create(descriptions, orientation) = 
            { Descriptions = descriptions
              Orientation = orientation
              CssClass = "" }
        static member Render x =
            dlAttr [ attr.``class`` (sprintf "%s %s" x.CssClass (match x.Orientation with Horizontal -> "dl-horizontal" | Vertical -> "")) ] (List.map DescriptionTerm.Render x.Descriptions)
        member x.Render() = DescriptionList.Render x
        member x.WithCssClass cls = { x with CssClass = cls }
    and DescriptionTerm = {
        Term: string
        Description: Description
    } with
        static member Create(term, description) =
            { Term = term
              Description = Description.Doc description }
        static member Create(term, description) =
            { Term = term
              Description = Description.Text description }
        static member Create(term) =
            { Term = term
              Description = Description.Empty }
        static member Render x =
            [ dt [ text x.Term ]
              dd [ (match x.Description with
                   | Doc doc  -> doc
                   | Text txt -> text txt
                   | Empty -> Doc.Empty) ] ]
            |> Seq.cast
            |> Doc.Concat
        member x.Render() = DescriptionTerm.Render x
    and DescriptionOrientation =
        | Horizontal
        | Vertical
    and Description =
        | Doc of Doc
        | Text of string
        | Empty

    /// Renders a navigation tab and the content associated with it.
    type NavTabs = {
        Tabs: NavTab list
        NavTabType: NavTabType
        IsJustified: bool
    } with
        static member Create() =
            { Tabs = []
              NavTabType = NavTabType.Normal
              IsJustified = false }
        /// Renders the menu of the tabs.
        static member RenderTabs x =
            ulAttr [ attr.``class`` ("nav "
                                     + (if x.IsJustified then "nav-justified " else "")
                                     + (match x.NavTabType with 
                                        | Normal -> "nav-tabs" 
                                        | Pill Horizontal -> "nav-pills"
                                        | Pill Vertical -> "nav-pills nav-stacked")) ]
                   (x.Tabs |> List.map NavTab.RenderTab |> Seq.cast)
        member x.RenderNav() = NavTabs.RenderTabs x
        /// Renders the content link to the tabs.
        static member RenderContent x =
            divAttr [ attr.``class`` "tab-content" ] (x.Tabs |> List.map NavTab.RenderContent |> Seq.cast)
        member x.RenderContent() = NavTabs.RenderContent x
        member x.WithTabs tabs = { x with Tabs = tabs }
        member x.WithType ty = { x with NavTabType = ty }
        member x.Justify isJustified = { x with IsJustified = isJustified }
    and NavTab = {
        Id: string
        Title: string
        Content: Doc
        State: NavTabState 
    } with
        static member Create id =
            { Id = id
              Title = ""
              Content = Doc.Empty
              State = NavTabState.Normal }
        static member RenderTab x =
            liAttr [ attr.role "presentation"
                     attr.``class`` (match x.State with
                                     | NavTabState.Normal -> ""
                                     | NavTabState.Active -> "active"
                                     | NavTabState.Disabled -> "disabled") ] 
                   [ (match x.State with
                      | NavTabState.Disabled -> Hyperlink.Create(Href "#", x.Title)
                      | _ ->  Hyperlink.Create(Href <| "#" + x.Id, x.Title)
                                       .WithRole("tab")
                                       .WithDataToggle("tab")
                      ).Render() ]
        member x.RenderTab() = NavTab.RenderTab x
        static member RenderContent x =
            divAttr [ attr.role "tabpanel"
                      attr.id x.Id
                      attr.``class`` (match x.State with NavTabState.Active -> "tab-pane fade in active" | _ -> "tab-content tab-pane fade") ]
                    [ x.Content ]
        member x.RenderContent() = NavTab.RenderContent x
        member x.WithContent doc = { x with Content = doc }
        member x.WithTitle title = { x with Title = title }
        member x.WithState state = { x with State = state }
    and NavTabState =
        | Normal
        | Active
        | Disabled
    and NavTabType =
        | Normal
        | Pill of PillStack
    and PillStack =
        | Horizontal
        | Vertical
        
    type Pagination = {
        Links: PaginationListItem list
    } with 
        static member Create links = { Links = links }
        static member Render x =
            nav [ ulAttr [ attr.``class`` "pagination" ] (x.Links |> List.map PaginationListItem.Render |> Seq.cast) ]
        member x.Render() = Pagination.Render x
    and PaginationListItem = {
        Link: Hyperlink
        PaginationListItemState: PaginationListItemState
    } with
        static member Create link =
            { Link = link
              PaginationListItemState = PaginationListItemState.Normal }
        static member Create(link, state) = 
            { Link = link
              PaginationListItemState = state }
        static member Render x =
            liAttr [ attr.``class`` (match x.PaginationListItemState with
                                     | Normal -> ""
                                     | Active -> "active"
                                     | Disabled -> "disabled") ]
                   [ x.Link.Render() ]
        member x.Render() = PaginationListItem.Render x
    and PaginationListItemState =
        | Normal
        | Active
        | Disabled

    type FormElement = {
        FormGroups: FormGroup list
        Buttons: Button list
        FormDisplay: FormDisplay
    } with
        static member Create(formGroups) =
            { FormGroups = formGroups
              Buttons = []
              FormDisplay = Normal }
        static member Create(formGroups, buttons) =
            { FormGroups = formGroups
              Buttons = buttons
              FormDisplay = Normal }
        static member Create(formDisplay, formGroups, buttons) =
            { FormGroups = formGroups
              Buttons = buttons
              FormDisplay = formDisplay }
        static member Render x =
            formAttr [ attr.``class`` (match x.FormDisplay with Normal -> "" | Horizontal -> "form-horizontal" | Inline -> "form-inline") ]
                     [ fieldset [ yield! x.FormGroups |> List.map (fun fg -> fg.Render x.FormDisplay) |> Seq.cast
                                  yield! x.Buttons |> List.map Button.Render |> Seq.cast ] ]
        member x.Render() = FormElement.Render x
        member x.WithButtons btns = { x with Buttons = btns }
        member x.WithDisplay display = { x with FormDisplay = display }
    and FormGroup = {
        Id: string
        Title: FormTitle
        Input: FormInput
    } with
        static member Create(id) =
            { Id = id
              Title = FormTitle.SrOnly ""
              Input = FormInput.Custom Doc.Empty }
        static member Create(id, title, input) =
            { Id = id
              Title = Text title
              Input = input }
        static member Create(id, title, input) =
            { Id = id
              Title = title
              Input = input }
        static member CreateTextInput(id, title, rv, err, placeholder, ?attrs) =
            { Id = id
              Title = title
              Input = FormInput.Text(rv, err, placeholder, defaultArg attrs []) }
        static member CreatePasswordInput(id, title, rv, err, placeholder, ?attrs) =
            { Id = id
              Title = title
              Input = FormInput.Password(rv, err, placeholder, defaultArg attrs []) }
        member x.WithTitle title = { x with Title = title }
        member x.WithTitle title = { x with Title = FormTitle.Text title }
        member x.WithInput input = { x with Input = input }
        static member Render(formDisplay, x) =
            let _label cls =
                match x.Title with
                | FormTitle.Text txt   -> labelAttr [ attr.``for`` x.Id; attr.``class`` cls ] [ text txt ]
                | SrOnly txt -> labelAttr [ attr.``for`` x.Id; attr.``class`` (cls + " sr-only") ] [ text txt ]

            let container elts =
                match formDisplay with
                | Horizontal -> 
                       match x.Input with 
                       | Checkbox _ -> [ divAttr [ attr.``class`` "col-sm-offset-2 col-sm-10" ] elts :> Doc ]
                       | _ -> [ _label "col-sm-2 control-label"; divAttr [ attr.``class`` "col-sm-10" ] elts :> Doc ]
                | Normal | Inline   ->
                       match x.Input with 
                       | Checkbox _ -> elts
                       | _ -> [ _label "" ] @ elts
            
            match x.Input with
            | FormInput.Text (rv, errors, placeholder, attrs) -> 
                divAttr [ attr.classDyn (errors |> View.Map (fun e -> if e <> [] then "form-group has-error" else "form-group")) ]
                        (container [ Doc.Input [ yield attr.``class`` "form-control"
                                                 yield attr.placeholder placeholder 
                                                 yield! attrs ] rv :> Doc
                                     errors 
                                     |> View.Map (List.map(fun e -> spanAttr [ attr.``class`` "help-block" ] [ text e ] :> Doc) >> Doc.Concat) 
                                     |> Doc.EmbedView ])
                
            | Password (rv, errors, placeholder, attrs) ->
                divAttr [ attr.classDyn (errors |> View.Map (function e -> if e <> [] then "form-group has-error" else "form-group")) ]
                        (container[ Doc.PasswordBox [ yield attr.``class`` "form-control"
                                                      yield attr.placeholder placeholder
                                                      yield! attrs ] rv :> Doc
                                    errors 
                                    |> View.Map (List.map(fun e -> spanAttr [ attr.``class`` "help-block" ] [ text e ] :> Doc) >> Doc.Concat) 
                                    |> Doc.EmbedView ])

            | Checkbox (rv, attrs) ->
                divAttr [ attr.``class`` "form-group" ]
                        (container [ divAttr [ yield attr.``class`` "checkbox" 
                                               yield! attrs ]
                                             [ label [ Doc.CheckBox [] rv
                                                       (match x.Title with FormTitle.Text txt -> text txt | _ -> Doc.Empty) ] ] ])
                
            | Static txt -> 
                divAttr [ attr.``class`` "form-group" ]
                        (container [ pAttr [ attr.``class`` "form-control-static" ] [ text txt ] ])
                
            | Custom doc -> 
                divAttr [ attr.``class`` "form-group" ]
                        (container [ doc ])

        member x.Render display = FormGroup.Render(display, x)
        /// Displays custom error messages registered with the customErrorId
        static member ShowErrors view =
            FormGroup.Create("errors")
                     .WithTitle(SrOnly "Errors after submit")
                     .WithInput(FormInput.Custom (Doc.ShowErrors (View.ThroughCustomId view) (function [] -> Doc.Empty | errors -> divAttr [ attr.``class`` "alert alert-warning"; attr.role "alert" ] (errors |> List.map(fun err -> p [ text err.Text ] :> Doc)) :> Doc)))
        /// Displays success message when submit is successful
        static member ShowSuccess message view =
            FormGroup.Create("success")
                     .WithTitle(SrOnly "Success message after submit")
                     .WithInput(FormInput.Custom (Doc.ShowSuccess view (function _ -> divAttr [ attr.``class`` "alert alert-success"; attr.role "alert" ] [ text message ] :> Doc)))
    and FormInput =
        | Text of ref: Var<string> 
                  * inlineErrors: View<string list> 
                  * placeholder: string 
                  * extraAttrs: List<Attr>
        | Password of ref: Var<string> 
                      * inlineErrors: View<string list> 
                      * placeholder: string 
                      * extraAttrs: List<Attr>
        | Checkbox of ref: Var<bool> * extraAttrs: List<Attr>
        | Static of string
        | Custom of Doc
    and FormTitle =
        | Text of string
        /// Creates a title visible by screen reader only
        | SrOnly of string
    and FormDisplay =
        | Normal
        | Horizontal
        | Inline