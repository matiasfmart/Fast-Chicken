namespace FastChicken.Models
{
    public class Combo
    {
        public Combo() { this.ComboId = ""; this.Name = ""; this.Description = ""; this.Price = ""; this.Type = ""; }
        public string ComboId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string Type { get; set; }
        public IList<ComboDetail>? Drinks { get; set; }
        public IList<ComboDetail>? Sides { get; set; }
        public IList<ComboDetail>? Products { get; set; }

    }
}