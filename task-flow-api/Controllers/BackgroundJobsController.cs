using Application.Features.Administration.AuditLog;
using Application.Features.Administration.Auth.Login;
using Application.Features.Administration.Auth.Register;
using Microsoft.AspNetCore.Mvc;

namespace task_flow_api.Controllers
{
    [ApiController]
    [Route("api/background/jobs")]
    public class BackgroundJobsController : ControllerBase
    {
        private readonly AuditLogJobCommandHandler _auditLogJobCommandHandler;

        public BackgroundJobsController(AuditLogJobCommandHandler auditLogJobCommandHandler)
        {
            _auditLogJobCommandHandler = auditLogJobCommandHandler;
        }

        [HttpPost("audit-log")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken cancellationToken)
        {
            await _auditLogJobCommandHandler.ExecuteAsync(cancellationToken);

            return Ok();
        }
    }
}
