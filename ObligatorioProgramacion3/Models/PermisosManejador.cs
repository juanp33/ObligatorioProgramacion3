
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
                return;
            }

            var user = httpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                return;
            }

            var userClaims = user.Claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
            if (userClaims.Contains(requisito.NombrePagina))
            {
                context.Succeed(requisito);
                return;
            }
        }
    }
}