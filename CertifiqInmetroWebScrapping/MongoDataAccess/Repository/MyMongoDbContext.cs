using MongoDB.Driver;
using MongoDB.Bson;
using CertifiqInmetroWebScrapping.MongoDataAccess.Model;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.Repository
{
    public class MyMongoDbContext
    {
        private const string connectionString = "";
        private const string databaseName = "DataScraper";        
        private const string HtmlCollection = "HtmlConvenio";
        private const string CertificadorPaginaCollection = "CertificadorPagina";
        private const string preCertificadoCollection = "PreCertificado";
        private const string CertificadoCollection = "Certificado";
        private readonly IMongoDatabase _database;

        public MyMongoDbContext()
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public IMongoCollection<PreCertificadoModel> PreCertificado => _database.GetCollection<PreCertificadoModel>(preCertificadoCollection);
        public IMongoCollection<CertificadoModel> Certificado => _database.GetCollection<CertificadoModel>(CertificadoCollection);

        public IMongoCollection<T> Connect<T>(in string collection)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);
            return db.GetCollection<T>(collection);
        }

        public async Task SalvarPagina(HtmlConvenioModel htmlConvenio)
        {
            var colecao = Connect<HtmlConvenioModel>(HtmlCollection);
            await colecao.InsertOneAsync(document: htmlConvenio);
        }

        public async Task SalvarPaginaCertificado(CertificadorPaginaModel certificadorPagina)
        {
            var colecao = Connect<CertificadorPaginaModel>(CertificadorPaginaCollection);
            await colecao.InsertOneAsync(document: certificadorPagina);
        }

        public List<CertificadorPaginaModel> ObterPaginasConvenio()
        {
            var colecao = Connect<CertificadorPaginaModel>(CertificadorPaginaCollection);            
            List<CertificadorPaginaModel> minhaLista = colecao.AsQueryable().ToList();
            return minhaLista;
        }

        public HtmlConvenioModel ObterHtmlConvenio()
        {
            var colecao = Connect<HtmlConvenioModel>(HtmlCollection);
            var firstDocument = colecao.Find(new BsonDocument()).FirstOrDefault();
            return firstDocument;
        }

        public async Task<long> ObterQuantidadeHtmlConvenioAsync()
        {
            var colecao = Connect<HtmlConvenioModel>(HtmlCollection);
            return await colecao.EstimatedDocumentCountAsync();
        }

        public bool DeleteHtmlConvenio(HtmlConvenioModel htmlConvenio)
        {
            var colecao = Connect<HtmlConvenioModel>(HtmlCollection);

            // Define a filter to specify the document(s) to delete
            FilterDefinition<HtmlConvenioModel> filter = Builders<HtmlConvenioModel>.Filter.Eq("_id", ObjectId.Parse(htmlConvenio.Id));

            // Delete a single document that matches the filter
            DeleteResult result = colecao.DeleteOne(filter);

            return result.DeletedCount > 0;            

        }

        public bool DeletePaginaConsultada(CertificadorPaginaModel certificadorPagina)
        {
            var colecao = Connect<CertificadorPaginaModel>(CertificadorPaginaCollection);

            // Define a filter to specify the document(s) to delete
            FilterDefinition<CertificadorPaginaModel> filter = Builders<CertificadorPaginaModel>.Filter.Eq("_id", ObjectId.Parse(certificadorPagina.Id));

            // Delete a single document that matches the filter
            DeleteResult result = colecao.DeleteOne(filter);

            return result.DeletedCount > 0;
        }

        public async Task SalvarPreCertificados(List<PreCertificadoModel> lista)
        {
            var colecao = Connect<PreCertificadoModel>(preCertificadoCollection);
            await colecao.InsertManyAsync(documents: lista);
        }

        public void TestConnection()
        {
            var settings = MongoClientSettings.FromConnectionString(connectionString);
            // Set the ServerApi field of the settings object to Stable API version 1
            settings.ServerApi = new ServerApi(ServerApiVersion.V1);
            // Create a new client and connect to the server
            var client = new MongoClient(settings);
            // Send a ping to confirm a successful connection
            try
            {
                var result = client.GetDatabase("admin").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
                Console.WriteLine("Pinged your deployment. You successfully connected to MongoDB!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
