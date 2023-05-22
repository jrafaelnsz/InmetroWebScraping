using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.Scrap;
using CertifiqInmetroWebScrapping.Scrap.Interface;
using CertifiqInmetroWebScrapping.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Configuration;

namespace CertifiqInmetroWebScrapping
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MontaMenu();
            Console.ReadKey();            
        }

        static void MontaMenu()
        {
            Console.Clear();

            Console.WriteLine("1 - Consultar certifiq.inmetro.gov.br/Consulta/ConsultaEmpresas");
            Console.WriteLine("2 - Consultar inmetro.gov.br/prodcert/certificados/busca.asp?");

            var opc = Console.ReadLine();

            switch (opc)
            {
                case "1":
                    ConsultaSiteNovo();
                    break;
                case "2":
                    ConsultaSiteVelho();
                    break;
                default:
                    MontaMenu();
                    break;
            }
        }

        static void ConsultaSiteNovo() 
        {
            try
            {
                //1: Obter Organismos certificadores (criar json com resultado)
                //2: Buscar empresas do organismo certificador (criar json com resultado)

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

                foreach (var item in listaOrganismoCerificador)
                {
                    Console.WriteLine($"Obtendo empresas do certificador {item.Descricao}");
                    EmpresaScraping.Obter(item);
                }
            }
            catch (Exception ex)
            {

                throw;
            }

        }

        static void ConsultaSiteVelho()
        {
            try
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
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}