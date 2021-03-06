using OAuth.Sample.Domain.Enum;

namespace OAuth.Sample.Domain.Model.User
{

    public class LoginRequest
    {
        public ProviderType ProviderType { get; set; }
        public string AccessToken { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string PhotoUrl { get; set; }
        public string Email { get; set; }
        public string Description { get; set; }

    }

    public class LoginResponse
    {

        public int UserId { get; set; }
        //public string Name { get; set; }
        //public string PhotoUrl { get; set; }
        //public string Description { get; set; }

        public string JwtToken { get; set; }

    }
}