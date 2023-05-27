using CertifiqInmetroWebScrapping.MongoDataAccess.DataAccessLayer;
using CertifiqInmetroWebScrapping.MongoDataAccess.Model;
using CertifiqInmetroWebScrapping.MongoDataAccess.Repository;
using HtmlAgilityPack;
using System.Web;

namespace CertifiqInmetroWebScrapping.Scrap
{
    public class PreCertificadoScraping
    {
        public void ProcessarDocumento() 
        {
            //Obter documento do banco de dados
            var documento = ObterDocumento();

            //obter dados do certificado do HTML
            var listaPreCertificados = ObterPreCertificado(documento.Html.Substring(0, documento.Html.IndexOf("</html>")+7));

            //salvar dados do certificado
            SalvarPreCertificado(listaPreCertificados);

            //remover documento do banco de dados
            RemoverDocumento(documento);
        } 

        private HtmlConvenioModel ObterDocumento()
        {
            return new MyMongoDbContext().ObterHtmlConvenio();
        }

        private List<PreCertificadoModel> ObterPreCertificado(string html)
        {
            var lista = new List<PreCertificadoModel>();

            //carregar html obtido no banco de dados
            var doc = new HtmlDocument();            
            doc.LoadHtml(HttpUtility.HtmlDecode(html));

            //Procuro pelo elemento strong
            var strongElements = doc.DocumentNode.SelectNodes("//strong");

            foreach (var item in strongElements)
            {
                if (!item.InnerText.Equals("Doc.Normativo"))
                {
                    var certificado = new PreCertificadoModel();

                    var list = new List<string>();
                    list = item.InnerText.Split(":").ToList();

                    if (list.Count == 7)
                    {
                        certificado.Certificador = list[1].Replace("Nº Certificado", "").Trim();
                        certificado.NumeroCertificado = list[2].Replace("Tipo", "").Trim();
                        certificado.Tipo = list[3].Replace("Emissão", "").Trim();
                        certificado.Emissao = list[4].Replace("Validade", "").Trim();
                        certificado.Validade = list[5].Replace("Status do Certificado", "").Trim();
                        certificado.StatusCertificado = list[6].Trim();
                        lista.Add(certificado);
                    }
                }
            }


            return lista;
        }

        private void SalvarPreCertificado(List<PreCertificadoModel> preCertificados)
        {
            var dbContext = new MyMongoDbContext();
            var repositorio = new PreCertificadoRepository(dbContext);
            repositorio.Add(preCertificados);
            //var task = db.SalvarPreCertificados(preCertificados);
            //task.Wait();
        }

        private void RemoverDocumento(HtmlConvenioModel htmlConvenio)
        {
            var db = new MyMongoDbContext();
            db.DeleteHtmlConvenio(htmlConvenio);
        }
    }
}
