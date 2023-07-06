using System;
namespace FastChicken.Models
{
	public class OrderItem
	{
        public string ComboId { get; set; }
        public string OrderItemId { get; set; }
		public int OrderId { get; set; }
        public string Name { get; set; }
		public string Price { get; set; }
        public string Side { get; set; }
		public int idSide { get; set; }
		public string Drink { get; set; }
		public int idDrink { get; set; }
		public bool Ice { get; set; }
		public string Type { get; set; }
        public bool Spicy { get; set; }
        public int Count { get; set; }
	}
}