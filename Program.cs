using meu_veiculo_robo.Serivces;
using meu_veiculo_robo.Serivces.MeuVeiculo;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace meu_veiculo_robo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddTransient<IMeuVeiculoSerivce, MeuVeiculoService>();
                    services.AddTransient<ICaptchaService, CaptchaService>();
                });
    }
}
