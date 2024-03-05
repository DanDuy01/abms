﻿using ABMS_backend.DTO;
using ABMS_backend.Models;
using ABMS_backend.Repositories;
using ABMS_backend.Utils.Validates;
using System.Net;
using ABMS_backend.Utils.Exceptions;
using Microsoft.AspNetCore.Http;

namespace ABMS_backend.Services
{
    public class CmbAccountManagementService : ICmbAccountManagementRepository
    {
        private readonly abmsContext _abmsContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CmbAccountManagementService(abmsContext abmsContext, IHttpContextAccessor httpContext)
        {
            _abmsContext = abmsContext;
            _httpContextAccessor = httpContext;
        }

        ResponseData<string> ICmbAccountManagementRepository.updateCmbAccount(string id, AccountForInsertDTO dto)
        {
            //validate
            string error = dto.Validate();

            if (error != null)
            {
                return new ResponseData<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrMsg = error
                };
            }

            try
            {
                Account account = _abmsContext.Accounts.Find(id);
                if (account == null)
                {
                    throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
                }
                Account account1 = _abmsContext.Accounts.FirstOrDefault(x => x.PhoneNumber == dto.phone || x.Email == dto.email);
                if (account1 != null && account1 != account)
                {
                    throw new CustomException(ErrorApp.ACCOUNT_EXISTED);
                }
                account.BuildingId = dto.building_id;
                account.PhoneNumber = dto.phone;
                account.Email = dto.email;
                account.FullName = dto.full_name;
                account.Role = dto.role;
                account.UserName = dto.user_name;
                account.Avatar = dto.avatar;
                if (_httpContextAccessor.HttpContext.Session.GetString("user") == null)
                {
                    return new ResponseData<string>
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        ErrMsg = ErrorApp.FORBIDDEN.description
                    };
                }
                account.ModifyUser = _httpContextAccessor.HttpContext.Session.GetString("user");
                account.ModifyTime = DateTime.Now;
                _abmsContext.Accounts.Update(account);
                _abmsContext.SaveChanges();
                return new ResponseData<string>
                {
                    Data = account.Id,
                    StatusCode = HttpStatusCode.OK,
                    ErrMsg = ErrorApp.SUCCESS.description
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrMsg = "Updated failed why " + ex.Message
                };
            }
        }

        ResponseData<string> ICmbAccountManagementRepository.deleteCmbAccount(string id)
        {
            try
            {
                //xoa account
                Account account = _abmsContext.Accounts.Find(id);
                if (account == null)
                {
                    throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
                }               
                account.Status = (int)Constants.STATUS.IN_ACTIVE;
                if (_httpContextAccessor.HttpContext.Session.GetString("user") == null)
                {
                    return new ResponseData<string>
                    {
                        StatusCode = HttpStatusCode.Forbidden,
                        ErrMsg = ErrorApp.FORBIDDEN.description
                    };
                }
                account.ModifyUser = _httpContextAccessor.HttpContext.Session.GetString("user");
                account.ModifyTime = DateTime.Now;
                _abmsContext.Accounts.Update(account);
                _abmsContext.SaveChanges();
                return new ResponseData<string>
                {
                    Data = account.Id,
                    StatusCode = HttpStatusCode.OK,
                    ErrMsg = ErrorApp.SUCCESS.description
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrMsg = "Deleted failed why " + ex.Message
                };
            }
        }

        ResponseData<List<Account>> ICmbAccountManagementRepository.getCmbAccount(AccountForSearchDTO dto)
        {
            var list = _abmsContext.Accounts.
                Where(x => (dto.searchMessage == null || x.PhoneNumber.Contains(dto.searchMessage.ToLower()) 
                || x.Email.ToLower().Contains(dto.searchMessage.ToLower()) 
                || x.FullName.ToLower().Contains(dto.searchMessage.ToLower()))
                && (dto.buildingId == null || x.BuildingId.Equals(dto.buildingId))
                && (dto.role == null || x.Role.Equals(dto.role))
                && (dto.status == null || x.Status == dto.status)).ToList();
            return new ResponseData<List<Account>>
            {
                Data = list,
                StatusCode = HttpStatusCode.OK,
                ErrMsg = ErrorApp.SUCCESS.description,
                Count = list.Count
            };
        }

        ResponseData<Account> ICmbAccountManagementRepository.getCmbAccountById(string id)
        {
            Account account = _abmsContext.Accounts.Find(id);
            if (account == null)
            {
                throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
            }
            return new ResponseData<Account>
            {
                Data = account,
                StatusCode = HttpStatusCode.OK,
                ErrMsg = ErrorApp.SUCCESS.description
            };
        }


    }
}
