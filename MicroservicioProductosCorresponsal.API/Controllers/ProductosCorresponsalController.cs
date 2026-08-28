using MicroservicioProductosCorresponsal.APP.CuentasAPP;
using MicroservicioProductosCorresponsal.DTO.Cuentas;
using MicroservicioProductosCorresponsal.DTO.Prestamos;
using MicroservicioProductosCorresponsal.APP.Prestamos;
using Microsoft.AspNetCore.Mvc;


namespace MicroservicioProductosCorresponsal.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosCorresponsalController : Controller
    {
        private readonly ICuentasAPP cuentasApp;
        private readonly IPrestamosAPP _prestamosApp;
        //private readonly ITasaCambioApp tasaCambioApp;

        public ProductosCorresponsalController(ICuentasAPP cuentasApp, IPrestamosAPP prestamosApp)
        {
            this.cuentasApp = cuentasApp;
            this._prestamosApp = prestamosApp;
        }



        [HttpPost("v1/BancoPopular/obtener-datos-cuenta-destino")]
        public async Task<ResponseInfoCuentaDTO> ObtenerdatosDestino(RequestInfoCuentaDTO request) => await cuentasApp.ObtenerDatosCuentaDestino(request);


        [HttpPost("v1/BancoPopular/obtener-lista-prestamos")]
        public async Task<ResponseBPListaPrestamosDTO> ObtenerListaPrestamos(ReqListaPrestamosDTO reqCuentasAhorroDTO) => await 
            _prestamosApp.ObtenerPrestamos(reqCuentasAhorroDTO);


        [HttpPost("v1/BancoPopular/Consultar-Prestamo")]
        public async Task<ConsultaCuotasBTResponseDTO> ConsultarDetallesPrestamos(ReqConsultaPrestamoDTO request)
            => await _prestamosApp.ConsultarDetallesPrestamoAPP(request);
    }
}
