namespace HCL.IdentityServer.API.Domain.DTO.Builders
{
    public class LogDTOBuidlder
    {
        private LogDTO _Instance;
        private LogDTOBuidlder _InstanceBuilder;

        public LogDTOBuidlder(string executingMethod)
        {
            _Instance = new LogDTO(executingMethod);
            _InstanceBuilder = this;
        }

        public LogDTOBuidlder BuildMessage(string message)
        {
            _Instance.Message = message;

            return _InstanceBuilder;
        }

        public LogDTOBuidlder BuildStatusCode(int code)
        {
            _Instance.StatusCode = code;

            return _InstanceBuilder;
        }

        public LogDTOBuidlder BuildSuccessState(bool state)
        {
            _Instance.IsSuccess = state;

            return _InstanceBuilder;
        }

        public LogDTO Build() => _Instance;
    }
}
