using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.Scrap;
using CertifiqInmetroWebScrapping.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CertifiqInmetroWebScrapping
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Clear();

            //1: Obter Organismos certificadores (criar json com resultado)
            //2: Buscar empresas do organismo certificador (criar json com resultado)

            var organismoCertificadorPath = "c:\\temp\\OrganismoCerificador.json";
            
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
                EmpresaScrapping.Obter(item);
            }

            Console.ReadKey();

            
        }
    }
}