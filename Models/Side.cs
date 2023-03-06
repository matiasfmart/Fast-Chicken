namespace FastChicken.Models
{
    public class Side
    {
        public Side() { this.Name = ""; }
        public int idSide { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
