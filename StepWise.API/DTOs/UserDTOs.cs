namespace StepWise.API.DTOs
{
    public class UpdateUserDTO
    {
        public string Nome { get; set; } = string.Empty;
    }
    public class UpdatePasswordDTO
    {
        public string SenhaAtual { get; set; } = string.Empty;
        public string NovaSenha { get; set; } = string.Empty;
    }
}