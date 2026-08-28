using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;
using BancoPopular.Solicitudes.BantotalServices.DTO.Productos;
using Newtonsoft.Json;

namespace MicroservicioProductosCorresponsal.DTO.Prestamos
{
    public class ResponseBPListaPrestamosDTO
    {
        public SdtProductosPrestamos? Prestamos { get; set; }
        public BPOutReqDTO? BpOutReq { get; set; }
    }

    public class BPRespListaPrestamosDTO
    {
        [JsonProperty("sdtProductosPrestamos")]
        public SdtProductosPrestamos? sdtProductosPrestamos { get; set; }

        [JsonProperty("erroresNegocio")]
        public ErroresNegocioDTO? erroresNegocio { get; set; }

        [JsonProperty("BtoutReq")]
        public BtoutreqDTO? BtoutReq { get; set; }
    }

    public class SdtProductosPrestamos
    {
        [JsonProperty("sBTProductoPrestamo")]
        public List<SBTProductoPrestamo>? sBTProductoPrestamo { get; set; }
    }

    public class SBTProductoPrestamo
    {
        [JsonProperty("sucursal")]
        public string? Sucursal { get; set; }

        [JsonProperty("saldo")]
        public double Saldo { get; set; }

        [JsonProperty("idOperacionBT")]
        public string? IdOperacionBT { get; set; }

        [JsonProperty("producto")]
        public Producto? producto { get; set; }

        public string? CuentaFormateada { get; set; }

    }
}
