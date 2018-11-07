﻿using System;
using System.Collections.Generic;
using Foundation;
using HttpClient.Core;
using UIKit;

namespace HttpClient
{
    public partial class ViewController : UITableViewController
    {
        private const string SegueIdentidfier = "showRequestDetails";
        private const string CellIdentidfier = "cell_identifier";

        private readonly List<RequestConfiguration> configurations = new List<RequestConfiguration>
        {
            new RequestConfiguration { Type = RequestType.WebRequestHttp, Title = "http  - WebRequest" },
            new RequestConfiguration { Type = RequestType.WebRequestHttps, Title = "https - WebRequest" },
            new RequestConfiguration { Type = RequestType.NSUrlConnectionHttp, Title = "http  - NSUrlConnection" },
            new RequestConfiguration { Type = RequestType.HttpClientHttp, Title = "http  - HttpClient" },
            new RequestConfiguration { Type = RequestType.HttpClientHttps, Title = "https - HttpClient" },
        };

        protected ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            TableView.SelectRow(NSIndexPath.FromRowSection(0, 0), false, UITableViewScrollPosition.Top);
        }

        public override void PrepareForSegue(UIStoryboardSegue segue, NSObject sender)
        {
            if(!string.IsNullOrEmpty(segue?.Identifier) && segue.Identifier == SegueIdentidfier)
            {
                if (segue.DestinationViewController is ResponseViewController responseViewController)
                {
                    NetworkProvider provider = null;
                    var selectedConfiguration = configurations[TableView.IndexPathForSelectedRow.Row];
                    switch (selectedConfiguration.Type)
                    {
                        case RequestType.WebRequestHttp:
                            provider = new DotNet(false);
                            break;
                        case RequestType.WebRequestHttps:
                            provider = new DotNet(true);
                            break;
                        case RequestType.NSUrlConnectionHttp:
                            provider = new Cocoa();
                            break;
                        case RequestType.HttpClientHttp:
                            provider = new NetHttp(false);
                            break;
                        case RequestType.HttpClientHttps:
                            provider = new NetHttp(true);
                            break;
                    }

                    responseViewController.Provider = provider;
                }
            }
        }

        public override nint RowsInSection(UITableView tableView, nint section)
        {
            return configurations.Count;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(CellIdentidfier, indexPath);

            var configuration = configurations[indexPath.Row];
            cell.TextLabel.Text = configuration.Title;

            return cell;
        }

        partial void RunHttpRequest(UIButton sender)
        {
            if(TableView.IndexPathForSelectedRow != null)
            {
                this.PerformSegue(SegueIdentidfier, this);
            }
        }
    }

    class RequestConfiguration
    {
        public string Title { get; set; }

        public RequestType Type { get; set; }
    }

    enum RequestType
    {
        WebRequestHttp,
        WebRequestHttps,
        NSUrlConnectionHttp,
        HttpClientHttp,
        HttpClientHttps,
    }
}