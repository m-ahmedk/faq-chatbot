using FAQDemo.API.Models;
using FAQDemo.API.Repositories.Interfaces;
using FAQDemo.API.Services.Interfaces;
using OpenAI;
using OpenAI.Chat;
using OpenAI.Embeddings;
using Pgvector;

namespace FAQDemo.API.Services
{
    public class FaqService : IFaqService
    {
        private readonly IFaqRepository _repository;
        private readonly EmbeddingClient _embeddingClient;
        private readonly ChatClient _chatClient;

        public FaqService(IFaqRepository repository, IConfiguration config)
        {
            _repository = repository;

            // OpenAI client
            var apiKey = config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("OpenAI API key not found.");

            var client = new OpenAIClient(apiKey);
            _embeddingClient = client.GetEmbeddingClient("text-embedding-3-small");

            var chatClient = new OpenAIClient(apiKey);
            _chatClient = chatClient.GetChatClient("gpt-5-mini");
        }

        public async Task<Faq> AddAsync(Faq faq)
        {
            // Generate embedding for Question + Answer
            var response = await _embeddingClient.GenerateEmbeddingAsync($"{faq.Question} {faq.Answer}");
            var vector = response.Value.ToFloats().ToArray();

            faq.Vector = new Vector(vector);

            return await _repository.AddAsync(faq);
        }


        public Task<List<Faq>> GetAllAsync() => _repository.GetAllAsync();

        public Task<Faq?> GetByIdAsync(int id) => _repository.GetByIdAsync(id);

        public Task UpdateAsync(Faq faq) => _repository.UpdateAsync(faq);

        public Task<bool> DeleteAsync(int id) => _repository.DeleteAsync(id);

        public async Task<List<Faq>> SearchAsync(string query, int topN = 5)
        {
            // 1. Embed the query
            var response = await _embeddingClient.GenerateEmbeddingAsync(query);
            var queryVector = new Vector(response.Value.ToFloats().ToArray());

            // 2. Retrieve closest FAQs
            return await _repository.SearchAsync(queryVector, topN);
        }

        public async Task<string> AskAsync(string question, int topN = 3)
        {
            // Step 1: Embed the query
            var response = await _embeddingClient.GenerateEmbeddingAsync(question);
            var queryVector = new Vector(response.Value.ToFloats().ToArray());

            // Step 2: Get top FAQs
            var faqs = await _repository.SearchAsync(queryVector, topN);
            if (!faqs.Any())
                return "Sorry, I couldn't find any relevant FAQ.";

            // Step 3: Build context for GPT
            var context = string.Join("\n", faqs.Select(f =>
                $"- Q: {f.Question}\n  A: {f.Answer}"));

            // Step 4: Ask GPT to respond naturally

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage("You are a helpful FAQ assistant. Only answer based on the FAQ data provided."),
                new UserChatMessage($@"
                    FAQ Data:
                    {context}

                    User Question: {question}
                    Answer naturally, but only using the FAQ data."
                )
            };

            var completion = await _chatClient.CompleteChatAsync(messages);
            return completion.Value.Content[0].Text.Trim();
        }

    }
}
