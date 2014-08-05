namespace TablePartsFS
open System
open UIKit
open Foundation
open System.Collections.Generic

[<AllowNullLiteral>]
type HomeScreen =
    inherit UITableViewController
    new () = {inherit UITableViewController (UITableViewStyle.Grouped)}
    new (handle:IntPtr) = {inherit UITableViewController (handle)}
    [<Export ("initWithCoder:")>]
    new (coder:NSCoder) = {inherit UITableViewController (coder)}

    // Creates a set of table items.
    member x.CreateTableItems () =
        new TableSource [|{Name = "Section 0 Header"; Footer = "Section 0 Footer"; Items = ["Row 0";"Row 1";"Row 2"]}
                          {Name = "Section 1 Header"; Footer = "Section 1 Footer"; Items = ["Row 0";"Row 1";"Row 2"]}
                          {Name = "Section 2 Header"; Footer = "Section 2 Footer"; Items = ["Row 0";"Row 1";"Row 2"]}|]

    override x.ViewDidLoad () =
        base.ViewDidLoad ()
        x.TableView.Source <- x.CreateTableItems ()