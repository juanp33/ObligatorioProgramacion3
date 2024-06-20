using System.ComponentModel.DataAnnotations;

namespace ObligatorioProgramacion3.Models
{
    public class RegistroViewModel
    {



        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string NombreUsuario { get; set; }

        [Required(ErrorMessage = "El email de usuario es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string EmailUsuario { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        [DataType(DataType.Password)]
        public string Contraseña { get; set; }

        public int? RolId { get; set; }

        // Datos de Cliente
        [Required(ErrorMessage = "El nombre del cliente es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "El email del cliente es obligatorio.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido.")]
        public string EmailCliente { get; set; }

        [Required(ErrorMessage = "El tipo de cliente es obligatorio.")]
        public string TipoCliente { get; set; }
    }
}

