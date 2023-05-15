namespace HCL.IdentityServer.API.Domain.DTO
{
    public class LogDTO
    {
        public string Message { get; set; } = "";
        public int StatusCode { get; set; } = 0;
        public bool IsSuccess { get; set; }
        public string ExecutiedMethod { get; }

        public LogDTO(string message, int statusCode, string executedMethod, bool isSuccess)
        {
            Message = message;
            StatusCode = statusCode;
            ExecutiedMethod = executedMethod;
            IsSuccess = isSuccess;
        }

        public LogDTO()
        {
        }

        public LogDTO(string executedMethod)
        {
            ExecutiedMethod = executedMethod;
        }
    }
}