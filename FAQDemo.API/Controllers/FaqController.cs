using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FAQDemo.API.DTOs.Faq;
using FAQDemo.API.Helpers;
using FAQDemo.API.Models;
using FAQDemo.API.Services.Interfaces;

namespace FAQDemo.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FaqController : ControllerBase
    {
        private readonly IFaqService _faqService;
        private readonly IMapper _mapper;

        public FaqController(IFaqService faqService, IMapper mapper)
        {
            _faqService = faqService;
            _mapper = mapper;
        }

        // GET: /api/faq
        [HttpGet]
        [AllowAnonymous] // FAQs are public
        public async Task<IActionResult> GetAll()
        {
            var faqs = await _faqService.GetAllAsync();
            var dto = _mapper.Map<List<FaqDto>>(faqs);

            return Ok(ApiResponse<List<FaqDto>>.SuccessResponse(dto));
        }

        // GET: /api/faq/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var faq = await _faqService.GetByIdAsync(id);
            if (faq == null)
                return NotFound(ApiResponse<string>.FailResponse("FAQ not found"));

            var dto = _mapper.Map<FaqDto>(faq);
            return Ok(ApiResponse<FaqDto>.SuccessResponse(dto));
        }

        // POST: /api/faq
        [HttpPost]
        [Authorize(Roles = "Admin")] // only admins can add
        public async Task<IActionResult> Create([FromBody] CreateFaqDto dto)
        {
            var faq = _mapper.Map<Faq>(dto);
            var created = await _faqService.AddAsync(faq);
            var faqDto = _mapper.Map<FaqDto>(created);

            return CreatedAtAction(nameof(GetById), new { id = created.Id },
                ApiResponse<FaqDto>.SuccessResponse(faqDto, "FAQ created successfully"));
        }

        // PATCH: /api/faq
        [HttpPatch]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update([FromBody] UpdateFaqDto dto)
        {
            var faq = await _faqService.GetByIdAsync(dto.Id);
            if (faq == null)
                return NotFound(ApiResponse<string>.FailResponse("FAQ not found"));

            // Apply changes
            _mapper.Map(dto, faq);

            await _faqService.UpdateAsync(faq);
            var updatedDto = _mapper.Map<FaqDto>(faq);

            return Ok(ApiResponse<FaqDto>.SuccessResponse(updatedDto, "FAQ updated successfully"));
        }

        // DELETE: /api/faq/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _faqService.DeleteAsync(id);
            if (!deleted)
                return NotFound(ApiResponse<string>.FailResponse("FAQ not found"));

            return Ok(ApiResponse<string>.SuccessResponse("FAQ deleted successfully"));
        }

        // GET: /api/faq/search?query=shipping
        [HttpGet("search")]
        [AllowAnonymous] // FAQs are public
        public async Task<IActionResult> Search([FromQuery] string query, [FromQuery] int topN = 5)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(ApiResponse<string>.FailResponse("Query cannot be empty."));

            var faqs = await _faqService.SearchAsync(query, topN);
            var dto = _mapper.Map<List<FaqDto>>(faqs);

            return Ok(ApiResponse<List<FaqDto>>.SuccessResponse(dto));
        }

        // GET: /api/faq/ask?question=How do I reset my password?
        [HttpGet("ask")]
        [AllowAnonymous]
        public async Task<IActionResult> Ask([FromQuery] string question)
        {
            if (string.IsNullOrWhiteSpace(question))
                return BadRequest(ApiResponse<string>.FailResponse("Question cannot be empty."));

            var answer = await _faqService.AskAsync(question);
            return Ok(ApiResponse<string>.SuccessResponse(answer));
        }

    }
}
