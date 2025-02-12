namespace HouseBrokerApp.AppService.Controller
{
    public interface IControllerHelper
    {
        public string GenerateJwtToken(string userId,string email, string fullName);
    }
}
