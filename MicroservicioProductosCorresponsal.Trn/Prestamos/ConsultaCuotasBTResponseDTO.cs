using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;

namespace MicroservicioProductosCorresponsal.DTO.Prestamos
{
    public class ConsultaCuotasBTResponseDTO
    {
        public BtinreqDTO Btinreq { get; set; }

        public string? Canal { get; set; }
        public int? TipoDocumento { get; set; }
        public string? Documento { get; set; }
        public string? Credito { get; set; }
        public string? Fecha { get; set; }
        public string? Hora { get; set; }
        public string? IdCorresponsal { get; set; }
        public string? IdTerminal { get; set; }
        public int? CantidadCuotas { get; set; }
        public string? NombreCliente { get; set; }
        public int? Cuenta { get; set; }
        public int? Moneda { get; set; }
        public int? ClienteUId { get; set; }
        //public int? OperacionUid_Clioente { get; set; }

        public ListaCreditosWrapperDTO? ListaCreditos { get; set; }

        public int? IdBantotal { get; set; }
        public string? Resultado { get; set; }
        public string? Descripcion { get; set; }

        public object? Erroresnegocio { get; set; }
        public BPOutReqDTO? BpOutReq { get; set; }
        public BtoutreqDTO Btoutreq { get; set; }
    }

    public class ListaCreditosWrapperDTO
    {
        public List<ListaCreditosItemDTO>? ListaCreditos { get; set; }
    }

    public class ListaCreditosItemDTO
    {
        public int? NroCredito { get; set; }
        public int? OperacionUID { get; set; }
        public ListaCuotasWrapperDTO? ListaCuotas { get; set; }
        public decimal? MontoCancTotal { get; set; }
        public int? MonedaCred { get; set; }
    }

    public class ListaCuotasWrapperDTO
    {
        // En el ejemplo viene SdtBBTADetalleCuota como arreglo
        public List<SdtBBTADetalleCuotaDTO>? SdtBBTADetalleCuota { get; set; }
    }

    public class SdtBBTADetalleCuotaDTO
    {
        public int? NumCuo { get; set; }
        public string? FecVec { get; set; }
        public string? Tipo { get; set; }
        public decimal? ImpCuota { get; set; }
        public decimal? ImpMora { get; set; }
        public decimal? TotPago { get; set; }
        public decimal? ImpInteres { get; set; }
    }
}
