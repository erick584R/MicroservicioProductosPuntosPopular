using MicroservicioProductosCorresponsal.DTO.Cuentas;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroservicioProductosCorresponsal.APP.CuentasAPP
{
    public interface ICuentasAPP
    {
        Task<ResponseInfoCuentaDTO> ObtenerDatosCuentaDestino(RequestInfoCuentaDTO request);
    }
}
