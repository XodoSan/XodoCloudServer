namespace Application.Entities
{
    public class UserAuthenticationResult
    {
        public UserAuthenticationResult(bool result, string error)
        {
            Result = result;
            Error = error;
        }

        public bool Result { get; }
        public string Error { get; }
    }
}
