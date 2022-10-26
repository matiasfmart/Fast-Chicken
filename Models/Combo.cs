namespace FastChicken.Models
{
    public class Combo
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public float Price { get; set; }
        public string Description { get; set; }
        public Product Main { get; set; }
        public Product Side { get; set; }
        public Product Drink { get; set; }
    }
}
