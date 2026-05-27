using Microsoft.AspNetCore.Mvc;
using NoticeBoardApi.DTOs;
using NoticeBoardApi.Models;
using NoticeBoardApi.Services;
using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.AspNetCore.Authorization;

namespace NoticeBoardApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NoticesController(INoticeService service) : ControllerBase
    {
        private readonly INoticeService _service = service;

        [Authorize]
        [HttpGet("/api/me")]
        public IActionResult Me()
        {
            var username = User.Identity?.Name;
            var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;
            return Ok(new { username, role });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult GetAll()
        {
            var notices = _service.GetAll().Select(n => new NoticeDto
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                CreatedBy = n.CreatedBy,
                CreatedAt = n.CreatedAt.ToString("dd MMM yyyy HH:mm"),
                IsArchived = n.IsArchived,
            });
            return Ok(notices);
        }


        [AllowAnonymous]
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            
            var notice = _service.GetById(id);
        
            if (notice is null) return NotFound();
            var noticeDto = new NoticeDto
            {
                Id = notice.Id,
                Title = notice.Title,
                Body = notice.Body,
                CreatedBy = notice.CreatedBy,
                CreatedAt = notice.CreatedAt.ToString("dd MMM yyyy HH:mm"),
                IsArchived = notice.IsArchived,
            };
            return Ok(noticeDto); 
        }

        [AllowAnonymous]
        [HttpGet("archived")]
        public IActionResult GetArchived()
        {
            var notices = _service.GetArchived().Select(n => new NoticeDto
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                CreatedBy = n.CreatedBy,
                CreatedAt = n.CreatedAt.ToString("dd MMM yyyy HH:mm"),
                IsArchived = n.IsArchived
            });
            return Ok(notices);
        }

        
        [HttpPost]
        public IActionResult Create([FromBody] Notice notice)
        {
            notice.CreatedBy = User.Identity?.Name ?? "Unknown";
            var created = _service.Create(notice);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }


        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Notice notice)
        {
            var updated = _service.Update(id, notice);
            if (updated is null) return NotFound();
            return Ok(updated);
        }



        [Authorize(Roles="Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var success = _service.Delete(id);
            if (!success) return NotFound();
            return NoContent();
        }



        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/archive")]
        public IActionResult Archive(int id)
        {
            var notice = _service.GetById(id);
            if (notice is null) return NotFound();
            notice.IsArchived = true;
            _service.Update(id, notice);
            return NoContent();
        }
    }
}
