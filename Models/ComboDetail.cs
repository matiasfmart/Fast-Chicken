namespace FastChicken.Models
{
    public class ComboDetail
    { 
        public ComboDetail() { this.idCombo = ""; this.Type = ""; }
        public int idComboDetail { get;set; }
        public string idCombo { get; set; }
        public string Type { get; set; }
        public int Quantity { get; set; }
        public int idDetail { get; set; }
    }
}
