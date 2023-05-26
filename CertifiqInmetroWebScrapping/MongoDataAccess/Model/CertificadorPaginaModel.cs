
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.Model
{
    public class CertificadorPaginaModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string CodConvenio { get; set; }
        public int Paginas { get; set; }

        public CertificadorPaginaModel(string codConvenio, int paginas)
        {
            CodConvenio = codConvenio;
            Paginas = paginas;  
        }
    }

}
