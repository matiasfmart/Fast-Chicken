namespace FastChicken.Models
{
    public class ResultModel
    {
        public ResultModel() { this.Status = ""; this.Message = ""; }
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
