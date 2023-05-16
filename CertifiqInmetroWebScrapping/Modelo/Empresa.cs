using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertifiqInmetroWebScrapping.Modelo
{
    public class Empresa
    {
        public string NomeEmpresa { get; set; }
        public string UnidadeNegocio { get; set; }
        public string UF { get; set; }
        public string PadraoNormativo { get; set; }
        public string NumeroCertificado { get; set; }
        public string OrganismoCertificador { get; set; }
        public string OrganismoAcreditadoCGCRE { get; set; }
        public Escopo Escopos { get; set; }
        public string Situaco { get; set; }
    }
}
