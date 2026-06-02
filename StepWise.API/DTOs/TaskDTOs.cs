namespace StepWise.API.DTOs
{
    public class CreateTaskDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prazo { get; set; } = string.Empty;
        public string Prioridade { get; set; } = "Media";
        public List<string> Tags { get; set; } = new();
        public List<CreateStepDTO> Etapas { get; set; } = new();
    }

    public class CreateStepDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }

    public class UpdateTaskDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prazo { get; set; } = string.Empty;
        public string Prioridade { get; set; } = "Media";
        public List<string> Tags { get; set; } = new();
    }

    public class UpdateStepDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
    }
    }
    public class TaskDetailsDTO
    {
        public string Id { get; set; } = string.Empty;
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public string Prazo { get; set; } = string.Empty;
        public string Prioridade { get; set; } = "Media";
        public List<string> Tags { get; set; } = new();
        public List<StepDetailsDTO> Etapas { get; set; } = new();
        public DateTime DataCriacao { get; set; }
        public bool Concluida { get; set; }
    }

    public class StepDetailsDTO
    {
        public string Titulo { get; set; } = string.Empty;
        public string Descricao { get; set; } = string.Empty;
        public bool Concluida { get; set; }
    }

