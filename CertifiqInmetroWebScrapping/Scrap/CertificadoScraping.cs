using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace CertifiqInmetroWebScrapping.Scrap
{
    public class CertificadoScraping
    {
        public static void Obter(OrganismoCertificador certificador)
        {
            string chromeDriverPath = @"C:\Users\mcmin\.nuget\packages\selenium.webdriver.chromedriver\113.0.5672.6300\driver\win32\chromedriver.exe";

            // Create a new ChromeDriver instance
            var options = new ChromeOptions();
            //options.AddArgument("--headless"); // Optional: Run in headless mode without opening a browser window
            IWebDriver driver = new ChromeDriver(chromeDriverPath, options);

            driver.Navigate().GoToUrl("http://www.inmetro.gov.br/prodcert/certificados/lista.asp");

            Thread.Sleep(2000);

            IWebElement organismoCertificadorSelect = driver.FindElement(By.Id("sigla_certificador"));

            SelectElement select2 = new SelectElement(organismoCertificadorSelect);

            select2.SelectByValue(certificador.Valor);

            Thread.Sleep(1000);

            IWebElement buscarButton = driver.FindElement(By.Name("btn_enviar"));
            buscarButton.Click();

            Thread.Sleep(3000);

            var paginas = 1;

            if (driver.FindElements(By.XPath("//img[contains(@src,'ultima')]")).Count > 0)
            {
                IWebElement img = driver.FindElement(By.XPath("//img[contains(@src,'ultima')]"));

                var onclick = img.GetAttribute("onclick").Split(",");
                paginas = onclick.Length > 0 ? Convert.ToInt32(onclick[0].Replace("Pagina(", "")) : paginas;

                for (int i = 1; i <= paginas; i++)
                {
                    BuscaSimples(driver, certificador, i);

                    if (i < paginas)
                        ProximaPagina(driver, i + 1);
                }
            }
            else
                BuscaSimples(driver, certificador);

            // Close the browser and quit the driver
            driver.Quit();
        }

        static void ProximaPagina(IWebDriver driver, int pagina)
        {
            IWebElement proximo = driver.FindElement(By.XPath("//*[@id=\"form\"]/table[3]/tbody/tr[2]/td[2]/table[3]/tbody/tr[1]/td[3]/span"));
            var onclick = proximo.GetAttribute("onclick").Split(",");
            var lote = onclick.Length > 0 ? Convert.ToInt32(onclick[0].Replace("Pagina(", "")) : pagina;

            if (pagina == lote)
                proximo.Click();
            else
                driver.FindElement(By.XPath($"/html/body/form/table[3]/tbody/tr[2]/td[2]/table[3]/tbody/tr[1]/td[2]/span[{pagina}]")).Click();
                        
            Thread.Sleep(3000);
        }

        static void BuscaSimples(IWebDriver driver, OrganismoCertificador certificador, int pagina = 1)
        {
            if (File.Exists($"c:\\temp\\certificados\\{certificador.Valor.Trim()}_{pagina}.json"))
                return;

            var tables = driver.FindElements(By.XPath("/html/body/form/table[3]/tbody/tr[2]/td[2]/table[2]/tbody/tr/td/table/tbody/tr/td/table"));
            var certificados = new List<Certificado>();

            foreach (var table in tables)
            {
                var certificado = new Certificado();

                IList<IWebElement> strong = table.FindElements(By.TagName("strong"));

                if (strong.Count > 0)
                {
                    var list = new List<string>();
                    list = strong[0].Text.Split(":").ToList();

                    if (list.Count == 7)
                    {
                        certificado.Certificador = list[1].Replace("Nº Certificado", "").Trim();
                        certificado.NumeroCertificado = list[2].Replace("Tipo", "").Trim();
                        certificado.Tipo = list[3].Replace("Emissão", "").Trim();
                        certificado.Emissao = list[4].Replace("Validade", "").Trim();
                        certificado.Validade = list[5].Replace("Status do Certificado", "").Trim();
                        certificado.StatusCertificado = list[6].Trim();                        
                    }

                    IList<IWebElement> colunas = table.FindElements(By.ClassName("listagem"));

                    if (colunas.Count > 0)
                    {
                        if (long.TryParse(colunas[1].Text, out _))
                        {
                            certificado.CpfCnpj = colunas[1].Text;
                            certificado.RazaoSocialNome = colunas[2].Text;
                            certificado.NomeFantasia = colunas[3].Text;
                            certificado.Endereco = colunas[4].Text;
                            certificado.Status = colunas[5].Text;
                            certificado.PapelEmpresa = colunas[6].Text;
                        }
                    }

                    strong[1].Click();
                    Thread.Sleep(1000);

                    IWebElement iframe = table.FindElement(By.TagName("iframe"));
                    driver.SwitchTo().Frame(iframe);

                    certificado.DocNormativo = driver.FindElement(By.Id("txtRetornaDoc")).GetAttribute("value");

                    driver.SwitchTo().ParentFrame();


                    certificados.Add(certificado);
                }
            }

            if (certificados.Count > 0)
                SalvarArquivo(certificador, certificados, pagina);
        }

        static void SalvarArquivo(OrganismoCertificador certificador, List<Certificado> certificados, int pagina)
        {
            JsonFileManager.Write(certificados, $"c:\\temp\\certificados\\{certificador.Valor.Trim()}_{pagina}.json");
        }
    }
}
