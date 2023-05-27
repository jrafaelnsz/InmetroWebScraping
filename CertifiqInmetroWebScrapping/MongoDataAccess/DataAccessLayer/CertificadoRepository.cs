using CertifiqInmetroWebScrapping.MongoDataAccess.Model;
using CertifiqInmetroWebScrapping.MongoDataAccess.Repository;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.DataAccessLayer
{
    public class CertificadoRepository
    {
        private readonly IMongoCollection<CertificadoModel> _certificadoCollection;

        public CertificadoRepository(MyMongoDbContext dbContext)
        {
            _certificadoCollection = dbContext.Certificado;
        }

        public void AddCertificados(List<CertificadoModel> lista)
        {
            _certificadoCollection.InsertMany(lista);
        }

        public List<CertificadoModel> ObterCertificados()
        {
            var documents = _certificadoCollection.AsQueryable().ToList();
            return documents;
        }
    }
}
