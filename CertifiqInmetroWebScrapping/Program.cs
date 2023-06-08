using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.MongoDataAccess.DataAccessLayer;
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
            Console.WriteLine("6 - Consultar por número do certificado ");
            Console.WriteLine("7 - Teste listagem de certificados");
            Console.WriteLine("8 - Internalizar dados para base relacional Mysql");



            var opcao = Console.ReadLine();
            await ExecucaoResilienteAsync(opcao);
        }

        static async Task ExecucaoResilienteAsync(string opcao)
        {
            Console.Clear();

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
                    case "6":
                        ConsultaPorNumeroCertificado();
                        break;
                    case "7":
                        ListarCertificado();
                        break;

                    case "8":
                        internalizarDadosEmMassa();
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

        private static void ListarCertificado()
        {
            var db = new MyMongoDbContext();
            var certificados = new CertificadoRepository(db).ObterCertificados();

            foreach (var item in certificados)
            {
                Console.WriteLine(item.ToString());
            }
        }

        private static void ProcessarHtmlPreCertificado()
        {
            Console.WriteLine("PROCESSANDO HTMLs");
            while (true)
            {
                Console.WriteLine("OBTENDO HTMLs A SEREM PROCESSADOS");
                var task = new MyMongoDbContext().ObterQuantidadeHtmlConvenioAsync();
                task.Wait();

                Console.WriteLine($"HTMLs ENCONTRADOS: {task.Result}");

                for (int i = 0; i < task.Result; i++)
                {
                    Console.WriteLine($"Processando PreCertificado {i+1} de {task.Result}");
                    new PreCertificadoScraping().ProcessarDocumento();
                }

                Spinner("AGUARDANDO CARGA ", 3000);                
            }
        }

        private static async Task ConsultaConveniosAspViaRequestAsync()
        {
            var paginas = new MyMongoDbContext().ObterPaginasConvenio();            

            var paginas2 = paginas.OrderBy(p => p.Paginas).ToList();

            foreach (var item in paginas2)
            {
                Console.WriteLine($"OBTENDO HTMLS DA CERTFIFCADORA {item.CodConvenio.Trim()}");
                var db = new MyMongoDbContext();
                for (int i = 1; i <= item.Paginas; i++)
                {
                    Console.WriteLine($"PAGINA {i} DE {item.Paginas}...");
                    // db.TestConnection();
                    var html = await ChupaCabra.CriarRequisicao(i, item.CodConvenio);

                    var myHtml = new HtmlConvenioModel(item.CodConvenio, i, html);
                    db.SalvarPagina(myHtml);
                    Console.WriteLine("CONCLUIDA!");
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

            while (!DisponibilidadeSite.Verificar())
            {
                Spinner();
            }

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

        static void ConsultaPorNumeroCertificado()
        {
            while (!DisponibilidadeSite.Verificar())
            {
                Spinner();
            }
            
            CertificadoDetalheScraping.Obter();            
        }

        private static void Spinner(string msgPadrao = "SISTEMA INDISPONÍVEL POR FAVOR AGUARDE: ", int maximo = 600)
        {
            Console.Clear();

            var contador = 0;
            var acumuador = 0;

            while (true)
            {                
                Console.Clear();
                switch (contador)
                {
                    case 0:
                        Console.Write($"{msgPadrao}\\");
                        break;
                    case 1:
                        Console.Write($"{msgPadrao}|");
                        break;
                    case 2:
                        Console.Write($"{msgPadrao}/");
                        break;
                    case 3:
                        Console.Write($"{msgPadrao}-");
                        break;
                    default:
                        break;
                }

                Thread.Sleep(100);
                
                contador++;
                acumuador++;

                if (contador == 4)                
                    contador = 0;

                if (acumuador >= maximo)
                    break;
            }
        }

        static void GerarPlanilha()
        {
            Console.WriteLine("Gerando a planilha");
            var gerarPlanilha = new ExcelFileManager();
            gerarPlanilha.GerarPlilha();
        }

        static void internalizarDadosEmMassa()
        {
            Console.WriteLine("internalizando dados");
            var gerarPlanilha = new ExcelFileManager();
            gerarPlanilha.InternalizarDadosEmMassa();
        }

        static void ConsultaSiteVelhoPaginas()
        {
            while (!DisponibilidadeSite.Verificar())
            {
                Spinner();
            }

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