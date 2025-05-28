using Microsoft.AspNetCore.Identity;

namespace kioscov1.Models.UserEntity
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Password { get; set; }
        public int RolId { get; set; }
        public Rol? Rol { get; set; }
    }
}
