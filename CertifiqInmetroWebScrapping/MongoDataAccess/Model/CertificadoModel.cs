using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.Model
{
    public class CertificadoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Certificador { get; set; }
        public string NumeroCertificado { get; set; }
        public string Tipo { get; set; }
        public string Emissao { get; set; }
        public string Validade { get; set; }
        public string StatusCertificado { get; set; }
        public string DocNormativo { get; set; }
        public string CpfCnpj { get; set; }
        public string RazaoSocialNome { get; set; }
        public string NomeFantasia { get; set; }
        public string Endereco { get; set; }
        public string Status { get; set; }
        public string PapelEmpresa { get; set; }
    }
}
