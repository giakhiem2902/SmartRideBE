using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRideBackend.Data;
using SmartRideBackend.DTOs;
using SmartRideBackend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace SmartRideBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            return int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        /// <summary>
        /// Get all chat rooms for current user
        /// </summary>
        [HttpGet("rooms")]
        public async Task<ActionResult<ApiResponse<List<ChatRoomDto>>>> GetChatRooms()
        {
            try
            {
                var userId = GetUserId();

                var rooms = await _context.ChatRooms
                    .Include(r => r.CreatedByUser)
                    .Include(r => r.Members)
                    .Include(r => r.Messages)
                    .ThenInclude(m => m.User)
                    .Where(r => r.Members.Any(m => m.UserId == userId && m.IsActive) && !r.IsDeleted)
                    .Select(r => new ChatRoomDto
                    {
                        Id = r.Id,
                        Title = r.Title ?? "",
                        Description = r.Description ?? "",
                        CreatedByUserId = r.CreatedByUserId,
                        CreatedByUserName = r.CreatedByUser != null ? r.CreatedByUser.FullName ?? "" : "",
                        CreatedAt = r.CreatedAt,
                        UpdatedAt = r.UpdatedAt,
                        IsActive = r.IsActive,
                        MemberCount = r.Members.Count(m => m.IsActive),
                        UnreadCount = 0,
                        LastMessage = r.Messages
                            .OrderByDescending(m => m.CreatedAt)
                            .Select(m => new ChatMessageDto
                            {
                                Id = m.Id,
                                ChatRoomId = m.ChatRoomId,
                                UserId = m.UserId,
                                UserName = m.User != null ? m.User.FullName ?? "Unknown" : "Unknown",
                                Content = m.Content ?? "",
                                CreatedAt = m.CreatedAt,
                                IsOwn = m.UserId == userId
                            })
                            .FirstOrDefault()
                    })
                    .OrderByDescending(r => r.CreatedAt)
                    .ToListAsync();

                return Ok(new ApiResponse<List<ChatRoomDto>>
                {
                    Success = true,
                    Message = "Chat rooms retrieved successfully",
                    Data = rooms
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ChatRoomDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Search users by name or email
        /// </summary>
        [HttpGet("search-users")]
        public async Task<ActionResult<ApiResponse<List<UserSearchDto>>>> SearchUsers([FromQuery] string query = "")
        {
            try
            {
                var currentUserId = GetUserId();

                var users = await _context.Users
                    .Where(u => u.Id != currentUserId && 
                           (u.FullName!.Contains(query) || u.Email!.Contains(query)))
                    .Take(20) // Limit to 20 results
                    .Select(u => new UserSearchDto
                    {
                        Id = u.Id,
                        FullName = u.FullName ?? "Unknown",
                        Email = u.Email ?? ""
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<UserSearchDto>>
                {
                    Success = true,
                    Message = "Users found",
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<UserSearchDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Create a new chat room
        /// </summary>
        [HttpPost("rooms")]
        public async Task<ActionResult<ApiResponse<ChatRoomDto>>> CreateChatRoom([FromBody] CreateChatRoomDto dto)
        {
            try
            {
                var userId = GetUserId();

                var room = new ChatRoom
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    CreatedByUserId = userId,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true,
                    IsDeleted = false
                };

                _context.ChatRooms.Add(room);
                await _context.SaveChangesAsync();

                // Add creator as member
                var member = new ChatRoomMember
                {
                    ChatRoomId = room.Id,
                    UserId = userId,
                    JoinedAt = DateTime.UtcNow,
                    IsActive = true
                };

                _context.ChatRoomMembers.Add(member);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId);

                return Ok(new ApiResponse<ChatRoomDto>
                {
                    Success = true,
                    Message = "Chat room created successfully",
                    Data = new ChatRoomDto
                    {
                        Id = room.Id,
                        Title = room.Title ?? "",
                        Description = room.Description ?? "",
                        CreatedByUserId = room.CreatedByUserId,
                        CreatedByUserName = user?.FullName ?? "Unknown",
                        CreatedAt = room.CreatedAt,
                        UpdatedAt = room.UpdatedAt,
                        IsActive = room.IsActive,
                        MemberCount = 1,
                        UnreadCount = 0,
                        LastMessage = null
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ChatRoomDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Invite user to chat room
        /// </summary>
        [HttpPost("rooms/{roomId}/members")]
        public async Task<ActionResult<ApiResponse<ChatRoomMemberDto>>> InviteUserToChatRoom(int roomId, [FromBody] InviteUserToChatRoomDto dto)
        {
            try
            {
                var userId = GetUserId();

                // Check if room exists
                var room = await _context.ChatRooms
                    .Include(r => r.Members)
                    .FirstOrDefaultAsync(r => r.Id == roomId && !r.IsDeleted);

                if (room == null)
                {
                    return NotFound(new ApiResponse<ChatRoomMemberDto>
                    {
                        Success = false,
                        Message = "Chat room not found"
                    });
                }

                // Check if user is member of this room (only members can invite)
                var isCurrentUserMember = room.Members.Any(m => m.UserId == userId && m.IsActive);
                if (!isCurrentUserMember)
                {
                    return Unauthorized(new ApiResponse<ChatRoomMemberDto>
                    {
                        Success = false,
                        Message = "You are not a member of this chat room"
                    });
                }

                // Check if target user exists
                var targetUser = await _context.Users.FindAsync(dto.UserId);
                if (targetUser == null)
                {
                    return NotFound(new ApiResponse<ChatRoomMemberDto>
                    {
                        Success = false,
                        Message = "User not found"
                    });
                }

                // Check if user is already a member
                var existingMember = await _context.ChatRoomMembers
                    .FirstOrDefaultAsync(m => m.ChatRoomId == roomId && m.UserId == dto.UserId);

                if (existingMember != null && existingMember.IsActive)
                {
                    return BadRequest(new ApiResponse<ChatRoomMemberDto>
                    {
                        Success = false,
                        Message = "User is already a member of this chat room"
                    });
                }

                // Add or reactivate member
                if (existingMember != null)
                {
                    existingMember.IsActive = true;
                }
                else
                {
                    var newMember = new ChatRoomMember
                    {
                        ChatRoomId = roomId,
                        UserId = dto.UserId,
                        JoinedAt = DateTime.UtcNow,
                        IsActive = true
                    };
                    _context.ChatRoomMembers.Add(newMember);
                }

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<ChatRoomMemberDto>
                {
                    Success = true,
                    Message = "User invited to chat room successfully",
                    Data = new ChatRoomMemberDto
                    {
                        Id = existingMember?.Id ?? 0,
                        UserId = dto.UserId,
                        UserName = targetUser.FullName ?? "Unknown",
                        UserEmail = targetUser.Email ?? "",
                        JoinedAt = existingMember?.JoinedAt ?? DateTime.UtcNow,
                        IsActive = true
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ChatRoomMemberDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get members of a chat room
        /// </summary>
        [HttpGet("rooms/{roomId}/members")]
        public async Task<ActionResult<ApiResponse<List<ChatRoomMemberDto>>>> GetChatRoomMembers(int roomId)
        {
            try
            {
                var userId = GetUserId();

                // Check if room exists
                var room = await _context.ChatRooms.FindAsync(roomId);
                if (room == null || room.IsDeleted)
                {
                    return NotFound(new ApiResponse<List<ChatRoomMemberDto>>
                    {
                        Success = false,
                        Message = "Chat room not found"
                    });
                }

                // Get members
                var members = await _context.ChatRoomMembers
                    .Include(m => m.User)
                    .Where(m => m.ChatRoomId == roomId && m.IsActive)
                    .Select(m => new ChatRoomMemberDto
                    {
                        Id = m.Id,
                        UserId = m.UserId,
                        UserName = m.User != null ? m.User.FullName ?? "Unknown" : "Unknown",
                        UserEmail = m.User != null ? m.User.Email ?? "" : "",
                        JoinedAt = m.JoinedAt,
                        IsActive = m.IsActive
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<ChatRoomMemberDto>>
                {
                    Success = true,
                    Message = "Members retrieved successfully",
                    Data = members
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ChatRoomMemberDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get messages in a specific chat room
        /// </summary>
        [HttpGet("rooms/{roomId}/messages")]
        public async Task<ActionResult<ApiResponse<List<ChatMessageDto>>>> GetRoomMessages(int roomId, [FromQuery] int limit = 50, [FromQuery] int offset = 0)
        {
            try
            {
                var userId = GetUserId();

                // Check if user is member of this room
                var isMember = await _context.ChatRoomMembers
                    .AnyAsync(m => m.ChatRoomId == roomId && m.UserId == userId && m.IsActive);

                if (!isMember)
                {
                    return Unauthorized(new ApiResponse<List<ChatMessageDto>>
                    {
                        Success = false,
                        Message = "You are not a member of this chat room"
                    });
                }

                var messages = await _context.ChatMessages
                    .Include(m => m.User)
                    .Where(m => m.ChatRoomId == roomId && !m.IsDeleted)
                    .OrderByDescending(m => m.CreatedAt)
                    .Skip(offset)
                    .Take(limit)
                    .Select(m => new ChatMessageDto
                    {
                        Id = m.Id,
                        ChatRoomId = m.ChatRoomId,
                        UserId = m.UserId,
                        UserName = m.User != null ? m.User.FullName ?? "Unknown" : "Unknown",
                        UserAvatar = "",
                        Content = m.Content ?? "",
                        CreatedAt = m.CreatedAt,
                        EditedAt = m.EditedAt,
                        IsOwn = m.UserId == userId
                    })
                    .OrderBy(m => m.CreatedAt)
                    .ToListAsync();

                return Ok(new ApiResponse<List<ChatMessageDto>>
                {
                    Success = true,
                    Message = "Messages retrieved successfully",
                    Data = messages
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ChatMessageDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Send a message to chat room
        /// </summary>
        [HttpPost("send-message")]
        public async Task<ActionResult<ApiResponse<ChatMessageDto>>> SendMessage([FromBody] SendChatMessageDto dto)
        {
            try
            {
                var userId = GetUserId();

                // Validate room exists and user is member
                var room = await _context.ChatRooms.FindAsync(dto.ChatRoomId);
                if (room == null || room.IsDeleted)
                {
                    return NotFound(new ApiResponse<ChatMessageDto>
                    {
                        Success = false,
                        Message = "Chat room not found"
                    });
                }

                var isMember = await _context.ChatRoomMembers
                    .AnyAsync(m => m.ChatRoomId == dto.ChatRoomId && m.UserId == userId && m.IsActive);

                if (!isMember)
                {
                    return Unauthorized(new ApiResponse<ChatMessageDto>
                    {
                        Success = false,
                        Message = "You are not a member of this chat room"
                    });
                }

                var message = new ChatMessage
                {
                    ChatRoomId = dto.ChatRoomId,
                    UserId = userId,
                    Content = dto.Content,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatMessages.Add(message);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId);

                return Ok(new ApiResponse<ChatMessageDto>
                {
                    Success = true,
                    Message = "Message sent successfully",
                    Data = new ChatMessageDto
                    {
                        Id = message.Id,
                        ChatRoomId = message.ChatRoomId,
                        UserId = message.UserId,
                        UserName = user?.FullName ?? "Unknown",
                        Content = message.Content,
                        CreatedAt = message.CreatedAt,
                        IsOwn = true
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ChatMessageDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Create a new support ticket
        /// </summary>
        [HttpPost("support-tickets")]
        public async Task<ActionResult<ApiResponse<SupportTicketDto>>> CreateSupportTicket([FromBody] CreateSupportTicketDto dto)
        {
            try
            {
                var userId = GetUserId();

                var ticket = new SupportTicket
                {
                    UserId = userId,
                    Title = dto.Title,
                    Description = dto.Description,
                    Category = dto.Category,
                    Priority = dto.Priority,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Open"
                };

                _context.SupportTickets.Add(ticket);
                await _context.SaveChangesAsync();

                var user = await _context.Users.FindAsync(userId);

                return Ok(new ApiResponse<SupportTicketDto>
                {
                    Success = true,
                    Message = "Support ticket created successfully",
                    Data = new SupportTicketDto
                    {
                        Id = ticket.Id,
                        UserId = ticket.UserId,
                        UserName = user?.FullName ?? "Unknown",
                        Title = ticket.Title,
                        Description = ticket.Description,
                        Category = ticket.Category,
                        Status = ticket.Status,
                        Priority = ticket.Priority,
                        CreatedAt = ticket.CreatedAt
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<SupportTicketDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get user's support tickets
        /// </summary>
        [HttpGet("support-tickets")]
        public async Task<ActionResult<ApiResponse<List<SupportTicketDto>>>> GetUserSupportTickets([FromQuery] string? status = null)
        {
            try
            {
                var userId = GetUserId();

                var query = _context.SupportTickets
                    .Where(t => t.UserId == userId && !t.IsDeleted);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(t => t.Status == status);
                }

                var tickets = await query
                    .Include(t => t.User)
                    .OrderByDescending(t => t.CreatedAt)
                    .Select(t => new SupportTicketDto
                    {
                        Id = t.Id,
                        UserId = t.UserId,
                        UserName = t.User != null ? t.User.FullName ?? "Unknown" : "Unknown",
                        Title = t.Title ?? "",
                        Description = t.Description ?? "",
                        Category = t.Category ?? "",
                        Status = t.Status,
                        Priority = t.Priority,
                        CreatedAt = t.CreatedAt,
                        ResolvedAt = t.ResolvedAt,
                        Resolution = t.Resolution
                    })
                    .ToListAsync();

                return Ok(new ApiResponse<List<SupportTicketDto>>
                {
                    Success = true,
                    Message = "Support tickets retrieved successfully",
                    Data = tickets
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<SupportTicketDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get specific support ticket details
        /// </summary>
        [HttpGet("support-tickets/{id}")]
        public async Task<ActionResult<ApiResponse<SupportTicketDto>>> GetSupportTicket(int id)
        {
            try
            {
                var userId = GetUserId();
                var ticket = await _context.SupportTickets
                    .Include(t => t.User)
                    .Include(t => t.AssignedToUser)
                    .Where(t => t.Id == id && !t.IsDeleted && t.UserId == userId)
                    .Select(t => new SupportTicketDto
                    {
                        Id = t.Id,
                        UserId = t.UserId,
                        UserName = t.User != null ? t.User.FullName ?? "Unknown" : "Unknown",
                        Title = t.Title ?? "",
                        Description = t.Description ?? "",
                        Category = t.Category ?? "",
                        Status = t.Status,
                        Priority = t.Priority,
                        CreatedAt = t.CreatedAt,
                        ResolvedAt = t.ResolvedAt,
                        AssignedToUserName = t.AssignedToUser != null ? t.AssignedToUser.FullName : null,
                        Resolution = t.Resolution
                    })
                    .FirstOrDefaultAsync();

                if (ticket == null)
                {
                    return NotFound(new ApiResponse<SupportTicketDto>
                    {
                        Success = false,
                        Message = "Support ticket not found"
                    });
                }

                return Ok(new ApiResponse<SupportTicketDto>
                {
                    Success = true,
                    Message = "Support ticket retrieved successfully",
                    Data = ticket
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<SupportTicketDto>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }

        /// <summary>
        /// Get FAQ items
        /// </summary>
        [HttpGet("faq")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<FaqItemDto>>>> GetFaqItems([FromQuery] string? category = null)
        {
            try
            {
                // TODO: Implement FAQ table
                var faqs = new List<FaqItemDto>
                {
                    new FaqItemDto
                    {
                        Id = 1,
                        Question = "Làm cách nào để đặt vé?",
                        Answer = "Bạn có thể đặt vé bằng cách: 1. Tìm kiếm chuyến xe, 2. Chọn ghế, 3. Thanh toán",
                        Category = "Booking",
                        ViewCount = 100
                    },
                    new FaqItemDto
                    {
                        Id = 2,
                        Question = "Làm cách nào để hủy vé?",
                        Answer = "Vào mục 'Vé của tôi', chọn vé cần hủy, click 'Hủy vé' và xác nhận",
                        Category = "Booking",
                        ViewCount = 80
                    }
                };

                if (!string.IsNullOrEmpty(category))
                {
                    faqs = faqs.Where(f => f.Category == category).ToList();
                }

                return Ok(new ApiResponse<List<FaqItemDto>>
                {
                    Success = true,
                    Message = "FAQ items retrieved successfully",
                    Data = faqs
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<FaqItemDto>>
                {
                    Success = false,
                    Message = ex.Message
                });
            }
        }
    }
}
