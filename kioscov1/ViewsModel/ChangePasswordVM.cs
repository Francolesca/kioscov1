using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace kioscov1.ViewsModel
{
    public class ChangePasswordVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage= "Contraseña actual requerida")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        public string PasswordAct {  get; set; }
        [Required(ErrorMessage = "Nueva contraseña requerida")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Debe tener al menos {2} caracteres")]
        [Display(Name = "Contraseña nueva")]
        public string PasswordNew { get; set; }
        [DataType(DataType.Password)]
        [Compare("PasswordNew", ErrorMessage = "Las contraseñas no coinciden.")]
        public string PasswordConfirm { get; set; }

    }
}
