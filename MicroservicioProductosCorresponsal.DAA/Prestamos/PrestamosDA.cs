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
            catch(Exception ex)
            {
                respuesta = new BPRespListaPrestamosDTO
                {
                    sdtProductosPrestamos = null,
                    BtoutReq = new BtoutreqDTO()
                    {
                        Estado = $"Error - {ex.Message}"
                    }
                };
                return respuesta;
            }

        }




        public async Task<ConsultaCuotasBTResponseDTO> ConsultarDetallesPrestamoDA(ReqConsultaPrestamoDTO request)
        {

            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // Validaciones mínimas locales (la capa App también valida)
            if (request.BPInReq == null || request.BPInReq.Canal == null || request.BPInReq.Canal == 0)
            {
                return new ConsultaCuotasBTResponseDTO
                {
                    Btoutreq = new BtoutreqDTO
                    {
                        Estado = "ERROR",
                        Fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                        Hora = DateTime.Now.ToString("HH:mm:ss")
                    }
                };
            }

            int canal = request.BPInReq.Canal;

            // Iniciar Btinreq usando el paquete (gestión de token/session)
            BtinreqDTO btinreq;
            try
            {
                btinreq = await _bts.IniciarBtinreq(canal);
                if (btinreq == null)
                {
                    btinreq = new BtinreqDTO();
                    btinreq.Canal = "BTDIGITAL";
                    btinreq.Device = request.BPInReq?.DispositivoFisico ?? "AV";
                    btinreq.Requerimiento = "1";
                    btinreq.Usuario = Environment.GetEnvironmentVariable("UsuarioBantotalServices") ?? btinreq.Usuario;
                }
            }
            catch (Exception ex)
            {
                return new ConsultaCuotasBTResponseDTO
                {
                    Btoutreq = new BtoutreqDTO
                    {
                        Estado = $"Error - IniciarBtinreq: {ex.Message}",
                        Fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                        Hora = DateTime.Now.ToString("HH:mm:ss")
                    }
                };
            }

            // Construir body siguiendo tu ejemplo
            var bodyObj = new
            {
                Btinreq = btinreq,
                //Canal = request.BPInReq.Canal,
                CantidadCuotas = request.CantidadCuotas ?? 0,
                Credito = request.Credito ?? string.Empty,
                //Credito= cuentaIngresada,
                Documento = request.Documento ?? string.Empty,
                Fecha = request.Fecha ?? string.Empty,
                Hora = request.Hora ?? string.Empty,
                IdCorresponsal = request.IdCorresponsal ?? string.Empty,
                IdTerminal = request.IdTerminal ?? string.Empty,
                Moneda = request.Moneda ?? 1,
                TipoDocumento = request.TipoDocumento ?? 0
            };

            string UrlComplementaria = "BTCorresponsales_v1";
            string MetodoUsado = "ConsultaCuotas";

            var httpClient = new HttpClient(new HttpClientHandler
            {
                // Mantengo el mismo comportamiento inseguro de validación de certificado que hay en tu paquete para ser consistente
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            });

            try
            {
                string json = JsonConvert.SerializeObject(bodyObj);
                HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{_urlBase}{UrlComplementaria}?{MetodoUsado}", content);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (string.IsNullOrEmpty(responseBody))
                    return null;

                var resp = JsonConvert.DeserializeObject<ConsultaCuotasBTResponseDTO>(responseBody);

                // Si la respuesta trae una Cuenta, intentamos obtener ClienteUId vía ObtInfoCliente_CTNRO
                try
                {
                    if (resp != null && resp.Cuenta.HasValue && resp.Cuenta.Value != 0)
                    {
                        int ctnro = resp.Cuenta.Value;
                        Console.WriteLine($"[PrestamosDA.ConsultarDetallesPrestamoDA] Obteniendo ClienteUId para canal {canal}, ctnro {ctnro}");

                        // Llamada al método del paquete para obtener cliente por cuenta (CTNRO)
                        var clienteResp = await _bts.CTNRO_ClienteUId(canal, ctnro);
                        if (clienteResp != null && clienteResp != null)
                        {
                            resp.ClienteUId = int.Parse(clienteResp);
                            Console.WriteLine($"[PrestamosDA.ConsultarDetallesPrestamoDA] ClienteUId resuelto: {resp.ClienteUId}");
                        }
                        else
                        {
                            Console.WriteLine("[PrestamosDA.ConsultarDetallesPrestamoDA] ObtInfoCliente_CTNRO devolvió null o sin SdtPersona");
                        }

                        // Intentar obtener OperacionUId del préstamo (ObtenerProducto_Prestamo) usando la cuenta como CuentaBP
                        /*try
                        {
                            var prodPrestamoResp = await _bts.ObtenerProducto_Prestamo(canal, ctnro.ToString());
                            if (prodPrestamoResp?.ProductosPrestamos?.ProductoPrestamo != null &&
                                prodPrestamoResp.ProductosPrestamos.ProductoPrestamo.Count > 0)
                            {
                                var primerProducto = prodPrestamoResp.ProductosPrestamos.ProductoPrestamo[0];
                                resp.OperacionUid_Clioente = primerProducto.OperacionUId;
                                Console.WriteLine($"[PrestamosDA.ConsultarDetallesPrestamoDA] OperacionUId (cliente) resuelto: {resp.OperacionUid_Clioente}");
                            }
                            else
                            {
                                Console.WriteLine("[PrestamosDA.ConsultarDetallesPrestamoDA] ObtenerProducto_Prestamo no devolvió productos");
                            }
                        }
                        catch (Exception exProd)
                        {
                            Console.WriteLine($"[PrestamosDA.ConsultarDetallesPrestamoDA] Error al obtener producto prestamo: {exProd.Message}");
                        }*/
                    }
                    else
                    {
                        Console.WriteLine("[PrestamosDA.ConsultarDetallesPrestamoDA] No se encontró Cuenta en la respuesta de ConsultaCuotas; no se puede resolver ClienteUId/OperacionUId");
                    }
                }
                catch (Exception exInner)
                {
                    Console.WriteLine($"[PrestamosDA.ConsultarDetallesPrestamoDA] Error al resolver ClienteUId/OperacionUId: {exInner.Message}");
                }

                return resp;
            }
            catch (Exception ex)
            {
                return new ConsultaCuotasBTResponseDTO
                {
                    Btoutreq = new BtoutreqDTO
                    {
                        Estado = $"Error - {ex.Message}",
                        Fecha = DateTime.Now.ToString("yyyy-MM-dd"),
                        Hora = DateTime.Now.ToString("HH:mm:ss")
                    }
                };
            }
        }


        #region Auxiliares
        public string FormatearProductoP_DPF_BP(string idOperacionBT)
        {
            if (string.IsNullOrWhiteSpace(idOperacionBT))
                return string.Empty;

            // longitud mínima esperada para usar los índices que usas (ajusta si tu formato diferente)
            const int minLength = 46;
            if (idOperacionBT.Length < minLength)
            {
                // devolver id original o un marcador para diagnóstico
                return idOperacionBT;
            }

            try
            {
                var part1 = idOperacionBT.Substring(10, 3);
                var part2 = idOperacionBT.Substring(37, 9).TrimStart('0');
                var part3 = idOperacionBT.Substring(34, 3);
                if (string.IsNullOrEmpty(part3))
                    part3 = "0";

                var formatted = $"{part1}{part2}{part3}";
                return formatted;
            }
            catch (Exception)
            {
                // si algo inesperado falla, devolver el original (o "SIN CUENTA")
                return idOperacionBT;
            }
        }




        public async Task<string> ObtenerCuentaEquivalenteAsync(string cuentaNormal)
        {
            // Usa "3" como canal para Corresponsal (este valor puede ajustarse si en el futuro cambia)
            int canalBP = 3; // o el valor correspondiente según tu negocio
                             // Ejecuta método de Bantotal Services/Solicitudes
            var equivalencia = await _bts.BuscarEquivalencia(canalBP, cuentaNormal);

            // Si existe equivalencia (CuentaBP es numérica, sin guiones y diferente de la original), la retorna
            if (equivalencia.CuentaBP != null || equivalencia.BPOutReq.CodigoError == "0")
            {
                return equivalencia.CuentaBP;
            }
            // Si hay error, retorna la misma cuenta de entrada (sin modificación)
            cuentaNormal = null;
            return cuentaNormal;
        }
        #endregion

    }
}
