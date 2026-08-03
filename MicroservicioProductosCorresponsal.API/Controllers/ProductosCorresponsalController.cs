using Microsoft.AspNetCore.Mvc;
using MicroservicioProductosCorresponsal.DTO.Cuentas;
using MicroservicioProductosCorresponsal.APP.CuentasAPP;


namespace MicroservicioProductosCorresponsal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosCorresponsalController : Controller
    {
        private readonly ICuentasAPP cuentasApp;
        //private readonly ITasaCambioApp tasaCambioApp;

        public ProductosCorresponsalController(ICuentasAPP cuentasApp)
        {
            this.cuentasApp = cuentasApp;
        }



        [HttpPost("v1/BancoPopular/obtener-datos-cuenta-destino")]
        public async Task<ResponseInfoCuentaDTO> ObtenerdatosDestino(RequestInfoCuentaDTO request) => await cuentasApp.ObtenerDatosCuentaDestino(request);
    }
}
