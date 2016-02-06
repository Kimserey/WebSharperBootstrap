namespace WebSharperBootstrap

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client

[<JavaScript>]
module Client =
    let Main =
        
        let ``fork me on github`` =
            aAttr [ attr.href "https://github.com/Kimserey/WebSharperBootstrap" ]
                  [ imgAttr [ attr.style "position: absolute; top: 0; right: 0; border: 0;"
                              attr.src "https://camo.githubusercontent.com/365986a132ccd6a44c23a9169022c0b5c890c387/68747470733a2f2f73332e616d617a6f6e6177732e636f6d2f6769746875622f726962626f6e732f666f726b6d655f72696768745f7265645f6161303030302e706e67"
                              attr.alt "Fork me on GitHub"
                              attr.``data-`` "canonical-src" "https://s3.amazonaws.com/github/ribbons/forkme_right_red_aa0000.png" ] [] ]
        
        let button =
            Button.Create("Create a new user", ButtonStyle.Primary, fun () -> JS.Alert "Yay!")

        let hyperlink =
            Hyperlink.Create(Href "#", "Hello world link")

        let rowButtons =
            GridRow.Create(
                [ GridColumn.Create(button.Render(), [ GridColumnSize.ColSm6 ])
                  GridColumn.Create(hyperlink.Render(), [ GridColumnSize.ColSm6 ]) ])

        let breadcrumbBar =
            BreadcrumbBar.Create(
                [ Breadcrumb.Create("Home", "#")
                  Breadcrumb.Create("Navigation 1", "#")
                            .WithDecoration(Icon "fa-user")
                  Breadcrumb.Create("Navigation 2", "#")
                            .WithDecoration(Image "https://placeholdit.imgix.net/~text?txtsize=5&txt=25%C3%9725&w=25&h=25&txtpad=1")
                  Breadcrumb.CreateActive("Navigation 3") ])
        
        let rowBreadcrumbWithButton =
            let _b = 
                button.WithCssClass("pull-right")

            GridRow.Create(
                [ GridColumn.Create(breadcrumbBar.Render(), [ GridColumnSize.ColXs12; GridColumnSize.ColSm8 ])
                  GridColumn.Create(_b.Render(), [ GridColumnSize.ColXs12; GridColumnSize.ColSm4 ]) ])

        let descriptionList =
            DescriptionList.Create(
                [ DescriptionTerm.Create("Description", "Something cool is happening.")
                  DescriptionTerm.Create("Computer", "Super super super computer.")
                  DescriptionTerm.Create("Custom div", p [ strong [ text "Hehe " ]
                                                           small [ text "hoho" ] ]) ],
                DescriptionOrientation.Horizontal)
    
        let tabs =
            NavTabs.Create()
                   .Justify(true)
                   .WithType(Pill PillStack.Horizontal)
                   .WithTabs(
                        [ NavTab.Create("home")
                                .WithTitle("Home")
                                .WithContent(divAttr [ attr.``class`` "well" ] [ text "Home page here." ])
                                .WithState(NavTabState.Active)

                          NavTab.Create("account")
                                .WithTitle("Account")
                                .WithContent(divAttr [ attr.``class`` "well" ] [ text "Home page here." ])
                  
                          NavTab.Create("profile")
                                .WithTitle("Profile")
                                .WithContent(divAttr [ attr.``class`` "well" ] [ text "Profile page here." ])

                          NavTab.Create("hello")
                                .WithTitle("Hello")
                                .WithState(NavTabState.Disabled) ])

        let pagination =
            Pagination.Create(
                [ PaginationListItem.Create(Hyperlink.Create(Href "#", "Previous"), PaginationListItemState.Disabled)
                  PaginationListItem.Create(Hyperlink.Create(Href"#", "1"), PaginationListItemState.Active)
                  PaginationListItem.Create(Hyperlink.Create(Href"#", "2"))
                  PaginationListItem.Create(Hyperlink.Create(Href "#", "3"))
                  PaginationListItem.Create(Hyperlink.Create(Href "#", "Next")) ])

        [ ``fork me on github``
          Container.Create(
            [ rowButtons.Render()
              breadcrumbBar.Render()
              rowBreadcrumbWithButton.Render()
              descriptionList.Render()
              tabs.RenderTabs()
              tabs.RenderContent()
              pagination.Render()
              Forms.normal
              Forms.horizontal
              Forms.inlineForm ], ContainerType.Normal)
          |> Container.Render ]
        |> Seq.cast
        |> Doc.Concat
        |> Doc.RunById "main"
