using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace meu_veiculo_robo.Serivces
{
    public class CaptchaService : ICaptchaService
    {
        private readonly string _urlRequisicao;
        private readonly string _urlResposta;
        private readonly string _keyCaptcha;
        private readonly string _googleKey;

        public CaptchaService(IConfiguration configuration)
        {
            _urlRequisicao = configuration.GetValue<string>("Captcha:UrlRequisicao");
            _urlResposta = configuration.GetValue<string>("Captcha:UrlResposta");
            _keyCaptcha = configuration.GetValue<string>("Captcha:Key");
            _googleKey = configuration.GetValue<string>("Captcha:GoogleKey");
        }

        public async Task<string> RequisitarCaptchaAsync(byte[] imageBase64, string extensaoImage)
        {
            string nomeArquivo = $"{Guid.NewGuid()}.{extensaoImage}";
            try
            {
                File.WriteAllBytes(Path.Combine(Environment.CurrentDirectory, nomeArquivo), imageBase64);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao converter o Byte[] em imagem", ex);
            }

            return await RequisitarCaptchaAsync(Path.Combine(Environment.CurrentDirectory, "images", nomeArquivo));
        }

        public async Task<string> RequisitarCaptchaAsync(string caminhoImagem)
        {
            Console.WriteLine($"Requisitando o captcha da imagem: {caminhoImagem}");
            using var client = new HttpClient();
            var stream = new FileStream(caminhoImagem, FileMode.Open);
            HttpContent content = new StreamContent(stream);
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = caminhoImagem
            };
            var form = new MultipartFormDataContent { { new StringContent(_keyCaptcha), "key" }, content };

            var response = await client.PostAsync(_urlRequisicao, form);

            if (!response.IsSuccessStatusCode)
                Console.WriteLine($"Erro na requisição de captcha, status code: {response.StatusCode}");

            var retorno = await response.Content.ReadAsStringAsync();
            return await ResultadoCaptchaAsync(retorno.Replace("OK|", ""));
        }

        public async Task<string> ResultadoCaptchaAsync(string requestId)
        {
            Console.WriteLine($"Aguardando o resultado do captcha {requestId}");

            using var client = new HttpClient { BaseAddress = new Uri(_urlResposta) };
            await Task.Delay(TimeSpan.FromSeconds(5));
            int tentativas = 10;

            for (var tentativa = 0; tentativa < tentativas; tentativa++)
            {
                Console.WriteLine($"Tentativa {tentativa},  {requestId}");

                var result = client.GetStringAsync($"{_urlResposta}key={_keyCaptcha}&action=get&id={requestId}").Result;

                if (result.Contains("OK|"))
                    return result.Replace("OK|", "");

                if (result.Contains("ERROR_CAPTCHA_UNSOLVABLE"))
                    throw new ArgumentException("Erro na API de captcha.");

                if (result.Contains("ERROR_ZERO_BALANCE"))
                    throw new ArgumentException("Erro na API de captcha.");

                await Task.Delay(TimeSpan.FromSeconds(10));
            }

            throw new ArgumentException("Captcha demorou muito pra responder.");
        }

        public async Task<string> RequisitarCaptchaV2Async(string url)
        {
            Console.WriteLine($"Requisitar o Captchav2");
            using var client = new HttpClient { BaseAddress = new Uri(_urlRequisicao) };
            var result = await client.GetStringAsync($"{_urlRequisicao}?key={_keyCaptcha}&method=userrecaptcha&googlekey={_googleKey}&pageurl={url}forms/frmPesquisarRenavam.aspx");
            return await ResultadoCaptchaAsync(result.Replace("OK|", ""));
        }
    }
}
