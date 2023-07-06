namespace FastChicken.Models
{
    public class Order
    {
        public Order()
        {
            this.Items = new List<OrderItem>();
        }
        public int Total { get; set; }
        public DateTime Date { get; set; }
        public bool HereToGo { get; set; } //no esta en la base
        public int OrderNum { get; set; }
        public List<OrderItem> Items { get; set; }
    }
}