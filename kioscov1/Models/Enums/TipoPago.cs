using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace kioscov1.Models.Enums
{
    public enum TipoPago
    {
        [Display(Name = "Efectivo")]
        Efectivo = 1,
        [Display(Name = "Transferencia")]
        Transferencia = 2,
    }
}
