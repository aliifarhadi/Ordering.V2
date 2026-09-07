using System.ComponentModel.DataAnnotations;

namespace AeroTech.Messages.Core.Enums;

public enum ActivationStatus
{

    [Display(Description = "Active")]Active = 1,
    [Display(Description = "DeActive")] DeActive = 2,
}