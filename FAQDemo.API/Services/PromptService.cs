using OpenAI;
using OpenAI.Chat;
using FAQDemo.API.Services.Interfaces;

namespace FAQDemo.API.Services
{
    public class PromptService : IPromptService
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly ChatClient _chatClient;

        public PromptService(IEmbeddingService embeddingService, IConfiguration config)
        {
            _embeddingService = embeddingService;

            var apiKey = config["OpenAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
                throw new InvalidOperationException("OpenAI API key not found.");

            var client = new OpenAIClient(apiKey);
            _chatClient = client.GetChatClient("gpt-5-mini");
        }

        public async Task<string> AnswerAsync(string question, int topN = 10)
        {
            // Step 1: Robust classifier
            var classifierMessages = new List<ChatMessage>
            {
                new SystemChatMessage(@"
                    You are a query classifier for product-related questions.
                    Output ONLY one of these tokens: COUNT, MIN, MAX, AVERAGE, RANK, FUZZY.

                    Definitions:
                    - COUNT → any question about how many, total, number of products/items
                    - MIN → cheapest, lowest price
                    - MAX → most expensive, highest price
                    - AVERAGE → average price or average quantity
                    - RANK → nth cheapest or nth most expensive
                    - FUZZY → descriptive/lookups like drinks, snacks, healthy options

                    Examples:
                    'How many products are there?' -> COUNT
                    'Total products you have' -> COUNT
                    'Cheapest drink?' -> MIN
                    'Most expensive product' -> MAX
                    'Average price of snacks' -> AVERAGE
                    'Second most expensive product' -> RANK
                    'Any healthy snacks?' -> FUZZY
                    "),
                new UserChatMessage(question)
            };

            var classification = await _chatClient.CompleteChatAsync(classifierMessages);
            var category = classification.Value.Content[0].Text.Trim().ToUpperInvariant();

            // Step 2: Retrieve products from vector search
            var products = await _embeddingService.SearchProductsAsync(question, topN);
            if (!products.Any())
                return "I could not find any relevant products in the database.";

            // Step 3: Deterministic queries with LINQ
            if (category == "COUNT")
            {
                return $"There are {products.Count} products available.";
            }

            if (category == "MIN")
            {
                var cheapest = products.OrderBy(p => p.Price).First();
                return $"The cheapest product is {cheapest.Name}, priced at {cheapest.Price:C}.";
            }

            if (category == "MAX")
            {
                var expensive = products.OrderByDescending(p => p.Price).First();
                return $"The most expensive product is {expensive.Name}, priced at {expensive.Price:C}.";
            }

            if (category == "AVERAGE")
            {
                var avg = products.Average(p => p.Price);
                return $"The average price of the matched products is {avg:F2}.";
            }

            if (category == "RANK")
            {
                var q = question.ToLowerInvariant();
                int position = 1;
                if (q.Contains("second")) position = 2;
                else if (q.Contains("third")) position = 3;

                var ordered = products.OrderByDescending(p => p.Price).ToList();
                if (position <= ordered.Count)
                {
                    var nth = ordered.Skip(position - 1).First();
                    return $"The {position} most expensive product is {nth.Name}, priced at {nth.Price:C}.";
                }
                return "Not enough products found for that ranking.";
            }

            // Step 4: FUZZY fallback → GPT summarization
            // Format product data as JSON-like for better context
            var context = string.Join(",\n", products.Select(p =>
                $"{{ \"name\": \"{p.Name}\", \"price\": {p.Price}, \"quantity\": {p.Quantity} }}"));

            var messages = new List<ChatMessage>
            {
                new SystemChatMessage(@"
                    You are a helpful assistant. Only answer from the provided product data.
                    - Do not invent products or prices.
                    - If answering with products, use plain text or a simple list.
                    - If calculation is required, but not classified, politely explain that only supported operations are COUNT, MIN, MAX, AVERAGE, and RANK."),
                
                // Optional few-shot example
                new SystemChatMessage(@"
                    Example:
                    Product Data: [ { ""name"": ""Coke"", ""price"": 1.5 }, { ""name"": ""Pepsi"", ""price"": 1.4 } ]
                    Question: What’s the cheapest product?
                    Answer: Pepsi ($1.40)"),

                    new UserChatMessage($@"
                        Product Data:
                        [{context}]

                        Question: {question}
                    ")
            };

            var response = await _chatClient.CompleteChatAsync(messages);
            return response.Value.Content[0].Text.Trim();
        }
    }
}
