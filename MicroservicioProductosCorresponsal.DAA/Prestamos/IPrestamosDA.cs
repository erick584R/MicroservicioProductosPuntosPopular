using MicroservicioProductosCorresponsal.DTO.Prestamos;

namespace MicroservicioProductosCorresponsal.DAA.Prestamos
{
    public interface IPrestamosDA
    {
        Task<BPRespListaPrestamosDTO> ListaPrestamos(ReqListaPrestamosDTO reqCuentas);
        public string FormatearProductoP_DPF_BP(string idOperacionBT);
        Task<ConsultaCuotasBTResponseDTO> ConsultarDetallesPrestamoDA(ReqConsultaPrestamoDTO request);
        Task<string> ObtenerCuentaEquivalenteAsync(string cuentaNormal);
    }
}
