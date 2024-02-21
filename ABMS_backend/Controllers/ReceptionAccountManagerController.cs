﻿using ABMS_backend.DTO;
using ABMS_backend.Repositories;
using ABMS_backend.Services;
using ABMS_backend.Utils.Validates;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;

namespace ABMS_backend.Controllers
{
    [Route(Constants.REQUEST_MAPPING_PREFIX + Constants.VERSION_API_V1)]
    [ApiController]
    public class ReceptionAccountManagerController : ControllerBase
    {
        private IReceptionistAccountRepository _service;

        public ReceptionAccountManagerController(IReceptionistAccountRepository service)
        {
            _service = service;
        }

        [HttpPost("ReceptionAccount/create")]
        public IActionResult Create([FromBody] AccountDTO dto)
        {
            _service.createReceptionAccount(dto);
            return Ok();
        }
    }
}
