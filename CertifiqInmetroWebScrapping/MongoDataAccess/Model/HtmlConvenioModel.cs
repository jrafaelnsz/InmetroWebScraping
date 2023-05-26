using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CertifiqInmetroWebScrapping.MongoDataAccess.Model
{
    [BsonIgnoreExtraElements]
    public class HtmlConvenioModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [DataMember]
        public string CodConvenio { get; set; }
        [DataMember]
        public int Pagina { get; set; }
        [DataMember]
        public string Html { get; set; }
        [DataMember]
        public bool Processada { get; set; }

        public HtmlConvenioModel(string codConvenio, int pagina, string html, bool processada = false)
        {
            CodConvenio = codConvenio;
            Pagina = pagina;
            Html = html;
            Processada = processada;
        }
    }
}
