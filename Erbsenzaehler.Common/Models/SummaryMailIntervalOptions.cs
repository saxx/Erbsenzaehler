using System.ComponentModel.DataAnnotations;

namespace Erbsenzaehler.Models
{
    public enum SummaryMailIntervalOptions
    {
        [Display(Name = "Deaktivieren")]
        Disable,
        [Display(Name = "Täglich")]
        Daily,
        [Display(Name = "Wöchentlich")]
        Weekly,
        [Display(Name = "Monatlich")]
        Monthly
    }
}