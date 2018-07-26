using System;
using System.Collections.Generic;
using UIKit;

namespace GrowRowTable
{
	public class GrowRowTableDataSource : UITableViewDataSource
	{
		public List<GrowItem> Items = new List<GrowItem>();

		public string CellID {
			get { return "GrowCell"; }
		}

		public GrowRowTableDataSource ()
		{
			// Initialize
			Initialize();
		}

		private void Initialize() {

			// Populate database
			Items.Add(new GrowItem("Macintosh_128k.png","Macintosh 128K","The Macintosh 128K, originally released as the Apple Macintosh, is the original Apple Macintosh personal computer. Its beige case consisted of a 9 in (23 cm) CRT monitor and came with a keyboard and mouse. A handle built into the top of the case made it easier for the computer to be lifted and carried. It had an initial selling price of $2,495 (equivalent to $5,683 in 2015)."));
			Items.Add(new GrowItem("Macintosh_512K.png","Macintosh 512K","The Macintosh 512K Personal Computer is the second of a long line of Apple Macintosh computers, and was the first update to the original Macintosh 128K. It was virtually identical to the previous Mac, differing primarily in the amount of built-in memory (RAM)."));
			Items.Add(new GrowItem("Macintosh_Plus.jpg","Macintosh Plus","The Macintosh Plus computer is the third model in the Macintosh line, introduced on January 16, 1986, two years after the original Macintosh and a little more than a year after the Macintosh 512K, with a price tag of US$2599. As an evolutionary improvement over the 512K, it shipped with 1 MB of RAM standard, expandable to 4 MB, and an external SCSI peripheral bus, among smaller improvements."));
			Items.Add(new GrowItem("Macintosh_SE.jpg","Macintosh SE","The Macintosh SE is a personal computer manufactured by Apple between March 1987 and October 1990. This computer marked a significant improvement on the Macintosh Plus design and was introduced by Apple at the same time as the Macintosh II. It had a similar case to the original Macintosh computer, but with slight differences in color and styling."));
			Items.Add(new GrowItem("MacII.jpg","Macintosh II","The Apple Macintosh II is the first personal computer model of the Macintosh II series in the Apple Macintosh line and the first Macintosh to support a color display. A basic system with 20 MB drive and monitor cost about $5500, A complete color-capable system could cost as much as $10,000 once the cost of the color monitor, video card, hard disk, keyboard and RAM were added. "));
			Items.Add(new GrowItem("SE30.jpg","Macintosh SE/30","The Macintosh SE/30 is a personal computer that was designed, manufactured and sold by Apple Computer, Inc. from 1989 until 1991. It was the fastest of the original black-and-white compact Macintosh series."));
			Items.Add(new GrowItem("Macintosh_Portable.jpg","Macintosh Portable","The Macintosh Portable was Apple Inc.'s first battery-powered portable Macintosh personal computer. Released on September 20, 1989, it was received with excitement from most critics but consumer sales were quite low. It featured a fast, sharp, and expensive black and white active matrix LCD screen in a hinged design that covered the keyboard when the machine was not in use."));
			Items.Add(new GrowItem("Macintosh_Classic.jpg","Macintosh Classic","The Macintosh Classic is a personal computer manufactured by Apple Inc. Introduced on October 15, 1990, it was the first Apple Macintosh to sell for less than US$1,000. Production of the Classic was prompted by the success of the Macintosh Plus and the Macintosh SE. The system specifications of the Classic were very similar to its predecessors, with the same 9-inch (23 cm) monochrome CRT display, 512×342 pixel resolution, and 4 megabyte (MB) memory limit of the older Macintosh computers."));
			Items.Add(new GrowItem("Macintosh_LC.jpg","Macintosh LC","The Macintosh LC (meaning low-cost color) is Apple Computer's product family of low-end consumer Macintosh personal computers in the early 1990s. The original Macintosh LC was released in October 1990 and was the first affordable color-capable Macintosh. Due to its affordability and Apple II compatibility the LC was adopted primarily in the education and home markets."));
			Items.Add(new GrowItem("Powerbook_150.jpg","Powerbook","The PowerBook (known as Macintosh PowerBook before 1997) is a line of Macintosh laptop computers that was designed, manufactured and sold by Apple Computer, Inc. from 1991 to 2006. During its lifetime, the PowerBook went through several major revisions and redesigns, often being the first to incorporate features that would later become standard in competing laptops."));
			Items.Add(new GrowItem("Macintosh_Classic_2.jpg","Macintosh Classic II","The Apple Macintosh Classic II (also known as the Performa 200) replaced the Macintosh SE/30 in the compact Macintosh line in 1991. Like the SE/30, the Classic II was powered by a 16 MHz Motorola 68030 CPU and 40 or 80 MB hard disk, but in contrast to its predecessor, it was limited by a 16-bit data bus (the SE/30 had a 32-bit data bus) and a 10 MB memory ceiling."));
			Items.Add(new GrowItem("Macintosh_Color_Classic.jpg","Macintosh Color Classic","The Macintosh Color Classic, released on February 10, 1993, is the first color compact Apple Macintosh computer. It has an integrated 10″ Sony Trinitron color display with the same 512×384 pixel resolution as the Macintosh 12″ RGB monitor. It can display 256 colors(Can upgrade to 4096 colors). "));
			Items.Add(new GrowItem("Power_Macintosh.jpg","Power Macintosh","Power Macintosh, later Power Mac, is a line of Apple Macintosh workstation-class personal computers based on various models of PowerPC microprocessors that were developed, marketed, and supported by Apple Inc. from March 1994 until August 2006. "));
		}

		public override nint NumberOfSections (UITableView tableView)
		{
			// Hard coded 1 section
			return 1;
		}

		public override nint RowsInSection (UITableView tableView, nint section)
		{
			return Items.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, Foundation.NSIndexPath indexPath)
		{
			var cell = tableView.DequeueReusableCell (CellID, indexPath) as GrowRowTableCell;
			var item = Items [indexPath.Row];

			// Setup
			cell.Image = UIImage.FromFile(item.ImageName);
			cell.Title = item.Title;
			cell.Description = item.Description;

			return cell;
		}
	}
}

