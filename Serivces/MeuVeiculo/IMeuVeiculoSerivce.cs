using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace meu_veiculo_robo.Serivces.MeuVeiculo
{
    public interface IMeuVeiculoSerivce 
    {
        Task FazerLeituraAsync(string renavam, string placa);
    }
}
