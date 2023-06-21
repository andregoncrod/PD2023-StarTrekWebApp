namespace StarTrekWebApp.Models
{
    public class Spacecraft
    {
        public string Uid { get; set; }
        public string Name { get; set; }
        public string Registry { get; set; }
        public string Status { get; set; }
        public string DateStatus { get; set; }
        public DateTime SystemDate { get; set; }
        public DateTime? LastChange { get; set; }
    }
}
