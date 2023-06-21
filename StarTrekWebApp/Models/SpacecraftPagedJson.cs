namespace StarTrekWebApp.Models
{
    public class SpacecraftPagedJson
    {
        public int total { get; set; }
        public int filtered { get; set; }
        public List<SpacecraftJson> results { get; set; }
    }
}
