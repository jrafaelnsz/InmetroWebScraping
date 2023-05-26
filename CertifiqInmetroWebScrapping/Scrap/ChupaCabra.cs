
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;


namespace CertifiqInmetroWebScrapping.Scrap
{
    public class ChupaCabra
    {
        public static async Task<string> CriarRequisicao(int pagina, string codCertificador)
        {
            string? retorno = string.Empty;

            var clientHandler = new HttpClientHandler
            {
                UseCookies = false,
            };
            var client = new HttpClient(clientHandler);
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri("http://www.inmetro.gov.br/prodcert/certificados/lista.asp"),
                Headers =
                {
                    { "cookie", "ASPSESSIONIDSCBBDQAA=CHEAGOACIAKGMECMFEJICOOG; ASPSESSIONIDSCAABSAA=DPBCMIPDMIJGCGEENGEOJKAH" },
                },
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "sigla_certificador", codCertificador },
                    { "pagina",pagina.ToString() },
                }),
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                retorno = await response.Content.ReadAsStringAsync();                                
            }

            return retorno; 

        }
    }
}
