﻿using Microsoft.AspNetCore.Mvc;
using ABMS_backend.Utils.Validates;
using ABMS_backend.DTO;
using ABMS_backend.Repositories;
using ABMS_backend.Models;

namespace ABMS_backend.Controllers
{
    [Route(Constants.REQUEST_MAPPING_PREFIX + Constants.VERSION_API_V1)]
    [ApiController]
    public class CmbAccountManagementController : ControllerBase
    {
        private ICmbAccountManagementRepository _repository;

        public CmbAccountManagementController(ICmbAccountManagementRepository repository)
        {
            _repository = repository;
        }

        [HttpPost("cmb-account/create")]
        public ResponseData<string> Create([FromBody] AccountForInsertDTO dto)
        {
            ResponseData<string> response = _repository.createCmbAccount(dto);
            return response;
        }

        [HttpPut("cmb-account/update/{id}")]
        public ResponseData<string> Update(String id, [FromBody] AccountForInsertDTO dto)
        {
            ResponseData<string> response = _repository.updateCmbAccount(id, dto);
            return response;
        }

        [HttpDelete("cmb-account/delete/{id}")]
        public ResponseData<string> Delete(String id)
        {
            ResponseData<string> response = _repository.deleteCmbAccount(id);
            return response;
        }

        [HttpGet("cmb-account/get")]
        public ResponseData<List<Account>> Get(AccountForSearchDTO dto)
        {
            ResponseData<List<Account>> response = _repository.getCmbAccount(dto);
            return response;
        }


        [HttpGet("cmb-account/get/{id}")]
        public ResponseData<Account> GetById(String id)
        {
            ResponseData<Account> response = _repository.getCmbAccountById(id);
            return response;
        }
    }
}
