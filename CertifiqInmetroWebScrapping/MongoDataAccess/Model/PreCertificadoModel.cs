using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.Model
{
    public class PreCertificadoModel
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
    }
}
