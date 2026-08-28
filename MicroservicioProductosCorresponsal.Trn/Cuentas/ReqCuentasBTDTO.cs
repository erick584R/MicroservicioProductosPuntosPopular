using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;
using Newtonsoft.Json;

namespace MicroservicioProductosCorresponsal.DTO.Cuentas
{
    public class ReqCuentasBTDTO
    {
        [JsonProperty("Btinreq")]
        public BtinreqDTO? Btinreq { get; set; }

        [JsonProperty("clienteUId")]
        public int ClienteUId { get; set; }
    }
}
