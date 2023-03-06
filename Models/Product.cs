namespace FastChicken.Models
{
    public class Product
    {
        public Product() { this.Name = ""; }
        public int idProduct { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
    }
}
