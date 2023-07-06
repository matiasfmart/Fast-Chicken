using System;
namespace FastChicken.Models
{
	public class TotalJournal
	{
		public int OrdersCount { get; set; }
		public int Total { get; set; }
		public DateTime StartJournalDate { get; set; }
		public DateTime EndJournalDate { get; set; }
	}
}

