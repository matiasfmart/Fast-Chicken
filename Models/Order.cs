namespace FastChicken.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrderNum { get; set; }
        public List<Combo> Combos { get; set; }
        public float Price { get; set; }
        public DateTime Date { get; set; }

        public Order()
        {
            this.Combos = new List<Combo>();
        }
    }
}
