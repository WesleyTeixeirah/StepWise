using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Linq;

namespace StepWise.API.Models
{
    public class TaskItem
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = string.Empty;

        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [BsonElement("prazo")]
        public string Prazo { get; set; } = string.Empty;

        [BsonElement("prioridade")]
        public string Prioridade { get; set; } = "Media";

        [BsonElement("tags")]
        public List<string> Tags { get; set; } = new();

        [BsonElement("etapas")]
        public List<Step> Etapas { get; set; } = new();

        [BsonElement("criadoEm")]
        public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

         [BsonIgnore]
        public int Progresso
        {
            get
            {
                if (Etapas == null || Etapas.Count == 0)
                    return 0;

                int concluidas = Etapas.Count(e => e.Concluida);
                return (int)Math.Round((double)concluidas / Etapas.Count * 100);
            }
        }
    

    }

    public class Step
    {
        [BsonElement("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [BsonElement("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [BsonElement("concluida")]
        public bool Concluida { get; set; } = false;
    }
}