using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Core.Enums;

public enum TenantStatus
{
    [Display(Name = "Draft")]
    Draft = 1,

    [Display(Name = "Provisioning")]
    Provisioning = 2,

    [Display(Name = "Active")]
    Active = 3,

    [Display(Name = "Suspended")]
    Suspended = 3,

    [Display(Name = "Deactivating")]
    Deactivating = 3,

    [Display(Name = "Deactivated")]
    Deactivated = 3
}