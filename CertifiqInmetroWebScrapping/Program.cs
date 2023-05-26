using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.MongoDataAccess.Model;
using CertifiqInmetroWebScrapping.MongoDataAccess.Repository;
using CertifiqInmetroWebScrapping.Scrap;
using CertifiqInmetroWebScrapping.Util;
using System.Configuration;

namespace CertifiqInmetroWebScrapping
{
    internal class Program
    {
        static async Task Main(string[] args)
        {                        
            await MontaMenu();
            Console.ReadKey();
        }

        static async Task MontaMenu()
        {
            Console.Clear();

            Console.WriteLine("0 - Obter quantidade de páginas da consulta asp por certificador");
            Console.WriteLine("1 - Consultar certifiq.inmetro.gov.br/Consulta/ConsultaEmpresas");
            Console.WriteLine("2 - Consultar inmetro.gov.br/prodcert/certificados/busca.asp?");
            Console.WriteLine("3 - Gerar planilha resultante");
            Console.WriteLine("4 - Obter Pre informacao convenios e certificados na busca.asp via request");
            Console.WriteLine("5 - Processar PreCertificados obtido via request");
            


            var opcao = Console.ReadLine();
            await ExecucaoResilienteAsync(opcao);
        }

        static async Task ExecucaoResilienteAsync(string opcao)
        {
            try
            {
                switch (opcao)
                {
                    case "0":
                        ConsultaSiteVelhoPaginas();
                        break;
                    case "1":
                        ConsultaSiteNovo();
                        break;
                    case "2":
                        ConsultaSiteVelho();
                        break;
                    case "3":
                        GerarPlanilha();
                        break;
                    case "4":
                        await ConsultaConveniosAspViaRequestAsync();
                        break;
                    case "5":
                        ProcessarHtmlPreCertificado();
                        break;
                    default:
                        await MontaMenu();
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await ExecucaoResilienteAsync(opcao);
            }
        }

        private static void ProcessarHtmlPreCertificado()
        {
            var task = new MyMongoDB().ObterQuantidadeHtmlConvenioAsync();
            task.Wait();

            
            for (int i = 0; i < task.Result; i++)
            {
                Console.WriteLine($"Processando PreCertificado {i} de {task.Result}");
                new PreCertificadoScraping().ProcessarDocumento();
            }
        }

        private static async Task ConsultaConveniosAspViaRequestAsync()
        {
            var paginas = new MyMongoDB().ObterPaginasConvenio();

            foreach (var item in paginas)
            {
                var db = new MyMongoDB();
                for (int i = 1; i <= item.Paginas; i++)
                {                    
                    // db.TestConnection();
                    var html = await ChupaCabra.CriarRequisicao(i, item.CodConvenio);

                    var myHtml = new HtmlConvenioModel(item.CodConvenio, i, html);
                    db.SalvarPagina(myHtml);                    
                }
                db.DeletePaginaConsultada(item);
            }
        }

        static void ConsultaSiteNovo()
        {
            //1: Obter Organismos certificadores (criar json com resultado)
            //2: Buscar empresas do organismo certificador (criar json com resultado)

            // var organismoCertificadorPath = "c:\\temp\\OrganismoCerificador.json";
            // var organismoCertificadorPath = "c:\\temp\\OrganismoCerificador.json";
            var config = ConfigurationManager.AppSettings["organismoCertificador"];

            var organismoCertificadorPath = ConfigurationManager.AppSettings["organismoCertificador"];

            //Verificar se existe o arquivo de OrganismoCerificador            
            if (!File.Exists(organismoCertificadorPath))
            {
                Console.WriteLine("Obtendo Organismo Certificador");
                OrganismoCerificadorScraping.ObterOrganismoCertificador();
            }

            Console.WriteLine("Lista de Organismo Certificador já existe");
            var listaOrganismoCerificador = JsonFileManager.Read<List<OrganismoCertificador>>(organismoCertificadorPath);
            var listaAux = listaOrganismoCerificador.Select(item => item).ToList();
            foreach (var item in listaOrganismoCerificador)
            {
                Console.WriteLine($"Obtendo empresas do certificador {item.Descricao}");
                EmpresaScraping.Obter(item);
                listaAux.Remove(item);
                JsonFileManager.Write(listaAux, organismoCertificadorPath);
            }
        }

        static void ConsultaSiteVelho()
        {
            //1: Obter Organismos certificadores (criar json com resultado)
            //2: Buscar empresas do organismo certificador (criar json com resultado)

            var organismoAcreditadoPath = ConfigurationManager.AppSettings["organismoAcreditado"];

            //Verificar se existe o arquivo de OrganismoCerificador            
            if (!File.Exists(organismoAcreditadoPath))
            {
                Console.WriteLine("Obtendo Organismo Acreditado");
                new OrganismoAcreditadoScraping().ObterOrganismo("sigla_certificador", organismoAcreditadoPath);
            }

            Console.WriteLine("Lista de Organismo Certificador já existe");
            var listaOrganismoCerificador = JsonFileManager.Read<List<OrganismoCertificador>>(organismoAcreditadoPath);

            foreach (var item in listaOrganismoCerificador)
            {
                Console.WriteLine($"Obtendo empresas do certificador {item.Descricao}");
                CertificadoScraping.Obter(item);
            }
        }

        static void GerarPlanilha()
        {
            Console.WriteLine("Gerando a planilha");
            var gerarPlanilha = new ExcelFileManager();
            gerarPlanilha.GerarPlilha();
        }

        static void ConsultaSiteVelhoPaginas()
        {
            //1: Obter Organismos certificadores (criar json com resultado)
            //2: Buscar empresas do organismo certificador (criar json com resultado)

            var organismoAcreditadoPath = ConfigurationManager.AppSettings["organismoAcreditado"];

            //Verificar se existe o arquivo de OrganismoCerificador            
            if (!File.Exists(organismoAcreditadoPath))
            {
                Console.WriteLine("Obtendo Organismo Acreditado");
                new OrganismoAcreditadoScraping().ObterOrganismo("sigla_certificador", organismoAcreditadoPath);
            }

            Console.WriteLine("Lista de Organismo Certificador já existe");
            var listaOrganismoCerificador = JsonFileManager.Read<List<OrganismoCertificador>>(organismoAcreditadoPath);
            List<OrganismoCertificador> listaAux = new List<OrganismoCertificador>();
            listaAux = listaOrganismoCerificador.Select(item => item).ToList();

            foreach (var item in listaOrganismoCerificador)
            {
                Console.WriteLine($"Obtendo quantidade de páginas do certificador {item.Descricao}");
                new OrganismoAcreditadoScraping().ObterQuantidadePaginas(item.Valor);
                listaAux.Remove(item);
                JsonFileManager.Write(listaAux, organismoAcreditadoPath);

            }
        }
    }
}