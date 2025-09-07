namespace FAQDemo.API.Services.Interfaces
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        string? Email { get; }
        List<string> Roles { get; }
    }
}
