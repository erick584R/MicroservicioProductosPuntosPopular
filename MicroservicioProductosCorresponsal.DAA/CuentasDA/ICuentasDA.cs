using MicroservicioProductosCorresponsal.DTO.Cuentas;
using System;
using System.Collections.Generic;
using System.Text;

namespace MicroservicioProductosCorresponsal.DAA.CuentasDA
{
    public interface ICuentasDA
    {

        Task<ResponseInfoCuentaDTO> ObtenerDatosCuentaDestino(RequestInfoCuentaDTO cuenta);
        Task<string> ObtenerCuentaEquivalenteAsync(int canalBP, string cuentaNormal);
    }
}
