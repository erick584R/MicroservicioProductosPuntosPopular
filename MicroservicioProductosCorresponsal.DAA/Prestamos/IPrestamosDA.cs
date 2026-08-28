using MicroservicioProductosCorresponsal.DTO.Prestamos;

namespace MicroservicioProductosCorresponsal.DAA.Prestamos
{
    public interface IPrestamosDA
    {
        Task<BPRespListaPrestamosDTO> ListaPrestamos(ReqListaPrestamosDTO reqCuentas);
    }
}
