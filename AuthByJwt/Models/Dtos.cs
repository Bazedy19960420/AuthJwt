namespace AuthByJwt.Models
{
    public class Dtos
    {
        public record UserForRegisterDto(string FirstName, string LastName, string UserName, string Password, string Email, string PhoneNumber, ICollection<string> Roles);
        public record UserForAuthenticationDto(string UserName, string Password);
        public record TokenDto(string AccessToken, string refreshToken);
    }
}
