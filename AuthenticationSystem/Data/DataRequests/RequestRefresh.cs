namespace AuthenticationSystem.Data.DataRequests
{
    public class RequestRefresh
    {
        public string TokenJwt { get; set; }
        public string RefreshToken { get; set; }
    }
}