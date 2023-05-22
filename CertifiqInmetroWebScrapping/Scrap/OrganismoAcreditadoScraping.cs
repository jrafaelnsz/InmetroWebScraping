using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.Scrap.Interface;
using CertifiqInmetroWebScrapping.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Configuration;

namespace CertifiqInmetroWebScrapping.Scrap
{
    public class OrganismoAcreditadoScraping : IOrganismoScraping
    {
        public void ObterOrganismo(string selectId, string filePath)
        {
            try
            {
                var listaOrganismoCerificador = new List<OrganismoCertificador>();

                //string chromeDriverPath = @"C:\Users\mcmin\.nuget\packages\selenium.webdriver.chromedriver\113.0.5672.6300\driver\win32\chromedriver.exe";
                string chromeDriverPath = ConfigurationManager.AppSettings["chromeDriver"];



                // Create a new ChromeDriver instance
                var options = new ChromeOptions();
                //options.AddArgument("--headless"); // Optional: Run in headless mode without opening a browser window
                IWebDriver driver = new ChromeDriver(chromeDriverPath, options);

                driver.Navigate().GoToUrl("http://www.inmetro.gov.br/prodcert/certificados/busca.asp?");

                IWebElement organismoCertificadorSelect = driver.FindElement(By.Id(selectId));
                SelectElement select = new SelectElement(organismoCertificadorSelect);

                foreach (var item in select.Options)
                {
                    if (!string.IsNullOrEmpty(item.GetAttribute("value")))
                        listaOrganismoCerificador.Add(new OrganismoCertificador(item.GetAttribute("value"), item.Text));
                }

                JsonFileManager.Write(listaOrganismoCerificador, filePath);
                driver.Quit();
            }
            catch (Exception ex)
            {

                throw;
            }
           
        }
    }
}
