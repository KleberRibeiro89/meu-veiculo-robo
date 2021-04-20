using meu_veiculo_robo.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meu_veiculo_robo.Serivces.MeuVeiculo
{
    public class MeuVeiculoService : IMeuVeiculoSerivce
    {
        private readonly ILogger<MeuVeiculoService> _logger;
        private readonly IWebDriver _webDriver;
        private readonly string _url;
        private readonly ICaptchaService _captchaService;
        public MeuVeiculoService(ILogger<MeuVeiculoService> logger,
                                IConfiguration configuration,
                                ICaptchaService captchaService)
        {
            _logger = logger;
            _url = configuration.GetValue<string>("MeuVeiculo:Url");
            _webDriver = WebDriverExtensions.CriarDriveChrome();
            _captchaService = captchaService;
        }
        public async Task FazerLeituraAsync(string renavam, string placa)
        {
            _logger.LogInformation("Começar a leitura");
            _logger.LogInformation($"Url: {_url}");
            _webDriver.Navigate().GoToUrl(_url);

            var renavamField = _webDriver.FindElement(By.XPath("//*[@id='txtRenavam']"));
            renavamField.Clear();
            renavamField.SendKeys(renavam);

            var placaField = _webDriver.FindElement(By.XPath("//*[@id='txtplaca']"));
            placaField.Clear();
            placaField.SendKeys(placa);


            _logger.LogDebug($"Resolvendo captcha");
            var imageCaptcha = _webDriver.FindElement(By.Id("imgCaptcha"));
            var imagem = imageCaptcha.GetAttribute("src");


            _webDriver.Navigate().GoToUrl(imagem);
           

            string nomeArquivo = $"{Guid.NewGuid()}.png";

            _webDriver.Screenshot(Path.Combine(Environment.CurrentDirectory,"Images", nomeArquivo));

            _webDriver.Navigate().Back();

            _logger.LogDebug($"Requisitando o captcha depois de salvar a mensagem");
            var captchaResult = await _captchaService.RequisitarCaptchaAsync(Path.Combine(Environment.CurrentDirectory, "Images", nomeArquivo));

            var captchaField = _webDriver.FindElement(By.XPath("//*[@id='txtValidacao']"));
            captchaField.Clear();
            captchaField.SendKeys(captchaResult);

            var _id = await _captchaService.RequisitarCaptchaV2Async(_url);

            var recaptchaButton = _webDriver.FindElement(By.XPath("//*[@id='recaptcha-anchor-label']"));
            recaptchaButton.Click();

            

            _logger.LogInformation("site acessado com sucesso");
            await Task.CompletedTask;

        }
    }
}
