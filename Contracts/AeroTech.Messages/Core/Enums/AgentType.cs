using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Core.Enums
{
    public enum AgentType
    {
        [Display(Description = "Travel Agent")] TravelAgent =1,
        [Display(Description = "Airline")] Airline =2,

    }
}
