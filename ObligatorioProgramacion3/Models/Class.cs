
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    namespace ObligatorioProgramacion3.Models
    {
        public class PermisoPaginaRequisito : IAuthorizationRequirement
        {
            public string NombrePagina { get; }

            public PermisoPaginaRequisito(string nombrePagina)
            {
                NombrePagina = nombrePagina;
            }
        }

        public class PermisoPaginaManejador : AuthorizationHandler<PermisoPaginaRequisito>
        {
            private readonly ObligatorioProgramacion3Context _context;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public PermisoPaginaManejador(ObligatorioProgramacion3Context context, IHttpContextAccessor httpContextAccessor)
            {
                _context = context;
                _httpContextAccessor = httpContextAccessor;
            }

            protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermisoPaginaRequisito requisito)
            {
                var httpContext = _httpContextAccessor.HttpContext;
                if (httpContext == null)
                {
                    // No hay contexto HTTP, no permitir acceso
                    return;
                }

                var user = httpContext.User;
                if (user == null || !user.Identity.IsAuthenticated)
                {
                    // Usuario no autenticado, no permitir acceso
                    return;
                }

                var claim = user.FindFirst(ClaimTypes.NameIdentifier);
                if (claim == null)
                {
                    // No se encuentra el claim del identificador de usuario
                    return;
                }

                if (!int.TryParse(claim.Value, out var usuarioId))
                {
                    // El valor del claim no se puede convertir a un entero
                    return;
                }

                var rolesUsuario = await _context.UsuarioRoles
                    .Where(ur => ur.UsuarioId == usuarioId)
                    .Select(ur => ur.RolId)
                    .ToListAsync();

                var tienePermiso = await _context.RolPermisos
                    .Include(rp => rp.Permiso)
                    .AnyAsync(rp => rolesUsuario.Contains(rp.RolId) && rp.Permiso.Nombre == requisito.NombrePagina);

                if (tienePermiso)
                {
                    context.Succeed(requisito);
                }
            }
        }
    }


