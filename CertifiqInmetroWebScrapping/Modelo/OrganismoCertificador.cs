using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertifiqInmetroWebScrapping.Modelo
{
    public class OrganismoCertificador
    {
        public string Valor { get; set; }
        public string Descricao { get; set; }

        public OrganismoCertificador(string valor, string descricao)
        {
            Valor = valor;
            Descricao = descricao;
        }
    }
}
