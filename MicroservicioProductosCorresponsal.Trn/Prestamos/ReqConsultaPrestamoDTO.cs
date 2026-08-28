using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;

namespace MicroservicioProductosCorresponsal.DTO.Prestamos
{
    public class ReqConsultaPrestamoDTO
    {
        // Requiere canal en BpInReq.Canal (igual que Giros)
        public BpInReqDTO BPInReq { get; set; }

        // Principal: número/identificador del crédito (se solicita sólo Crédito, el resto puede venir nulo)
        public string? Credito { get; set; }

        // Opcionales (según ejemplo Bantotal)
        public string? Documento { get; set; }
        public int? TipoDocumento { get; set; }
        public int? Moneda { get; set; }
        public int? CantidadCuotas { get; set; }
        public string? Canal { get; set; }
        public string? Fecha { get; set; }
        public string? Hora { get; set; }
        public string? IdCorresponsal { get; set; }
        public string? IdTerminal { get; set; }
    }
}
