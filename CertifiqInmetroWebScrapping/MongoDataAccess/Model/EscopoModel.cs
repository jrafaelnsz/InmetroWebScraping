
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.Model
{
    public class EscopoModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CodigoNace { get; set; }
        public string Detalhe { get; set; }
    }
}
