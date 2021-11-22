namespace DMSTask.Services
{
    public interface IUserService
    {
        string GetUserId();
        bool IsAuthenticated();
    }
}
