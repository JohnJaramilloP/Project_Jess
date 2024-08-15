using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace Acme.BookStore.Blazor
{
    public static class FileUtils
    {
        public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
            => js.InvokeAsync<object>(
               "saveAsFile",
               filename,
               Convert.ToBase64String(data));

        public async static Task<bool> AlertDelete(this IJSRuntime js, string title, string message, TipoMensajeSweetAlert tipoMensajeSweetAlert)
        {
            return await js.InvokeAsync<bool>("CustomConfirm", title, message, tipoMensajeSweetAlert.ToString());
        }

		public static void AlertSuccess(this IJSRuntime js, string message)
        {
            js.InvokeAsync<object>("Swal.fire", message);
        }
        public static void alertTextTitle(this IJSRuntime js, string title, string message, TipoMensajeSweetAlert tipoMensajeSweetAlert)
        {
            js.InvokeAsync<object>("Swal.fire", title, message, tipoMensajeSweetAlert.ToString());
        }
        public static async Task<string> handleSelectChange(this IJSRuntime js)
        {
            return await js.InvokeAsync<string>("handleSelectChange");
        }

        public static async Task<string> DetectarDatosNavegador(this IJSRuntime js)
        {
            return await js.InvokeAsync<string>("DetectarDatosNavegador");
        }

    }


    public enum TipoMensajeSweetAlert
    {
        question, warning, error, success, info
    }
}
