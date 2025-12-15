using Microsoft.AspNetCore.Mvc;

namespace Lab5.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MemberController : ControllerBase
    {
        private readonly IMemberService _memberService;

        public MemberController(IMemberService memberService)
        {
            _memberService = memberService;
        }

        [HttpPost]
        public async Task<IActionResult> AddMember([FromBody] MemberDTO memberDto)
        {
            try
            {
                await _memberService.AddMemberAsync(memberDto);
                return Ok("Thành viên đã được thêm thành công!");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
