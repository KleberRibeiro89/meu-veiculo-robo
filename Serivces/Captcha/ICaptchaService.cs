using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meu_veiculo_robo.Serivces
{
    public interface ICaptchaService
    {
        Task<string> RequisitarCaptchaAsync(byte[] imageBase64, string extensaoImage);

        Task<string> RequisitarCaptchaAsync(string caminhoImagem);

        Task<string> ResultadoCaptchaAsync(string requestId);

        Task<string> RequisitarCaptchaV2Async(string url);
    }
}
