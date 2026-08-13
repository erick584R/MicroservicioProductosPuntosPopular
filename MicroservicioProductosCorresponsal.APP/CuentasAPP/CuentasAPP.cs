using BancoPopular.Servicios.Servicio;
using BancoPopular.Solicitudes.BantotalServices.BTS;
using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;
using MicroservicioProductosCorresponsal.DTO.Cuentas;
using MicroservicioProductosCorresponsal.DAA.CuentasDA;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroservicioProductosCorresponsal.APP.CuentasAPP
{
    public class CuentasAPP: ICuentasAPP
    {

        private readonly ICuentasDA _cuentasDA;
        private readonly IServicio _helper;
        private readonly IConfiguration _configuration;
        private readonly IBantotalServices _bts;
        //private readonly ILogsUsuarioDA _logsUsuario;
        private readonly BPOutReqDTO _bpoutReqDTO;

        //private AccionesdelUsuarioDTO _datosRequestLog = new AccionesdelUsuarioDTO();

        public CuentasAPP(ICuentasDA cuentasDA, IServicio helper, IConfiguration configuration, IBantotalServices bts)
        {
            _cuentasDA = cuentasDA;
            _helper = helper;
            _configuration = configuration;
            _bts = bts;
            //_logsUsuario = logsUsuario;
            _bpoutReqDTO = new BPOutReqDTO();
        }


        public async Task<ResponseInfoCuentaDTO> ObtenerDatosCuentaDestino(RequestInfoCuentaDTO request)
        {
            var response = new ResponseInfoCuentaDTO();
            ResponseInfoCuentaDTO? obtenerDatos = null;
            try
            {
                //Buscamos la equivalencia de la cuenta en la tabla de cuentas equivalentes, si no se encuentra, se retorna el error 1013
                string EncontrarEquivalencia = await _cuentasDA.ObtenerCuentaEquivalenteAsync(Convert.ToInt32(request.BPInReq.Canal), request.Cuenta);

                //Si no se encontro la cuenta equivalente, se retorna el error 1013
                if (EncontrarEquivalencia == null)
                {
                    _bpoutReqDTO.CodigoError = "1013";
                    _bpoutReqDTO.MensajeError = _configuration
                        .GetSection("MensajesError")[_bpoutReqDTO.CodigoError];
                    _bpoutReqDTO.FechaHora = DateTime.Now;

                    response.BpOutReq = _bpoutReqDTO;


                    return response;

                }

                // Si se encontro la cuenta equivalente, se asigna a la propiedad Cuenta del request
                request.Cuenta = EncontrarEquivalencia;


                obtenerDatos = await _cuentasDA.ObtenerDatosCuentaDestino(request);
                if (obtenerDatos?.productosPasivos?.ProductosPasivos?.ProductoPasivo == null)
                {
                    _bpoutReqDTO.CodigoError = "1014";
                    _bpoutReqDTO.MensajeError = _configuration
                        .GetSection("MensajesError")[_bpoutReqDTO.CodigoError];
                    _bpoutReqDTO.FechaHora = DateTime.Now;

                    response.BpOutReq = _bpoutReqDTO;

                    
                    return response;

                }
                
                _bpoutReqDTO.CodigoError = "0";
                _bpoutReqDTO.MensajeError = _configuration
                    .GetSection("MensajesError")[_bpoutReqDTO.CodigoError];
                _bpoutReqDTO.FechaHora = DateTime.Now;
                obtenerDatos.productosPasivos.ProductosPasivos.ProductoPasivo[0].Producto.Moneda = obtenerDatos.productosPasivos.ProductosPasivos.ProductoPasivo[0].Producto.Moneda == "LPS" ? "HNL" : "USD";

                response.BpOutReq = _bpoutReqDTO;
                response.sdtPersona = obtenerDatos.sdtPersona;
                response.productosPasivos = obtenerDatos.productosPasivos;

                
                return response;
            }
            catch (Exception e)
            {
                _bpoutReqDTO.CodigoError = "1010";
                _bpoutReqDTO.MensajeError = _configuration
                    .GetSection("MensajesError")[_bpoutReqDTO.CodigoError];
                _bpoutReqDTO.FechaHora = DateTime.Now;

                response.BpOutReq = _bpoutReqDTO;

                
                return response;
            }
        }
    }
}
