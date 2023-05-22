using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spire.Xls;


namespace CertifiqInmetroWebScrapping.Util
{
    public class ExcelFileManager
    {

        public void GerarPlilha(string[,] conteudo,string caminho) {

            Workbook workbook = new Workbook();
            //Remove default worksheets
            workbook.Worksheets.Clear();
            //Add a worksheet and name it
            Worksheet worksheet = workbook.Worksheets.Add("InsertArrays");
            //Create a one-dimensional array
           // string[] oneDimensionalArray = new string[] { "January", "February", "March", "April", "May", "June" };
           // //Write the array to the first row of the worksheet
           // worksheet.InsertArray(oneDimensionalArray, 1, 1, false);
           // //Create a two-dimensional array
           // string[,] twoDimensionalArray = new string[,]{
           //    {"Name", "Age", "Sex", "Dept.", "Tel."},
           //    {"John", "25", "Male", "Development","654214"},
           //    {"Albert", "24", "Male", "Support","624847"},
           //    {"Amy", "26", "Female", "Sales","624758"}
           //};
            //Write the array to the worksheet starting from the cell A3
            worksheet.InsertArray(conteudo, 3, 1);
            //Auto fit column width in the located range
            worksheet.AllocatedRange.AutoFitColumns();
            //Save to an Excel file
            workbook.SaveToFile("E:\\InsertArrays.xlsx", ExcelVersion.Version2016);


        }
    }
}
