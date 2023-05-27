using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.MongoDataAccess.DataAccessLayer;
using CertifiqInmetroWebScrapping.MongoDataAccess.Model;
using CertifiqInmetroWebScrapping.MongoDataAccess.Repository;
using CertifiqInmetroWebScrapping.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Configuration;


namespace CertifiqInmetroWebScrapping.Scrap
{
    public class CertificadoDetalheScraping
    {
        public static void Obter()
        {
            var dbContext = new MyMongoDbContext();
            var repositorio = new PreCertificadoRepository(dbContext);

            while (repositorio.ObterQuantidadePreCertificados() > 0)
            {
                var preCertificados = repositorio.ObterPreCertificados();

                foreach (var preCertificado in preCertificados)
                {
                    #region Instância do Chrome
                    string chromeDriverPath = ConfigurationManager.AppSettings["chromeDriver"];

                    var options = new ChromeOptions();
                    //options.AddArgument("--headless"); // Optional: Run in headless mode without opening a browser window
                    IWebDriver driver = new ChromeDriver(chromeDriverPath, options);
                    #endregion

                    try
                    {
                        #region Navega para o site do inmetro
                        driver.Navigate().GoToUrl("http://www.inmetro.gov.br/prodcert/certificados/lista.asp");

                        Thread.Sleep(2000);
                        #endregion

                        #region Preenche Numero do certificado
                        IWebElement numeroCertificado = driver.FindElement(By.Id("num_certificado"));
                        numeroCertificado.SendKeys(preCertificado.NumeroCertificado);
                        #endregion

                        #region Seleciona a opção Organismo acreditado
                        IWebElement organismoCertificadorSelect = driver.FindElement(By.Id("sigla_certificador"));

                        SelectElement select2 = new SelectElement(organismoCertificadorSelect);

                        select2.SelectByValue(preCertificado.Certificador.PadRight(8));
                        #endregion

                        #region Clica no botão de busca
                        IWebElement buscarButton = driver.FindElement(By.Name("btn_enviar"));
                        buscarButton.Click();
                        Thread.Sleep(3000);
                        #endregion

                        #region Realiza a captura dos dados na tela
                        BuscaSimples(driver, preCertificado);
                        #endregion

                        #region Remove PreCertificado da base 
                        if(repositorio.Delete(preCertificado))
                        {
                            dbContext = new MyMongoDbContext();
                            repositorio = new PreCertificadoRepository(dbContext);
                        }
                        #endregion

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw;
                    }
                    finally
                    {
                        driver.Quit();
                    }
                }
            }            
        }

        static void BuscaSimples(IWebDriver driver, PreCertificadoModel certificador, int pagina = 1)
        {
            var tables = driver.FindElements(By.XPath("/html/body/form/table[3]/tbody/tr[2]/td[2]/table[2]/tbody/tr/td/table/tbody/tr/td/table"));
            var certificados = new List<CertificadoModel>();

            foreach (var table in tables)
            {
                var certificado = new CertificadoModel();

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
                SalvarCertificado(certificados);
        }

        static void SalvarCertificado(List<CertificadoModel> certificados)
        {
            var dbContext = new MyMongoDbContext();
            var repositorio = new CertificadoRepository(dbContext);
            repositorio.AddCertificados(certificados);            
        }

    }
}
