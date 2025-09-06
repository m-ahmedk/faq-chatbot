namespace FAQDemo.API.Services.Interfaces
{
    public interface IPromptService
    {
        Task<string> AnswerAsync(string question, int topN = 5);
    }
}