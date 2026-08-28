using BancoPopular.Solicitudes.BantotalServices.BTS;
using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;
using MicroservicioProductosCorresponsal.DTO.Prestamos;
using MicroservicioProductosCorresponsal.DTO.Cuentas;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Text;

namespace MicroservicioProductosCorresponsal.DAA.Prestamos
{
    public class PrestamosDA : IPrestamosDA
    {
        private readonly IBantotalServices _bts;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _urlBase;
        private static readonly HttpClient _httpClient;

        private string UrlComplementaria = "";
        private string MetodoUsado = "";
        private BtinreqDTO _btinreq;
        

        static PrestamosDA()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (_, __, ___, ____) => true
            };

            _httpClient = new HttpClient(handler)
            {
                Timeout = TimeSpan.FromSeconds(15)
            };
        }

        public PrestamosDA(IBantotalServices bts, IHttpContextAccessor httpContextAccessor)
        {
            _bts = bts;
            _httpContextAccessor = httpContextAccessor;
            _urlBase = Environment.GetEnvironmentVariable("UrlBaseBantotalServices") ?? "";
            _btinreq = new BtinreqDTO();
        }


        public async Task<BPRespListaPrestamosDTO> ListaPrestamos(ReqListaPrestamosDTO reqCuentas)
        {
            var resp = await _bts.ObtInfoCliente_NroDocu(reqCuentas.BPInReq.Canal, reqCuentas.NumeroDocumento, 1);

            int ctrno = resp.SdtPersona.Ctnro ?? 0;

            var clienteUidBT = await _bts.CTNRO_ClienteUId(Convert.ToInt32(reqCuentas.BPInReq.Canal), ctrno);
            if (clienteUidBT == null)
            {
                return null;
            }
            UrlComplementaria = "BTClientes_v1";
            MetodoUsado = "ObtenerPrestamos";
            BPRespListaPrestamosDTO respuesta = new BPRespListaPrestamosDTO();
            var httpClient = new HttpClient(new HttpClientHandler { ServerCertificateCustomValidationCallback = (sender, cert, chain, SslPolicyErrors) => { return true; } });
            try
            {
                _btinreq = _bts.IniciarBtinreq(reqCuentas.BPInReq.Canal).GetAwaiter().GetResult();

                string jsonString = JsonConvert.SerializeObject(value: new ReqCuentasBTDTO
                {
                    Btinreq = _btinreq,
                    ClienteUId = Convert.ToInt32(clienteUidBT)
                });
                HttpContent content = new StringContent(jsonString, Encoding.UTF8, "application/json");
                HttpResponseMessage respuestaPeticion = await httpClient.PostAsync($"{_urlBase}{UrlComplementaria}?{MetodoUsado}", content);
                string responseBody = (await respuestaPeticion.Content.ReadAsStringAsync() == null) ? "" : await respuestaPeticion.Content.ReadAsStringAsync();

                respuesta = JsonConvert.DeserializeObject<BPRespListaPrestamosDTO>(responseBody);



                return respuesta;
            }
            catch
            {

            }
            }
    }
}
