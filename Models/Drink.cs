namespace FastChicken.Models
{
    public class Drink
    {
        public Drink() { this.Name = ""; }
        public int idDrink { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public double Quantity { get; set; }

    }

    public class Drinks
    {
        public List<Drink> drinks { get; set; }
    }
}
