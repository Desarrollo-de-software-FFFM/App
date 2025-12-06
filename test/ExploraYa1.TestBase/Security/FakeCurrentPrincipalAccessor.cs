using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace ExploraYa1.CalificacionesTest
{
    [Dependency(ReplaceServices = true)]
    public class FakeCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
    {
        private ClaimsPrincipal _principal;

        /// <summary>
        /// Establece un ClaimsPrincipal explícito (usuario autenticado o no autenticado)
        /// </summary>
        public void SetPrincipal(ClaimsPrincipal principal)
        {
            _principal = principal;
        }

        /// <summary>
        /// Devuelve exactamente el principal seteado, o un principal no autenticado si es null
        /// </summary>
        protected override ClaimsPrincipal GetClaimsPrincipal()
        {
            // Si no hay principal seteado, devolver siempre usuario NO autenticado
            return _principal ?? new ClaimsPrincipal(new ClaimsIdentity());
        }
    }
}
