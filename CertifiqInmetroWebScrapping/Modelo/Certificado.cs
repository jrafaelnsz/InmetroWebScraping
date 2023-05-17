using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertifiqInmetroWebScrapping.Modelo
{
    public class Certificado
    {
        public string Certificador { get; set; }
        public string NumeroCertificado { get; set; }
        public string Tipo { get; set; }
        public string Emissao { get; set; }
        public string Validade { get; set; }
        public string StatusCertificado { get; set; }
        public string DocNormativo { get; set; }
        public string CpfCnpj { get; set; }
        public string RazaoSocialNome { get; set; }
        public string NomeFantasia { get; set; }
        public string Endereco { get; set; }
        public string Status { get; set; }
        public string PapelEmpresa { get; set; }
    }
}
