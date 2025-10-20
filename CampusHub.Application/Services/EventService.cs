using CampusHub.Application.DTO.Events;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace CampusHub.Application.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly ILogger<EventService> _logger;

        public EventService(IEventRepository eventRepository, ILogger<EventService> logger)
        {
            _eventRepository = eventRepository;
            _logger = logger;
        }

        public async Task<Result<List<EventDto>>> GetAllEventsAsync(int? userId = null)
        {
            try
            {
                var result = await _eventRepository.GetAllAsync();
                if (result.IsFailed)
                    return Result.Fail<List<EventDto>>(result.Errors);

                var bookmarkedIds = userId.HasValue ? await _eventRepository.GetBookmarkedEventIdsAsync(userId.Value) : new List<int>();
                var dtos = result.Value.Select(evt => MapToDto(evt, bookmarkedIds.Contains(evt.Id))).ToList();
                return Result.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve events");
                return Result.Fail<List<EventDto>>($"Failed to retrieve events: {ex.Message}");
            }
        }

        public async Task<Result<EventDto>> GetEventByIdAsync(int id)
        {
            try
            {
                var result = await _eventRepository.GetByIdAsync(id);
                if (result.IsFailed)
                    return Result.Fail<EventDto>(result.Errors);

                return Result.Ok(MapToDto(result.Value, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve event {EventId}", id);
                return Result.Fail<EventDto>($"Failed to retrieve event: {ex.Message}");
            }
        }

        public async Task<Result<List<EventDto>>> GetEventsByCollegeAsync(string college)
        {
            try
            {
                var result = await _eventRepository.GetByCollegeAsync(college);
                if (result.IsFailed)
                    return Result.Fail<List<EventDto>>(result.Errors);

                var dtos = result.Value.Select(evt => MapToDto(evt, false)).ToList();
                return Result.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve events for college {College}", college);
                return Result.Fail<List<EventDto>>($"Failed to retrieve events for college: {ex.Message}");
            }
        }

        public async Task<Result<EventDto>> CreateEventAsync(EventDto eventDto)
        {
            try
            {
                var evt = MapToEntity(eventDto);  
                var result = await _eventRepository.CreateAsync(evt);

                if (result.IsFailed)
                    return Result.Fail<EventDto>(result.Errors);

                _logger.LogInformation("Created event {EventId}", result.Value.Id);
                return Result.Ok(MapToDto(result.Value, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create event");
                return Result.Fail<EventDto>($"Failed to create event: {ex.Message}");
            }
        }

        public async Task<Result<EventDto>> UpdateEventAsync(int id, EventDto eventDto)
        {
            try
            {
                var existingResult = await _eventRepository.GetByIdAsync(id);
                if (existingResult.IsFailed)
                    return Result.Fail<EventDto>(existingResult.Errors);

                var existingEvent = existingResult.Value;

                existingEvent.Title = eventDto.Title;
                existingEvent.Description = eventDto.Description;

                existingEvent.CollegeId = eventDto.CollegeId;
                existingEvent.ProgramId = eventDto.ProgramId;

                existingEvent.CampusLocation = (CampusLocation)eventDto.CampusLocationId;

                existingEvent.StartDate = eventDto.StartDate;
                existingEvent.EndDate = eventDto.EndDate;
                existingEvent.RegistrationDeadline = eventDto.RegistrationDeadline;

                existingEvent.Location = eventDto.Location;
                existingEvent.ImagePath = NormalizeImagePath(eventDto.ImagePath);  

                existingEvent.Priority = eventDto.Priority;
                existingEvent.Type = eventDto.Type;
                existingEvent.Status = eventDto.Status;

                var updateResult = await _eventRepository.UpdateAsync(existingEvent);
                if (updateResult.IsFailed)
                    return Result.Fail<EventDto>(updateResult.Errors);

                _logger.LogInformation("Updated event {EventId}", id);
                return Result.Ok(MapToDto(updateResult.Value, false));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update event {EventId}", id);
                return Result.Fail<EventDto>($"Failed to update event: {ex.Message}");
            }
        }

        public async Task<Result> DeleteEventAsync(int id)
        {
            try
            {
                var result = await _eventRepository.DeleteAsync(id);
                if (result.IsSuccess)
                {
                    _logger.LogInformation("Deleted event {EventId}", id);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete event {EventId}", id);
                return Result.Fail($"Failed to delete event: {ex.Message}");
            }
        }

        public async Task<Result<bool>> ToggleBookmarkEventAsync(int eventId, int userId)
        {
            try
            {
                var isBookmarked = await _eventRepository.ToggleBookmarkAsync(eventId, userId);
                _logger.LogInformation("Bookmark toggled for user {UserId} on event {EventId}: {IsBookmarked}", userId, eventId, isBookmarked);
                return Result.Ok(isBookmarked);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to toggle bookmark for event {EventId}, user {UserId}", eventId, userId);
                return Result.Fail<bool>($"Failed to toggle bookmark: {ex.Message}");
            }
        }

        public async Task<Result> RegisterForEventAsync(int eventId, int userId)
        {
            try
            {
                var result = await _eventRepository.GetByIdAsync(eventId);
                if (result.IsFailed)
                    return Result.Fail(result.Errors);

                var evt = result.Value;

                evt.InterestedCount++;

                var updateResult = await _eventRepository.UpdateAsync(evt);
                if (updateResult.IsSuccess)
                {
                    _logger.LogInformation("User  {User Id} registered for event {EventId}", userId, eventId);
                }
                return updateResult.IsFailed ? Result.Fail(updateResult.Errors) : Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to register for event {EventId}, user {User Id}", eventId, userId);
                return Result.Fail($"Failed to register for event: {ex.Message}");
            }
        }

        public async Task<Result<List<EventDto>>> GetBookmarkedEventsAsync(int userId)
        {
            try
            {
                var result = await _eventRepository.GetBookmarkedEventsAsync(userId);
                if (result.IsFailed)
                    return Result.Fail<List<EventDto>>(result.Errors);

                var dtos = result.Value.Select(evt => MapToDto(evt, true)).ToList(); // all are bookmarked
                return Result.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve bookmarked events for user {UserId}", userId);
                return Result.Fail<List<EventDto>>($"Failed to retrieve bookmarked events: {ex.Message}");
            }
        }

        private static string GetCampusLocationName(CampusLocation loc)
        {
            return loc switch
            {
                CampusLocation.MainCampus => "Main Campus",
                CampusLocation.Congressional => "Congressional Extension Campus",
                CampusLocation.BagongSilang => "Bagong Silang Extension Campus",
                CampusLocation.Camarin => "Camarin Extension Campus",
                _ => "Unknown Campus"
            };
        }

        private EventDto MapToDto(Event evt, bool isBookmarked)
        {
            var normalizedImagePath = NormalizeImagePath(evt.ImagePath);

            return new EventDto
            {
                Id = evt.Id,
                Title = evt.Title,
                Description = evt.Description,

                CollegeId = evt.CollegeId,
                CollegeName = evt.College?.CollegeName ?? string.Empty,

                ProgramId = evt.ProgramId,
                ProgramName = evt.Program?.Name,

                CampusLocationId = (int)evt.CampusLocation,
                CampusLocationName = GetCampusLocationName(evt.CampusLocation),

                StartDate = evt.StartDate,
                EndDate = evt.EndDate,
                Status = CalculateEventStatus(evt.StartDate, evt.EndDate),
                RegistrationDeadline = evt.RegistrationDeadline,

                Location = evt.Location,
                ImagePath = normalizedImagePath,

                Priority = evt.Priority,
                Type = evt.Type,

                InterestedCount = evt.EventBookmarks?.Count ?? 0,
                IsBookmarked = isBookmarked,
                OrganizerName = evt.Organizer?.FullName
            };
        }

        private Event MapToEntity(EventDto dto)
        {
            return new Event
            {
                Title = dto.Title,
                Description = dto.Description,

                CollegeId = dto.CollegeId,
                ProgramId = dto.ProgramId,

                CampusLocation = (CampusLocation)dto.CampusLocationId,

                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                RegistrationDeadline = dto.RegistrationDeadline,

                Location = dto.Location,
                ImagePath = NormalizeImagePath(dto.ImagePath),

                Priority = dto.Priority,
                Type = dto.Type,
                InterestedCount = dto.InterestedCount,
                Status = CalculateEventStatus(dto.StartDate, dto.EndDate),
                CreatedAt = DateTime.UtcNow
            };
        }

        // Helper method to normalize ImagePath (strips prefix, returns filename only)
        private static string NormalizeImagePath(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
                return string.Empty;

            // Extract just the filename (handles prefixed or clean inputs)
            // e.g., "/uploads/events/filename.jpg" → "filename.jpg"
            // or "filename.jpg" → "filename.jpg"
            // or "C:\\path\\to\\file.jpg" → "file.jpg"
            var fileName = Path.GetFileName(imagePath.Trim());

            return fileName ?? string.Empty;  // Fallback if Path.GetFileName returns null
        }

        private static EventStatus CalculateEventStatus(DateTime startDate, DateTime endDate)
        {
            var now = DateTime.Now;
            if (now < startDate)
                return EventStatus.Upcoming;
            else if (now >= startDate && now <= endDate)
                return EventStatus.Ongoing;
            else
                return EventStatus.Ended;
        }
    }
}
