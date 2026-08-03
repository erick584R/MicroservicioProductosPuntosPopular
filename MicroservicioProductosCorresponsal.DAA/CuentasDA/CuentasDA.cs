using BancoPopular.Solicitudes.BantotalServices.BTS;
using MicroservicioProductosCorresponsal.DTO.Cuentas;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroservicioProductosCorresponsal.DAA.CuentasDA
{
    public class CuentasDA : ICuentasDA
    {

        private readonly IBantotalServices _bts;
        private readonly string _urlBase;
        private static readonly HttpClient _httpClient;
        static CuentasDA()
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

        public CuentasDA(IBantotalServices bts)
        {
            _bts = bts;
            _urlBase = Environment.GetEnvironmentVariable("UrlBaseBantotalServices") ?? "";
        }



        public async Task<ResponseInfoCuentaDTO> ObtenerDatosCuentaDestino(RequestInfoCuentaDTO request)
        {
            int ctnro = int.Parse(request.Cuenta[0..^6]);
            try
            {
                var datosCuenta = await _bts.ObtenerProducto_CC_CA(Convert.ToInt32(request.BPInReq.Canal), request.Cuenta);
                if (datosCuenta == null)
                {
                    return null;
                }
                var infoClienteTercero = await _bts.ObtInfoCliente_CTNRO(Convert.ToInt32(request.BPInReq.Canal), ctnro);
                if (infoClienteTercero == null)
                {
                    return null;
                }
                return new ResponseInfoCuentaDTO
                {
                    productosPasivos = datosCuenta,
                    sdtPersona = infoClienteTercero.SdtPersona,

                };
            }
            catch (Exception e)
            {
                throw e;

            }
        }

    }
}
