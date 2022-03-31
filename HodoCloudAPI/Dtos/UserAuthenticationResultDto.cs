namespace HodoCloudAPI.Dtos
{
    public class UserAuthenticationResultDto
    {
        public UserAuthenticationResultDto(bool isSuccess, string errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }
        public string ErrorMessage { get; }
    }
}
