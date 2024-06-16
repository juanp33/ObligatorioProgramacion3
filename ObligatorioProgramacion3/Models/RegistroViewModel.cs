using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models
{
    public class RegistroViewModel
    {


        
        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El email de usuario es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string EmailUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        public int? RolId { get; set; }

        // Datos de Cliente
        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El email del cliente es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string EmailCliente { get; set; }

        [Required(ErrorMessage = "El tipo de cliente es obligatorio.")]
        public string TipoCliente { get; set; }
    }
}

