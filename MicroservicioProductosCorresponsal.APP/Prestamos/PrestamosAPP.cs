using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;
using MicroservicioProductosCorresponsal.DAA.Prestamos;
using MicroservicioProductosCorresponsal.DTO.Prestamos;
using Microsoft.Extensions.Configuration;

namespace MicroservicioProductosCorresponsal.APP.Prestamos
{
    public class PrestamosAPP : IPrestamosAPP
    {
        private readonly IPrestamosDA _prestamosDA;
        private readonly IConfiguration _configuration;
        private readonly BPOutReqDTO _bpoutReq;

        public PrestamosAPP(IPrestamosDA prestamosDA, IConfiguration configuration)
        {
            _prestamosDA = prestamosDA;
            _configuration = configuration;
            _bpoutReq = new BPOutReqDTO();
        }

        public async Task<ResponseBPListaPrestamosDTO> ObtenerPrestamos(ReqListaPrestamosDTO reqCuentasAhorroDTO)
        {
            try
            {
                // Validaciones de entrada
                if (reqCuentasAhorroDTO == null || reqCuentasAhorroDTO.BPInReq == null || reqCuentasAhorroDTO.BPInReq.Canal == null || reqCuentasAhorroDTO.BPInReq.Canal == 0)
                {
                    return new ResponseBPListaPrestamosDTO
                    {
                        BpOutReq = new BPOutReqDTO
                        {
                            CodigoError = "1001",
                            MensajeError = _configuration.GetSection("MensajesError")?["1001"] ?? "El canal es requerido.",
                            FechaHora = DateTime.Now
                        }
                    };
                }



                // Validaciones de entrada
                if (reqCuentasAhorroDTO.BPInReq.Canal != 3)
                {
                    return new ResponseBPListaPrestamosDTO
                    {
                        BpOutReq = new BPOutReqDTO
                        {
                            CodigoError = "1031",
                            MensajeError = _configuration.GetSection("MensajesError")?["1031"] ?? "Canal no valido.",
                            FechaHora = DateTime.Now
                        }
                    };
                }



                if (string.IsNullOrWhiteSpace(reqCuentasAhorroDTO.NumeroDocumento))
                {
                    return new ResponseBPListaPrestamosDTO
                    {
                        BpOutReq = new BPOutReqDTO
                        {
                            CodigoError = "1006",
                            MensajeError = _configuration.GetSection("MensajesError")?["1006"] ?? "El Número de Documento es requerido.",
                            FechaHora = DateTime.Now
                        }
                    };
                }

                // Llamada a la capa DA
                var respDA = await _prestamosDA.ListaPrestamos(reqCuentasAhorroDTO);

                if (respDA == null)
                {
                    return new ResponseBPListaPrestamosDTO
                    {
                        BpOutReq = new BPOutReqDTO
                        {
                            CodigoError = "1005",
                            MensajeError = _configuration.GetSection("MensajesError")?["1005"] ?? "No se pudo realizar la Transacción.",
                            FechaHora = DateTime.Now
                        }
                    };
                }

                var result = new ResponseBPListaPrestamosDTO();

                // Si el DA devolvió BtoutReq (wrapper), evaluarlo primero
                if (respDA.BtoutReq != null)
                {
                    if (!string.IsNullOrWhiteSpace(respDA.BtoutReq.Estado) && respDA.BtoutReq.Estado.Equals("OK", StringComparison.OrdinalIgnoreCase))
                    {
                        // Si está OK pero no trae datos, respondemos acorde (mismo comportamiento que Productos)
                        if (respDA.sdtProductosPrestamos == null || respDA.sdtProductosPrestamos.sBTProductoPrestamo == null || !respDA.sdtProductosPrestamos.sBTProductoPrestamo.Any())
                        {
                            result.BpOutReq = new BPOutReqDTO
                            {
                                CodigoError = "1002",
                                MensajeError = _configuration.GetSection("MensajesError")?["1002"] ?? "No se encontraron préstamos para el cliente.",
                                FechaHora = DateTime.Now
                            };
                            return result;
                        }

                        // Mapear datos y formatear cuentas
                        result.Prestamos = new SdtProductosPrestamos
                        {
                            sBTProductoPrestamo = respDA.sdtProductosPrestamos.sBTProductoPrestamo
                        };

                        // Formateo seguro y mapeo de moneda
                        if (result.Prestamos?.sBTProductoPrestamo != null)
                        {
                            const int minLength = 46;
                            foreach (var prestamobp in result.Prestamos.sBTProductoPrestamo)
                            {
                                try
                                {
                                    var idOp = prestamobp?.IdOperacionBT;
                                    if (!string.IsNullOrWhiteSpace(idOp) && idOp.Length >= minLength)
                                    {
                                        // Formateador expuesto en la capa DA
                                        prestamobp.CuentaFormateada = _prestamosDA.FormatearProductoP_DPF_BP(idOp);
                                    }
                                    else if (!string.IsNullOrWhiteSpace(idOp))
                                    {
                                        // cadena presente pero menor a la esperada -> devolverla tal cual
                                        prestamobp.CuentaFormateada = idOp;
                                    }
                                    else
                                    {
                                        prestamobp.CuentaFormateada = "SIN CUENTA";
                                    }

                                    if (prestamobp.producto != null && !string.IsNullOrWhiteSpace(prestamobp.producto.Moneda))
                                    {
                                        var monedaUpper = prestamobp.producto.Moneda.Trim().ToUpperInvariant();
                                        prestamobp.producto.Moneda = monedaUpper == "LPS" ? "HNL" : (monedaUpper == "US$" ? "USD" : monedaUpper);
                                    }
                                }
                                catch (Exception exFormat)
                                {
                                    // No detener el flujo por un error de formateo en un registro
                                    Console.WriteLine($"[ObtenerPrestamos] Error al formatear IdOperacionBT '{prestamobp?.IdOperacionBT}': {exFormat.Message}");
                                    try { prestamobp.CuentaFormateada = prestamobp.IdOperacionBT ?? "SIN CUENTA"; } catch { prestamobp.CuentaFormateada = "SIN CUENTA"; }
                                }
                            }
                        }

                        result.BpOutReq = new BPOutReqDTO
                        {
                            CodigoError = "0",
                            MensajeError = _configuration.GetSection("MensajesError")?["0"] ?? "Éxito",
                            FechaHora = DateTime.Now
                        };

                        return result;
                    }
                    else
                    {
                        // Servicio devolvió BtoutReq con estado distinto de OK
                        result.BpOutReq = new BPOutReqDTO
                        {
                            CodigoError = "1005",
                            MensajeError = respDA.BtoutReq.Estado ?? respDA.erroresNegocio?.ToString() ?? "Error en servicio",
                            FechaHora = DateTime.Now
                        };
                        return result;
                    }
                }
                else if (respDA.erroresNegocio != null)
                {
                    // DA devolvió errores de negocio
                    result.BpOutReq = new BPOutReqDTO
                    {
                        CodigoError = "1005",
                        MensajeError = respDA.erroresNegocio.ToString(),
                        FechaHora = DateTime.Now
                    };
                    return result;
                }
                else
                {
                    // Caso fallback: no viene BtoutReq ni erroresNegocio, intentamos mapear si hay datos
                    if (respDA.sdtProductosPrestamos != null && respDA.sdtProductosPrestamos.sBTProductoPrestamo != null && respDA.sdtProductosPrestamos.sBTProductoPrestamo.Any())
                    {
                        result.Prestamos = new SdtProductosPrestamos
                        {
                            sBTProductoPrestamo = respDA.sdtProductosPrestamos.sBTProductoPrestamo
                        };

                        const int minLength = 46;
                        foreach (var prestamobp in result.Prestamos.sBTProductoPrestamo)
                        {
                            try
                            {
                                var idOp = prestamobp?.IdOperacionBT;
                                if (!string.IsNullOrWhiteSpace(idOp) && idOp.Length >= minLength)
                                {
                                    prestamobp.CuentaFormateada = _prestamosDA.FormatearProductoP_DPF_BP(idOp);
                                }
                                else if (!string.IsNullOrWhiteSpace(idOp))
                                {
                                    prestamobp.CuentaFormateada = idOp;
                                }
                                else
                                {
                                    prestamobp.CuentaFormateada = "SIN CUENTA";
                                }

                                if (prestamobp.producto != null && !string.IsNullOrWhiteSpace(prestamobp.producto.Moneda))
                                {
                                    var monedaUpper = prestamobp.producto.Moneda.Trim().ToUpperInvariant();
                                    prestamobp.producto.Moneda = monedaUpper == "LPS" ? "HNL" : (monedaUpper == "US$" ? "USD" : monedaUpper);
                                }
                            }
                            catch (Exception exFormat)
                            {
                                Console.WriteLine($"[ObtenerPrestamos] Error al formatear IdOperacionBT '{prestamobp?.IdOperacionBT}': {exFormat.Message}");
                                try { prestamobp.CuentaFormateada = prestamobp.IdOperacionBT ?? "SIN CUENTA"; } catch { prestamobp.CuentaFormateada = "SIN CUENTA"; }
                            }
                        }

                        result.BpOutReq = new BPOutReqDTO
                        {
                            CodigoError = "0",
                            MensajeError = _configuration.GetSection("MensajesError")?["0"] ?? "Éxito",
                            FechaHora = DateTime.Now
                        };
                        return result;
                    }
                    else
                    {
                        // No hay datos pero tampoco errores explícitos: devolver código que indique ausencia
                        result.BpOutReq = new BPOutReqDTO
                        {
                            CodigoError = "1002",
                            MensajeError = _configuration.GetSection("MensajesError")?["1002"] ?? "No se encontraron préstamos para el cliente.",
                            FechaHora = DateTime.Now
                        };
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                return new ResponseBPListaPrestamosDTO
                {
                    BpOutReq = new BPOutReqDTO
                    {
                        CodigoError = "500",
                        MensajeError = $"Error interno: {ex.Message}",
                        FechaHora = DateTime.Now
                    }
                };
            }
        }
    }
}
