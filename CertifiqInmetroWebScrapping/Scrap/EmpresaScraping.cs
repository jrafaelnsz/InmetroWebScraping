﻿using CertifiqInmetroWebScrapping.Modelo;
using CertifiqInmetroWebScrapping.MongoDataAccess.Repository;
using CertifiqInmetroWebScrapping.Util;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Configuration;

namespace CertifiqInmetroWebScrapping.Scrap
{
    public class EmpresaScraping
    {
        public static void Obter(OrganismoCertificador certificador)
        {
            if (File.Exists($"..//..//{certificador.Valor}.json"))
                return;

         // string chromeDriverPath = @"C:\Users\mcmin\.nuget\packages\selenium.webdriver.chromedriver\113.0.5672.6300\driver\win32\chromedriver.exe";
            string chromeDriverPath = ConfigurationManager.AppSettings["chromeDriver"];


            // Create a new ChromeDriver instance
            var options = new ChromeOptions();
            //options.AddArgument("--headless"); // Optional: Run in headless mode without opening a browser window
            IWebDriver driver = new ChromeDriver(chromeDriverPath, options);
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(3000);

            try
            {
                driver.Navigate().GoToUrl("https://certifiq.inmetro.gov.br/Consulta/ConsultaEmpresas");

                Thread.Sleep(1000);

                IWebElement organismoCertificadorSelect = driver.FindElement(By.Id("IdOrganismo"));

                SelectElement select2 = new SelectElement(organismoCertificadorSelect);

                select2.SelectByValue(certificador.Valor);

                Thread.Sleep(1000);

                IWebElement buscarButton = driver.FindElement(By.Id("Buscar"));
                buscarButton.Click();

                Thread.Sleep(3000);

                IWebElement table = driver.FindElement(By.Id("tableResultado"));
                IList<IWebElement> rows = table.FindElements(By.TagName("tr"));

                var empresas = new List<Empresa>();

                foreach (IWebElement row in rows)
                {
                    IList<IWebElement> columns = row.FindElements(By.TagName("td"));

                    if (columns.Count > 0)
                    {
                        var empresa = new Empresa();
                        var escopo = new Escopo();
                        empresa.NomeEmpresa = columns[0].Text;
                        empresa.UnidadeNegocio = columns[1].Text;
                        empresa.UF = columns[2].Text;
                        empresa.PadraoNormativo = columns[3].Text;

                        AguardaCarregamentoElemento(driver, By.TagName("a"));

                        columns[0].FindElement(By.TagName("a")).Click();

                        Thread.Sleep(2000);

                        AguardaCarregamentoElemento(driver, By.ClassName("panel-body"));

                        IWebElement panel = driver.FindElement(By.ClassName("panel-body"));

                        IList<IWebElement> li = panel.FindElements(By.TagName("li"));

                        foreach (IWebElement item in li)
                        {
                            if (item.Text.Contains("Organismo certificador:"))
                                empresa.OrganismoCertificador = item.Text.Replace("Organismo certificador: ", "");

                            if (item.Text.Contains("Organismo acreditado pela CGCRE:"))
                                empresa.OrganismoAcreditadoCGCRE = item.Text.Replace("Organismo acreditado pela CGCRE: ", "");

                            if (item.Text.Contains("Código Nace:"))
                                escopo.CodigoNace = item.Text.Replace("Código Nace: ", "");

                            if (item.Text.Contains("Detalhe:"))
                                escopo.Detalhe = item.Text.Replace("Detalhe: ", "");
                        }

                        empresa.Escopos = escopo;

                        var situacao = panel.FindElement(By.TagName("p"));

                        if (situacao.Text.Contains("Situação:"))
                            empresa.Situacao = situacao.Text.Replace("Situação: ", "");

                        empresas.Add(empresa);

                        IWebElement closeButton = driver.FindElement(By.ClassName("close"));
                        closeButton.Click();

                        Thread.Sleep(1000);
                    }
                }

                var db = new MyMongoDbContext();

                //JsonFileManager.Write(empresas, $"..//..//{certificador.Valor}.json");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
            finally
            {
                // Close the browser and quit the driver
                driver.Quit();
            }            
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
