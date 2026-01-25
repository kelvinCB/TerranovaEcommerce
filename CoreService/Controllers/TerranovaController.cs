using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreService.Data;
using CoreService.Repositories.Interface;

namespace CoreService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TerranovaController : ControllerBase
    {
        private readonly IEventLogRepository _eventLogRepository;

        public TerranovaController(IEventLogRepository eventLogRepository)
        {
            _eventLogRepository = eventLogRepository;
        }

        [HttpGet("connection")]
        public async Task<IActionResult> GetConnection()
        {
            
        }
    }
}