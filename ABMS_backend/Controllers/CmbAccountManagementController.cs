﻿using Microsoft.AspNetCore.Mvc;
using ABMS_backend.Utils.Validates;
using ABMS_backend.DTO;
using ABMS_backend.Repositories;

namespace ABMS_backend.Controllers
{
    [Route(Constants.REQUEST_MAPPING_PREFIX + Constants.VERSION_API_V1)]
    [ApiController]
    public class CmbAccountManagementController : ControllerBase
    {
        private ICmbAccountManagementRepository _service;

        public CmbAccountManagementController(ICmbAccountManagementRepository service)
        {
            _service = service;
        }

        [HttpPost("CmbAccount/create")]
        public IActionResult Create([FromBody] AccountDTO dto)
        {
            _service.createCmbAccount(dto);
            return Ok();
        }
    }
}
