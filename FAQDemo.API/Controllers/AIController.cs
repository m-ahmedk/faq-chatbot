using Microsoft.AspNetCore.Mvc;
using FAQDemo.API.Data;
using FAQDemo.API.Helpers;
using FAQDemo.API.Services.Interfaces;

namespace FAQDemo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AIController : ControllerBase
    {
        private readonly IEmbeddingService _embeddingService;
        private readonly IPromptService _promptService;
        private readonly AppDbContext _db;

        public AIController(IEmbeddingService embeddingService, IPromptService promptService, AppDbContext db)
        {
            _promptService = promptService;
            _embeddingService = embeddingService;
            _db = db;
        }

        // EMBEDDING
        /// <summary>
        /// Step 2: Generate an embedding for an existing product.
        /// Example: POST /api/ai/embed-product/1
        /// </summary>
        [HttpPost("embed-product/{id}")]
        public async Task<IActionResult> EmbedProduct(int id)
        {
            // First fetch the product from the database
            var product = await _db.Products.FindAsync(id);
            if (product == null) return NotFound("Product not found");

            // Call service to generate embedding + save in Embeddings table
            await _embeddingService.CreateProductEmbeddingAsync(product);

            return Ok($"Embedding created for product {product.Name} (Id: {id})");
        }

        /// <summary>
        /// Step 3: Search for products semantically.
        /// Example: GET /api/ai/search?q=chocolate
        /// </summary>
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string q, [FromQuery] int topN = 5)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Query cannot be empty");

            // Call service to perform vector similarity search
            var results = await _embeddingService.SearchProductsAsync(q, topN);

            return Ok(results);
        }

        // PROMPT
        /// <summary>
        /// Ask a question about products using RAG.
        /// Example: GET /api/ai/ask?question=What’s the cheapest product?
        /// </summary>
        [HttpGet("ask")]
        public async Task<IActionResult> Ask([FromQuery] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest("Question cannot be empty");

            var answer = await _promptService.AnswerAsync(question);

            return Ok(ApiResponse<string>.SuccessResponse(answer, "Answer generated successfully"));
        }
    }
}