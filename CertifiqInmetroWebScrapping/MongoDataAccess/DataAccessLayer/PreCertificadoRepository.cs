using CertifiqInmetroWebScrapping.MongoDataAccess.Model;
using CertifiqInmetroWebScrapping.MongoDataAccess.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.DataAccessLayer
{
    public class PreCertificadoRepository
    {
        private readonly IMongoCollection<PreCertificadoModel> _preCertificadoCollection;

        public PreCertificadoRepository(MyMongoDbContext dbContext)
        {
            _preCertificadoCollection = dbContext.PreCertificado;
        }

        public void Add(List<PreCertificadoModel> lista)
        {
            _preCertificadoCollection.InsertMany(lista);
        }

        public PreCertificadoModel ObterPreCertificado()
        {
            var firstDocument = _preCertificadoCollection.Find(new BsonDocument()).FirstOrDefault();
            return firstDocument;
        }

        public List<PreCertificadoModel> ObterPreCertificados(int limite=10)
        {
            var documents = _preCertificadoCollection.Find(x => true).Limit(limite).ToList();
            return documents;
        }

        public long ObterQuantidadePreCertificados()
        {
            var documents = _preCertificadoCollection.EstimatedDocumentCount();
            return documents;
        }

        public bool Delete(PreCertificadoModel preCertificado)
        {
            // Define a filter to specify the document(s) to delete
            FilterDefinition<PreCertificadoModel> filter = Builders<PreCertificadoModel>.Filter.Eq("_id", ObjectId.Parse(preCertificado.Id));

            // Delete a single document that matches the filter
            DeleteResult result = _preCertificadoCollection.DeleteOne(filter);

            return result.DeletedCount > 0;
        }
    }
}
