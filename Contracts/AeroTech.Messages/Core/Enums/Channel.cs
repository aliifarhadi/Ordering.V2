using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Core.Enums
{
    public enum Channel
    {
        [Display(Name = "IBE")]
        IBE = 1,
        [Display(Name = "BackOffice")]
        BackOffice = 2,
        [Display(Name = "API")]
        API = 3,
        [Display(Name = "GUI")]
        GUI = 4,
        [Display(Name = "GDS")]
        GDS = 5,
        [Display(Name = "System")]
        System = 6,

    }
}
