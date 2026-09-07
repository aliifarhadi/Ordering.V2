using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Core.Enums;

public enum CorporateCustomerType
{
   

    [Display(Name = "TravelAgency")]
    TravelAgency = 1,

    [Display(Name = "Organization")]
    Organization = 2,

}