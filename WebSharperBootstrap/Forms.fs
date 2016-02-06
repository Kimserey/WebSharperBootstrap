namespace WebSharperBootstrap

open WebSharper
open WebSharper.JavaScript
open WebSharper.JQuery
open WebSharper.UI.Next
open WebSharper.UI.Next.Html
open WebSharper.UI.Next.Client
open WebSharper.Forms

[<JavaScript>]
module Forms =
    
    let errView (submitterView: View<Result<_>>) (rv: Var<_>) =
        View.Through(submitterView, rv)
        |> View.Map (function 
                     | Result.Success _   -> None 
                     | Result.Failure errs -> Some errs)
    
    let normal =
        Form.Return (fun input1 input2 checkbox  -> input1, input2, checkbox)
        <*> (Form.Yield "" |> Validation.IsNotEmpty "Input 1 is required")
        <*> (Form.YieldOption None "")
        <*> (Form.Yield true)
        |> Form.WithSubmit
        |> Form.Render (fun input1 input2 checkbox submitter -> 
            Bootstrap.FormElement.Create(
                [ Bootstrap.FormGroup.Create("input-1", "This is input 1", FormInput.Text (input1, View.ErrorMessages(submitter.View, input1), "abcd efg", []))
                  Bootstrap.FormGroup.Create("input-2", "This is input 2", FormInput.Text (input2, View.Const [], "abcd efg", []))
                  Bootstrap.FormGroup.Create("checkbox-1", "This is a checkbox", FormInput.Checkbox (checkbox, [])) ], 
                [ Button.Create("Send now!", ButtonStyle.Primary, submitter.Trigger).WithExtraAttrs([ attr.style "margin-bottom: 15px;" ]) ]
            ).Render())

    let inlineForm =
        Form.Return (fun input1 input2 checkbox  -> input1, input2, checkbox)
        <*> (Form.Yield "" |> Validation.IsNotEmpty "Input 1 is required")
        <*> (Form.YieldOption None "")
        <*> (Form.Yield true)
        |> Form.WithSubmit
        |> Form.Render (fun input1 input2 checkbox submitter -> 
            Bootstrap.FormElement.Create(
                Bootstrap.FormDisplay.Inline,
                [ Bootstrap.FormGroup.Create("input-1", "This is input 1", FormInput.Text (input1, View.ErrorMessages(submitter.View, input1), "abcd efg", []))
                  Bootstrap.FormGroup.Create("input-2", "This is input 2", FormInput.Text (input2, View.Const [], "abcd efg", []))
                  Bootstrap.FormGroup.Create("checkbox-1", "This is a checkbox", FormInput.Checkbox (checkbox, [])) ], 
                [ Button.Create("Send now!", ButtonStyle.Primary, submitter.Trigger) ]
            ).Render())

    let horizontal =
        Form.Return (fun input1 input2 checkbox  -> input1, input2, checkbox)
        <*> (Form.Yield "" |> Validation.IsNotEmpty "Input 1 is required")
        <*> (Form.YieldOption None "")
        <*> (Form.Yield true)
        |> Form.WithSubmit
        |> Form.Render (fun input1 input2 checkbox submitter -> 
            Bootstrap.FormElement.Create(
                Bootstrap.FormDisplay.Horizontal,
                [ Bootstrap.FormGroup.Create("input-1", "This is input 1", FormInput.Text (input1, View.ErrorMessages(submitter.View, input1), "abcd efg", []))
                  Bootstrap.FormGroup.Create("input-2", "This is input 2", FormInput.Text (input2, View.Const [], "abcd efg", []))
                  Bootstrap.FormGroup.Create("checkbox-1", "This is a checkbox", FormInput.Checkbox (checkbox, [])) ], 
                [ Button.Create("Send now!", ButtonStyle.Primary, submitter.Trigger) ]
            ).Render())
