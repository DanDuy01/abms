﻿using ABMS_backend.DTO;
using ABMS_backend.Models;
using ABMS_backend.Repositories;
using ABMS_backend.Utils.Exceptions;
using ABMS_backend.Utils.Token;
using ABMS_backend.Utils.Validates;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace ABMS_backend.Services
{
    public class ParkingCardService : IParkingCardRepository
    {
        private readonly abmsContext _abmsContext;

        private readonly IHttpContextAccessor _httpContextAccessor;

        public ParkingCardService(abmsContext abmsContext, IHttpContextAccessor httpContextAccessor)
        {
            _abmsContext = abmsContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public ResponseData<string> createParkingCard(ParkingCardForInsertDTO dto)
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
                ParkingCard card = new ParkingCard();
                card.Id = Guid.NewGuid().ToString();
                card.ResidentId = dto.resident_id;
                card.LicensePlate = dto.license_plate;
                card.Brand = dto.brand;
                card.Color = dto.color;
                card.Image = dto.image;
                card.ExpireDate = dto.expire_date;
                card.Note = dto.note;
                card.Status = (int)Constants.STATUS.SENT;
                string getUser = Token.GetUserFromToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
                card.CreateUser = getUser;
                card.CreateTime = DateTime.Now;
                _abmsContext.ParkingCards.Add(card);
                _abmsContext.SaveChanges();
                return new ResponseData<string>
                {
                    Data = card.Id,
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

        public ResponseData<string> updateParkingCard(string id, ParkingCardForEditDTO dto)
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
                ParkingCard card = _abmsContext.ParkingCards.Find(id);
                if(card == null)
                {
                    throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
                }
                card.ResidentId = dto.resident_id;
                card.Brand = dto.brand;
                card.Color = dto.color;
                card.Image = dto.image;
                card.ExpireDate = dto.expire_date;
                card.Note = dto.note;
                card.Status = dto.status;
                string getUser = Token.GetUserFromToken(_httpContextAccessor.HttpContext.Request.Headers["Authorization"]);
                card.ModifyUser = getUser;
                card.ModifyTime = DateTime.Now;
                _abmsContext.ParkingCards.Update(card);
                _abmsContext.SaveChanges();
                return new ResponseData<string>
                {
                    Data = card.Id,
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

        public ResponseData<string> deleteParkingCard(string id)
        {
            try
            {
                ParkingCard card = _abmsContext.ParkingCards.Find(id);
                if (card == null)
                {
                    throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
                }
                card.Status = (int)Constants.STATUS.IN_ACTIVE;
                _abmsContext.ParkingCards.Update(card);
                _abmsContext.SaveChanges();
                return new ResponseData<string>
                {
                    Data = card.Id,
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

        public ResponseData<List<ParkingCard>> getParkingCard(ParkingCardForSearchDTO dto)
        {
            var list = _abmsContext.ParkingCards.
                Where(x => (dto.resident_id == null || x.ResidentId == dto.resident_id)
                && (dto.expire_date == null || x.ExpireDate == dto.expire_date)
                && (dto.status == null || x.Status == dto.status)
                && (dto.room_id == null || x.Resident.RoomId == dto.room_id)).ToList();
            return new ResponseData<List<ParkingCard>>
            {
                Data = list,
                StatusCode = HttpStatusCode.OK,
                ErrMsg = ErrorApp.SUCCESS.description,
                Count = list.Count
            };
        }

        public ResponseData<ParkingCard> getParkingCardById(string id)
        {
            ParkingCard card = _abmsContext.ParkingCards.Find(id);
            if (card == null)
            {
                throw new CustomException(ErrorApp.OBJECT_NOT_FOUND);
            }
            return new ResponseData<ParkingCard>
            {
                Data = card,
                StatusCode = HttpStatusCode.OK,
                ErrMsg = ErrorApp.SUCCESS.description
            };
        }
    }
}
