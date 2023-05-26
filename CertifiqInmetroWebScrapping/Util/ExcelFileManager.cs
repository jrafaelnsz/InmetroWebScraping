using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CertifiqInmetroWebScrapping.Modelo;
using Spire.Xls;
using System.Configuration;


namespace CertifiqInmetroWebScrapping.Util
{
    public class ExcelFileManager
    {

        public void GerarPlilha(){
            
            List<Certificado> Certificados = new List<Certificado>();
            List<Certificado> CertificadosSoma = new List<Certificado>();

            string caminhoDasPastas = ConfigurationManager.AppSettings["certificadosPasta"];
            List<string> list = new List<string>();

            List<string> paths = Directory.EnumerateFiles(caminhoDasPastas, "*.json").ToList();

            foreach (string filePath in paths)
            {
                using (StreamReader r = new StreamReader(filePath))
                {
                    string json = r.ReadToEnd();
                    Certificados = System.Text.Json.JsonSerializer.Deserialize<List<Certificado>>(json);
                }
                foreach (var certificado in Certificados)
                {
                    Certificado item = new Certificado()
                    {

                        Certificador = certificado.Certificador,
                        NumeroCertificado = certificado.NumeroCertificado,
                        Tipo = certificado.Tipo,
                        Emissao = certificado.Emissao,
                        Validade = certificado.Validade,
                        StatusCertificado = certificado.StatusCertificado,
                        DocNormativo = certificado.DocNormativo,
                        CpfCnpj = certificado.CpfCnpj,
                        RazaoSocialNome = certificado.RazaoSocialNome,
                        NomeFantasia = certificado.NomeFantasia,
                        Endereco = certificado.Endereco,
                        Status = certificado.Status,
                        PapelEmpresa = certificado.PapelEmpresa
                    };
                    CertificadosSoma.Add(item);
                }

            }

            string noneInicial = "";
            List<string> certificadora = new List<string>();
            foreach (var item in CertificadosSoma)
            {
                if (noneInicial != item.Certificador)
                {
                    noneInicial = item.Certificador;
                    certificadora.Add(item.Certificador);
                }

            }
            //Create a Workbook object
            Workbook workbook = new Workbook();
            //Remove default worksheets
            workbook.Worksheets.Clear();
            //Add a worksheet and name it

            foreach (var item in certificadora)
            {

                Worksheet worksheet = workbook.Worksheets.Add(item);
                //Write data to specific cells
                var PegaCampos = typeof(Certificado).GetProperties();
                for (int i = 0; i < PegaCampos.Length; i++)
                {
                    string teste = PegaCampos[i].Name;
                    i++;
                    worksheet.Range[1, i].Value = teste;
                    i--;
                }

                List<Certificado> destination = CertificadosSoma.Where(x => x.Certificador == item).Select(item => new Certificado
                {
                    Certificador = item.Certificador,
                    NumeroCertificado = item.NumeroCertificado,
                    Tipo = item.Tipo,
                    Emissao = item.Emissao,
                    Validade = item.Validade,
                    StatusCertificado = item.StatusCertificado,
                    DocNormativo = item.DocNormativo,
                    CpfCnpj = item.CpfCnpj,
                    RazaoSocialNome = item.RazaoSocialNome,
                    NomeFantasia = item.NomeFantasia,
                    Endereco = item.Endereco,
                    Status = item.Status,
                    PapelEmpresa = item.PapelEmpresa

                }).ToList();
                for (int i = 2; i < destination.Count; i++)
                {
                    worksheet.Range[i, 1].Value = destination[i].Certificador;
                    worksheet.Range[i, 2].Value = destination[i].NumeroCertificado;
                    worksheet.Range[i, 3].Value = destination[i].Tipo;
                    worksheet.Range[i, 4].Value = destination[i].Emissao;
                    worksheet.Range[i, 5].Value = destination[i].Validade;
                    worksheet.Range[i, 6].Value = destination[i].StatusCertificado;
                    worksheet.Range[i, 7].Value = destination[i].DocNormativo;
                    worksheet.Range[i, 8].Value = destination[i].CpfCnpj;
                    worksheet.Range[i, 9].Value = destination[i].RazaoSocialNome;
                    worksheet.Range[i, 10].Value = destination[i].NomeFantasia;
                    worksheet.Range[i, 11].Value = destination[i].Endereco;
                    worksheet.Range[i, 12].Value = destination[i].Status;
                    worksheet.Range[i, 13].Value = destination[i].PapelEmpresa;
                }

                //Auto fit column width
                worksheet.AllocatedRange.AutoFitColumns();
            }


            //Save to an Excel file
            workbook.SaveToFile(ConfigurationManager.AppSettings["planilha"], ExcelVersion.Version2016);


        }
    }
}
