using BancoPopular.Solicitudes.BantotalServices.DTO.Generales;
using BancoPopular.Solicitudes.BantotalServices.DTO.InformacionCliente;
using BancoPopular.Solicitudes.BantotalServices.DTO.Productos;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroservicioProductosCorresponsal.DTO.Cuentas
{
    public class RequestInfoCuentaDTO
    {
        public BpInReqDTO BPInReq { get; set; }
        public string? Cuenta { get; set; }
    }


    public class ResponseInfoCuentaDTO
    {
        public BPOutReqDTO? BpOutReq { get; set; }
        public ResponseProductosDTO? productosPasivos { get; set; }
        public SdtPersona? sdtPersona { get; set; }
    }
}
