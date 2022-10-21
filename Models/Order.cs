namespace FastChicken.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public float Price { get; set; }
        public DateTime Date { get; set; }

    }
}
