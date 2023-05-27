using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CertifiqInmetroWebScrapping.Scrap
{
    public class DisponibilidadeSite
    {
        public static bool Verificar()
        {
            try
            {
                Console.Clear();
                Console.WriteLine("VERFICANDO DISPONIBILIDADE...");
                Thread.Sleep(1000);

                var html = @"http://www.inmetro.gov.br/prodcert/certificados/busca.asp";

                HtmlWeb web = new HtmlWeb();

                var htmlDoc = web.Load(html);

                var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");

                Console.Clear();

                return !(node.InnerText.Equals("Server Application Error"));                
            }
            catch (Exception)
            {
                return false;                
            }
        }
    }
}
