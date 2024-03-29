﻿using GB_Backend.Data;
using GB_Backend.Models;
using GB_Backend.Models.APIforms;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace GB_Backend.Controllers
{
    [Route("api/[controller]/[action]")]
    [EnableCors]
    [Authorize]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public NotificationController(AppDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult getAllComplaint()
        {
            try
            {
                var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
                if (claim == null)
                {
                    return BadRequest("Wrong User email");
                }
                var user = _userManager.Users.FirstOrDefault(obj => obj.Email == claim.Value);
                if (user == null)
                {
                    return BadRequest("Wrong User email");
                }
                return Ok(_db.Notifications.Include(obj => obj.Receiver).Include(obj => obj.Sender).Where(obj => obj.Type == NotificationType.Complaint && obj.Receiver == user).Select(obj => new
                {
                    obj.Id,
                    obj.Title,
                    obj.Description,
                    obj.Date,
                    obj.Viewed,
                    sender = obj.Sender.Email,
                    senderType = _db.Roles.Where(obj2 => obj2.Id == _db.UserRoles.Where(obj1 => obj1.UserId == obj.Sender.Id).FirstOrDefault().RoleId).FirstOrDefault().Name,
                }).ToList());
            } catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
        [HttpGet]
        public IActionResult getAllMessage()
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _userManager.Users.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }
            return Ok(_db.Notifications.Include(obj => obj.Receiver).Include(obj => obj.Sender).Where(obj => obj.Type == NotificationType.Message && obj.Receiver == user).Select(obj => new
            {
                obj.Id,
                obj.Title,
                obj.Description,
                obj.Date,
                obj.Viewed,
                sender = obj.Sender.Email,
                senderType = _db.Roles.Where(obj2 => obj2.Id == _db.UserRoles.Where(obj1 => obj1.UserId == obj.Sender.Id).FirstOrDefault().RoleId).FirstOrDefault().Name,
            }).ToList());
        }

        [HttpGet]
        public IActionResult getMyComplaint()
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _userManager.Users.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong User email");
            }
            return Ok(_db.Notifications.Include(obj => obj.Receiver).Include(obj => obj.Sender).Where(obj => obj.Type == NotificationType.Complaint && obj.Sender == user).Select(obj => new
            {
                obj.Id,
                obj.Title,
                obj.Description,
                obj.Date,
                obj.Viewed,
            }).ToList());
        }

        [HttpPost]
        public IActionResult PushComplaint([FromBody] NotificationForm model)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var sender = _userManager.Users.FirstOrDefault(obj => obj.Email == claim.Value);
            if (sender == null)
            {
                return BadRequest("Wrong sender email");
            }
            var receiver = _userManager.Users.FirstOrDefault(obj => obj.Email == model.ReceiverEmail);
            if (receiver == null)
            {
                return BadRequest("Wrong receiver email");
            }
            if (!_userManager.IsInRoleAsync(receiver, "Admin").Result)
            {
                return BadRequest("can send complaint to admin only");
            }

            if (receiver.Email == sender.Email)
            {
                return BadRequest("can't send message to yourself");
            }

            Notification notification = new Notification();
            notification.SenderId = sender.Id;
            notification.ReceiverId = receiver.Id;
            notification.Description = model.Description;
            notification.Date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt");
            notification.Title = model.Title;
            notification.Viewed = false;
            notification.Type = NotificationType.Complaint;

            _db.Notifications.Add(notification);
            _db.SaveChanges();

            return Ok(new { id = notification.Id });
        }
        [HttpPost]
        public IActionResult PushMessage([FromBody] NotificationForm model)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var sender = _userManager.Users.FirstOrDefault(obj => obj.Email == claim.Value);
            if (sender == null)
            {
                return BadRequest("Wrong sender email");
            }
            var receiver = _userManager.Users.FirstOrDefault(obj => obj.Email == model.ReceiverEmail);
            if (receiver == null)
            {
                return BadRequest("Wrong receiver email");
            }

            if(receiver.Email == sender.Email)
            {
                return BadRequest("can't send message to yourself");
            }

            Notification notification = new Notification();
            notification.SenderId = sender.Id;
            notification.ReceiverId = receiver.Id;
            notification.Description = model.Description;
            notification.Date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Egypt Standard Time").ToString("dd-MM-yyyy hh:mm tt");
            notification.Title = model.Title;
            notification.Viewed = false;
            notification.Type = NotificationType.Message;


            _db.Notifications.Add(notification);
            _db.SaveChanges();

            return Ok(new {id = notification.Id});
        }
        [HttpGet]
        public IActionResult NotificationViewed(int id)
        {
            var claim = User.Claims.FirstOrDefault(obj => obj.Type == "Email");
            if (claim == null)
            {
                return BadRequest("Wrong User email");
            }
            var user = _userManager.Users.FirstOrDefault(obj => obj.Email == claim.Value);
            if (user == null)
            {
                return BadRequest("Wrong user email");
            }
            Notification notification = _db.Notifications.Include(obj => obj.Receiver).FirstOrDefault(obj => obj.Id == id);
            if (notification == null)
            {
                return BadRequest("Wrong notification id");
            }
            if (notification.Receiver.Id != user.Id)
            {
                return BadRequest("User not the receiver of this notification");
            }
            notification.Viewed = true;
            _db.Notifications.Update(notification);
            _db.SaveChanges();

            return Ok("Operation is done");
        }
    }
}
