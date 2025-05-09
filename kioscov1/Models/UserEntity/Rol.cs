namespace kioscov1.Models.UserEntity
{
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<Usuario> Usuarios { get; set; }
        public Rol ()
        {
            Usuarios = new List<Usuario> ();
        }
    }
}
