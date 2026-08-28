using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;

namespace MicroservicioProductosCorresponsal.DTO.Prestamos
{
    public class ReqListaPrestamosDTO
    {
        public BpInReqDTO BPInReq { get; set; }
        public string NumeroDocumento { get; set; }
    }
}
