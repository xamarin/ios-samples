namespace TablePartsFS
open System
open System.Linq
open UIKit
open System.Collections.Generic
open Foundation

open Conversion

type TableItemGroup = {Name : string; Footer : string; Items : string list}

type TableSource(tableItems: TableItemGroup array) =
   inherit UITableViewSource()

   let cellIdentifier = "TableCell"

   /// Called by the TableView to determine how many sections(groups) there are.
   override x.NumberOfSections (tableView) =
        implicit tableItems.Length

   /// Called by the TableView to determine how many cells to create for that particular section.
   override x.RowsInSection (tableview, section) =
       nint (tableItems.[int section].Items.Count())
      

   /// Called by the TableView to retrieve the header text for the particular section(group)
   override x.TitleForHeader (tableView, section) =
       let title = tableItems.[int section].Name
       title

   /// Called by the TableView to retrieve the footer text for the particular section(group)
   override x.TitleForFooter (tableView, section) =
       tableItems.[int section].Footer  

   override x.RowSelected (tableView, indexPath) =
       (new UIAlertView ("Row Selected", tableItems.[int indexPath.Section].Items.[int indexPath.Row], null, "OK", null)).Show ()
    
   override x.RowDeselected (tableView, indexPath) =
       printfn "Row %s deselected" <| indexPath.Row.ToString()

   /// Called by the TableView to get the actual UITableViewCell to render for the particular section and row
   override x.GetCell (tableView, indexPath) =
       printfn "Calling Get Cell, isEditing:%b" tableView.Editing
        
       // declare vars
       let cell =
           match tableView.DequeueReusableCell (cellIdentifier) with
           // if there are no cells to reuse, create a new one
           | null -> new UITableViewCell (UITableViewCellStyle.Default, cellIdentifier)
           | a -> a
       
       // set the item text
       cell.TextLabel.Text <- tableItems.[int indexPath.Section].Items.[int indexPath.Row]
       cell