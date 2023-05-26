using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.MongoDataAccess.Model;
using CertifiqInmetroWebScrapping.MongoDataAccess.Repository;
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
                driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(6000);
                driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(6000);


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

        public void ObterQuantidadePaginas(string selectId)
        {
            var paginas = 0;

            //  string chromeDriverPath = @"C:\Users\mcmin\.nuget\packages\selenium.webdriver.chromedriver\113.0.5672.6300\driver\win32\chromedriver.exe";
            string chromeDriverPath = ConfigurationManager.AppSettings["chromeDriver"];

            // Create a new ChromeDriver instance
            var options = new ChromeOptions();
            //options.AddArgument("--headless"); // Optional: Run in headless mode without opening a browser window
            IWebDriver driver = new ChromeDriver(chromeDriverPath, options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(60000);            
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60000);

            driver.Navigate().GoToUrl("http://www.inmetro.gov.br/prodcert/certificados/lista.asp");                       

            IWebElement organismoCertificadorSelect = driver.FindElement(By.Id("sigla_certificador"));

            SelectElement select2 = new SelectElement(organismoCertificadorSelect);

            select2.SelectByValue(selectId);

            IWebElement buscarButton = driver.FindElement(By.Name("btn_enviar"));            

            try
            {
                buscarButton.Click();
            }
            catch (Exception)
            {
                //engole o choro
                AguardaCarregamentoElemento(driver, By.XPath("/html/body/form/table[3]/tbody/tr[2]/td[2]/table[1]/tbody/tr/td[1]/font[1]/b"));
            }

            IWebElement boldElement = driver.FindElement(By.XPath("/html/body/form/table[3]/tbody/tr[2]/td[2]/table[1]/tbody/tr/td[1]/font[1]/b"));
            var texto = boldElement.Text.Trim();
            paginas = Convert.ToInt32(Math.Ceiling(Convert.ToInt32(texto) / 30.0));

            driver.Quit();

            var db = new MyMongoDbContext();
            db.SalvarPaginaCertificado(new CertificadorPaginaModel(selectId, paginas));

        }

        private static void AguardaCarregamentoElemento(IWebDriver driver, By by)
        {
            var count = 0;
            while (driver.FindElements(by).Count == 0)
            {
                Thread.Sleep(5000);
                count++;

                //depois de 60 * 5 segundos (3 minutos) sem que o elemento seja carregado
                //a exceção é lançada
                if (count >= 60)
                    throw new SystemException("Elemento não carregado");
            }
        }
    }
}
