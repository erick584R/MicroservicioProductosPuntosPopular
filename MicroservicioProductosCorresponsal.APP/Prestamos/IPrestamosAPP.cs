using MicroservicioProductosCorresponsal.DTO.Cuentas;
using MicroservicioProductosCorresponsal.DTO.Prestamos;

namespace MicroservicioProductosCorresponsal.APP.Prestamos
{
    public interface IPrestamosAPP
    {
        Task<ResponseBPListaPrestamosDTO> ObtenerPrestamos(ReqListaPrestamosDTO reqCuentasAhorroDTO);
        Task<ConsultaCuotasBTResponseDTO> ConsultarDetallesPrestamoAPP(ReqConsultaPrestamoDTO request);
    }
}
