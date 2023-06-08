using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CertifiqInmetroWebScrapping.MongoDataAccess.DataAccessLayer;
using CertifiqInmetroWebScrapping.MongoDataAccess.Model;
using CertifiqInmetroWebScrapping.MongoDataAccess.Repository;
using CertifiqInmetroWebScrapping.Scrap;
using CertifiqInmetroWebScrapping.Util;
using CertifiqInmetroWebScrapping.Modelo;
using Spire.Xls;
using System.Configuration;
using System.Runtime.ConstrainedExecution;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using MySqlConnector;
using System.Linq.Expressions;
using Dapper;

namespace CertifiqInmetroWebScrapping.Util
{
    public class ExcelFileManager
    {
        public void GerarPlilha(){
            
            var db = new MyMongoDbContext();
            var Certificados = new CertificadoRepository(db).ObterCertificados();
            //Create a Workbook object
            Workbook workbook = new Workbook();
            //Remove default worksheets
            workbook.Worksheets.Clear();
            //Add a worksheet and name it

                Worksheet worksheet = workbook.Worksheets.Add("Empresas Certificadas");
                //Write data to specific cells
                var PegaCampos = typeof(Certificado).GetProperties();
                for (int i = 0; i < PegaCampos.Length; i++)
                {
                    string teste = PegaCampos[i].Name;
                    i++;
                    worksheet.Range[1, i].Value = teste;
                    i--;
                }

                for (int i = 2; i < Certificados.Count; i++)
                {
                    worksheet.Range[i, 1].Value = Certificados[i].Certificador;
                    worksheet.Range[i, 2].Value = Certificados[i].NumeroCertificado;
                    worksheet.Range[i, 3].Value = Certificados[i].Tipo;
                    worksheet.Range[i, 4].Value = Certificados[i].Emissao;
                    worksheet.Range[i, 5].Value = Certificados[i].Validade;
                    worksheet.Range[i, 6].Value = Certificados[i].StatusCertificado;
                    worksheet.Range[i, 7].Value = Certificados[i].DocNormativo;
                    worksheet.Range[i, 8].Value = Certificados[i].CpfCnpj;
                    worksheet.Range[i, 9].Value = Certificados[i].RazaoSocialNome;
                    worksheet.Range[i, 10].Value = Certificados[i].NomeFantasia;
                    worksheet.Range[i, 11].Value = Certificados[i].Endereco;
                    worksheet.Range[i, 12].Value = Certificados[i].Status;
                    worksheet.Range[i, 13].Value = Certificados[i].PapelEmpresa;
                }
                //Auto fit column width
                worksheet.AllocatedRange.AutoFitColumns();          
            //Save to an Excel file
            workbook.SaveToFile(ConfigurationManager.AppSettings["planilha"], ExcelVersion.Version2016);

           Console.WriteLine("Planilha gerada com sucesso");
        }
        public void InternalizarDadosEmMassa()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["certificadosScrapping"].ConnectionString;

            var db = new MyMongoDbContext();
            var CertificadosMD = new CertificadoRepository(db).ObterCertificados();

            using (var conexaoBD = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {


                conexaoBD.Execute(@"insert into CertificadoDB(Id
                                                       ,Certificador
                                                       ,NumeroCertificado
                                                       ,Tipo
                                                       ,Emissao
                                                       ,Validade
                                                       ,StatusCertificado
                                                       ,DocNormativo
                                                       ,CpfCnpj
                                                       ,RazaoSocialNome
                                                       ,NomeFantasia
                                                       ,Endereco
                                                       ,Status
                                                       ,PapelEmpresa) values 
                                            (@Id
                                            ,@Certificador
                                            ,@NumeroCertificado
                                            ,@Tipo
                                            ,@Emissao,@Validade,@StatusCertificado
                                            ,@DocNormativo
                                            ,@CpfCnpj
                                            ,@RazaoSocialNome
                                            ,@NomeFantasia
                                            ,@Endereco
                                            ,@Status
                                            ,@PapelEmpresa)", CertificadosMD);
            }

            Console.WriteLine("Dados internalizados com sucesso");
        }

    
    }
}
