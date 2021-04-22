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

            _logger.LogInformation($"preenchendo o campo Renavam: {renavam}");
            var renavamField = _webDriver.FindElement(By.XPath("//*[@id='txtRenavam']"));
            renavamField.Clear();
            renavamField.SendKeys(renavam);


            _logger.LogInformation($"preenchendo o campo Placa: {placa}");
            var placaField = _webDriver.FindElement(By.XPath("//*[@id='txtplaca']"));
            placaField.Clear();
            placaField.SendKeys(placa);


            _logger.LogInformation($"Resolvendo captcha");
            var imageCaptcha = _webDriver.FindElement(By.Id("imgCaptcha"));
            var imagem = imageCaptcha.GetAttribute("src");


            _webDriver.Navigate().GoToUrl(imagem);
           

            string nomeArquivo = $"{Guid.NewGuid()}.png";

            _webDriver.Screenshot(Path.Combine(Environment.CurrentDirectory,"Images", nomeArquivo));

            _webDriver.Navigate().Back();

            _logger.LogInformation($"Requisitando o captcha depois de salvar a mensagem");
            var captchaResult = await _captchaService.RequisitarCaptchaAsync(Path.Combine(Environment.CurrentDirectory, "Images", nomeArquivo));

            var captchaField = _webDriver.FindElement(By.XPath("//*[@id='txtValidacao']"));
            captchaField.Clear();
            captchaField.SendKeys(captchaResult);
            _logger.LogInformation("Captcha 1 resolvido com sucesso");



            var _id = await _captchaService.RequisitarCaptchaV2Async(_url);
            _logger.LogInformation("Captcha 2 resolvido com sucesso");

            var script = $@"document.getElementById('g-recaptcha-response').innerHTML = '{_id}';";
            _webDriver.ExecuteJavaScript(script);

            _logger.LogInformation("Clicando no botão Confirmar");
            var botaoConfirmarField = _webDriver.FindElement(By.XPath("//*[@id='btnMultas']"));
            botaoConfirmarField.Click();

            await Task.Delay(TimeSpan.FromSeconds(3));


            _logger.LogInformation($"Navegando pra Url: { _url}forms/frmResumoMultas.aspx ");
            _webDriver.Navigate().GoToUrl($"{ _url}forms/frmResumoMultas.aspx");


            _logger.LogInformation("Tirando o print da tela");
            nomeArquivo = $"{renavam}.png";
            _webDriver.Screenshot(Path.Combine(Environment.CurrentDirectory, "Images","Renavam", nomeArquivo));

            _logger.LogInformation("fechando o WebDriver");
            _webDriver.FullDispose();

            await Task.CompletedTask;
        }
        
    
    }
}
