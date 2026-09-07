using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Core.Enums;

public enum LegalEntityKind
{
    [Display(Name = "Airline", Description = "Airline")]
    Airline = 1,

    [Display(Name = "Subsidiary", Description = "Subsidiary")]
    Subsidiary = 2,

    [Display(Name = "GeneralSalesAgent", Description = "General Sales Agent")]
    GeneralSalesAgent = 3,

    [Display(Name = "GroundHandler", Description = "Ground Handler")]
    GroundHandler = 4,

    [Display(Name = "Other", Description = "Other")]
    Other = 5
}
