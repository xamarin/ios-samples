using Foundation;
using System;
using UIKit;
using System.Linq;

namespace largetitles
{
    public partial class LargeTitlesViewController : UITableViewController, IUITableViewDelegate, IUITableViewDataSource, IUISearchResultsUpdating
    {
        public LargeTitlesViewController (IntPtr handle) : base (handle)
        {
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return searchResults == null ? titles.Length : searchResults.Length;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell("titlecell");
            cell.TextLabel.Text = searchResults == null ? titles[indexPath.Row] : searchResults[indexPath.Row];
            cell.DetailTextLabel.Text = "";
            return cell;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            TableView.DeselectRow(indexPath, true);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            var dvc = segue.DestinationViewController as DetailViewController;
            var indexPath = TableView.IndexPathForCell(sender as UITableViewCell);
            var item = searchResults == null ? titles[indexPath.Row] : searchResults[indexPath.Row];
            dvc.SetTitle(item);
        }

        string[] titles = new string[] {
             "The Ocean at the End of the Lane" 
            ,"So Long, and Thanks for All the Fish"
            ,"Twenty Thousand Leagues Under the Sea"
            ,"Rosencrantz and Guildenstern Are Dead"
            ,"A Portrait of the Artist as a Young Man"
            ,"Midnight in the Garden of Good and Evil"
            ,"Miss Peregrine’s Home for Peculiar Children"
            ,"Fried Green Tomatoes at the Whistle Stop Cafe"
            ,"World War Z: An Oral History of the Zombie War"
            ,"The Curious Incident of the Dog in the Night-Time"
            ,"His Last Bow: Some Reminiscences of Sherlock Holmes"
            ,"Zen and the Art of Motorcycle Maintenance: An Inquiry Into Values"
            ,"The Princess Bride: S. Morgenstern's Classic Tale of True Love and High Adventure"
            ,"Mr. William Shakespeares Comedies, Histories, and Tragedies: A Facsimile of the First Folio, 1623"
            ,"Longitude: The True Story of a Lone Genius Who Solved the Greatest Scientific Problem of His Time"
            ,"The Persecution and Assassination of Jean-Paul Marat as Performed by the Inmates of the Asylum of Charenton Under the Direction of the Marquis de Sade "
            ,"Don't Get Too Comfortable: The Indignities of Coach Class, The Torments of Low Thread Count, The Never-Ending Quest for Artisanal Olive Oil, and Other First World Problems "
        };
    }
}