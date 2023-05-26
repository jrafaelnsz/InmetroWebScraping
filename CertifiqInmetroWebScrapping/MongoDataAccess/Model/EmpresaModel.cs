using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.Model
{
    public class EmpresaModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string NomeEmpresa { get; set; }
        public string UnidadeNegocio { get; set; }
        public string UF { get; set; }
        public string PadraoNormativo { get; set; }
        public string NumeroCertificado { get; set; }
        public string OrganismoCertificador { get; set; }
        public string OrganismoAcreditadoCGCRE { get; set; }
        public EscopoModel Escopos { get; set; }
        public string Situacao { get; set; }
    }
}
