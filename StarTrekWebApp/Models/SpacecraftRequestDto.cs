using static StarTrekWebApp.Common.Enums;

namespace StarTrekWebApp.Models
{
    public class SpacecraftRequestDto
    {
        public Spacecraft Spacecraft { get; set; }
        public RequestActions Action { get; set; }
        public List<ErrorDto> Errors { get; set; }
    }
}
