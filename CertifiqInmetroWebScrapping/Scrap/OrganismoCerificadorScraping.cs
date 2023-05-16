using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace CertifiqInmetroWebScrapping.Scrap
{
    public class OrganismoCerificadorScraping
    {
        public static void ObterOrganismoCertificador()
        {
            var listaOrganismoCerificador = new List<OrganismoCertificador>();

            string chromeDriverPath = @"C:\Users\mcmin\.nuget\packages\selenium.webdriver.chromedriver\113.0.5672.6300\driver\win32\chromedriver.exe";

            // Create a new ChromeDriver instance
            var options = new ChromeOptions();
            //options.AddArgument("--headless"); // Optional: Run in headless mode without opening a browser window
            IWebDriver driver = new ChromeDriver(chromeDriverPath, options);
            
            driver.Navigate().GoToUrl("https://certifiq.inmetro.gov.br/Consulta/ConsultaEmpresas");
            
            IWebElement organismoCertificadorSelect = driver.FindElement(By.Id("IdOrganismo"));
            SelectElement select = new SelectElement(organismoCertificadorSelect);

            foreach (var item in select.Options)
            {
                if (!string.IsNullOrEmpty(item.GetAttribute("value")))
                    listaOrganismoCerificador.Add(new OrganismoCertificador(item.GetAttribute("value"), item.Text));
            }

            JsonFileManager.Write(listaOrganismoCerificador, "c:\\temp\\OrganismoCerificador.json");
            driver.Quit();
        }
    }
}
