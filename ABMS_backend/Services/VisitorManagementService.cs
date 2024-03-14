﻿using ABMS_backend.DTO;
using ABMS_backend.Models;
using ABMS_backend.Repositories;
using ABMS_backend.Utils.Exceptions;
using ABMS_backend.Utils.Token;
using ABMS_backend.Utils.Validates;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net;

namespace ABMS_backend.Services
{
    public class VisitorManagementService : IVisitorManagementRepository
    {
        private readonly abmsContext _abmsContext;
        private IMapper _mapper;

        private readonly IHttpContextAccessor _httpContextAccessor;
        public VisitorManagementService(abmsContext abmsContext, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _abmsContext = abmsContext;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public ResponseData<string> createRequestVisitor(VisitorForInsertDTO dto)
        {
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
                Visitor visitor= new Visitor();
                visitor.Id = Guid.NewGuid().ToString();
                visitor.RoomId = dto.roomId;
                visitor.FullName= dto.fullName;
                visitor.ArrivalTime = dto.arrivalTime;
                visitor.DepartureTime = dto.departureTime;
                visitor.Gender= dto.gender;
                visitor.PhoneNumber= dto.phoneNumber;
                visitor.IdentityNumber= dto.identityNumber;
                visitor.IdentityCardImgUrl= dto.identityCardImgUrl;
                visitor.Description= dto.description;
                //string getUser = Token.GetUserFromToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
                //visitor.ApproveUser= getUser;
                visitor.Status = (int)Constants.STATUS.SENT;
                _abmsContext.Visitors.Add(visitor);
                _abmsContext.SaveChanges();
                return new ResponseData<string>
                {
                    Data = visitor.Id,
                    StatusCode = HttpStatusCode.OK,
                    ErrMsg = ErrorApp.SUCCESS.description
                };

            }
            catch (Exception ex)
            {
                return new ResponseData<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrMsg = "Created failed why " + ex.Message
                };

            }

        }

        public ResponseData<string> deleteRequestVisitor(string id)
        {
            try
            {
                Visitor visitor_ = _abmsContext.Visitors.Find(id);
                if (visitor_ == null)
                {
                    throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
                }

                // Use visitor_ with the correct Id for updating:
                visitor_.Status = (int)Constants.STATUS.IN_ACTIVE;
                _abmsContext.Visitors.Update(visitor_);
                _abmsContext.SaveChanges();

                return new ResponseData<string>
                {
                    Data = visitor_.Id,
                    StatusCode = HttpStatusCode.OK,
                    ErrMsg = ErrorApp.SUCCESS.description
                };

            }
            catch (Exception ex)
            {
                return new ResponseData<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrMsg = "Delete failed: " + ex.Message
                };

            }
        }
        public ResponseData<List<Visitor>> getAllRequestVisitor(VisitorForSearchDTO dto)
        {

            var list = _abmsContext.Visitors.Include(x=>x.Room)
                .Where(x => (dto.roomId== null || x.RoomId== dto.roomId)
                && (dto.fullName== null || x.FullName== dto.fullName)
                && (dto.building_id == null || x.Room.BuildingId == dto.building_id)
                && (dto.time == null || (x.ArrivalTime <= dto.time && dto.time <= x.DepartureTime ))
                &&(dto.status== null || x.Status == dto.status)).Select(x=> new Visitor
                {
                    RoomId = x.RoomId,
                    FullName = x.FullName,
                    ArrivalTime = x.ArrivalTime,
                    DepartureTime = x.DepartureTime,
                    Status = x.Status,
                    Room =x.Room,
                    Id = x.Id,
                    PhoneNumber=x.PhoneNumber,
                    IdentityNumber=x.IdentityNumber,
                    IdentityCardImgUrl=x.IdentityCardImgUrl,
                    Description=x.Description,
                    ApproveUser=x.ApproveUser,
                    Gender=x.Gender,
                }).ToList();           
            return new ResponseData<List<Visitor>>
            {
                Data = list,
                StatusCode = HttpStatusCode.OK,
                ErrMsg = ErrorApp.SUCCESS.description,
                Count = list.Count
            };

        }
        public ResponseData<string> manageVisitor(string id, int status)
        {
            Visitor visitor = _abmsContext.Visitors.Find(id);
            if (visitor == null)
            {
                throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
            }
            visitor.Status = status;
            string getUser = Token.GetUserFromToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
            visitor.ApproveUser = getUser;
            _abmsContext.Visitors.Update(visitor);
            _abmsContext.SaveChanges();
            return new ResponseData<string>
            {
                Data = visitor.Id,
                StatusCode = HttpStatusCode.OK,
                ErrMsg = ErrorApp.SUCCESS.description
            };
        }

        public ResponseData<string> updateRequestVisitor(string id, VisitorForInsertDTO dto)
        {
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
                Visitor visitor = _abmsContext.Visitors.Find(id);

                if (visitor == null)
                {
                    throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
                }
                visitor.RoomId = dto.roomId;
                visitor.FullName = dto.fullName;
                visitor.ArrivalTime = dto.arrivalTime;
                visitor.DepartureTime = dto.departureTime;
                visitor.Gender = dto.gender;
                visitor.PhoneNumber = dto.phoneNumber;
                visitor.IdentityNumber = dto.identityNumber;
                visitor.IdentityCardImgUrl = dto.identityCardImgUrl;
                visitor.Description = dto.description;               
                string getUser = Token.GetUserFromToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
                visitor.ApproveUser = getUser;
                _abmsContext.Visitors.Update(visitor);
                _abmsContext.SaveChanges();
                return new ResponseData<string>
                {
                    Data = visitor.Id,
                    StatusCode = HttpStatusCode.OK,
                    ErrMsg = ErrorApp.SUCCESS.description
                };
            }
            catch (Exception ex)
            {
                return new ResponseData<string>
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ErrMsg = "Update failed why " + ex.Message
                };
            }
        }

        public ResponseData<Visitor> getRequestVisitorById(string id)
        {
            Visitor visitor = _abmsContext.Visitors.Find(id);
            if (visitor == null)
            {
                throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
            }
            return new ResponseData<Visitor>
            {
                Data = visitor,
                StatusCode = HttpStatusCode.OK,
                ErrMsg = ErrorApp.SUCCESS.description
            };
        }
    }
}
