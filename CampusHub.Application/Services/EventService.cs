using CampusHub.Application.DTOs;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.Extensions.Logging;
using System.IO;

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

        public async Task<Result<List<EventDto>>> GetAllEventsAsync()
        {
            try
            {
                var result = await _eventRepository.GetAllAsync();
                if (result.IsFailed)
                    return Result.Fail<List<EventDto>>(result.Errors);

                var dtos = result.Value.Select(MapToDto).ToList();
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

                return Result.Ok(MapToDto(result.Value));
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

                var dtos = result.Value.Select(MapToDto).ToList();
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
                return Result.Ok(MapToDto(result.Value));
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

                existingEvent.Date = eventDto.Date;
                existingEvent.RegistrationDeadline = eventDto.RegistrationDeadline;

                existingEvent.Location = eventDto.Location;
                existingEvent.ImagePath = NormalizeImagePath(eventDto.ImagePath);  

                existingEvent.Priority = eventDto.Priority;
                existingEvent.Type = eventDto.Type;

                var updateResult = await _eventRepository.UpdateAsync(existingEvent);
                if (updateResult.IsFailed)
                    return Result.Fail<EventDto>(updateResult.Errors);

                _logger.LogInformation("Updated event {EventId}", id);
                return Result.Ok(MapToDto(updateResult.Value));
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

        public async Task<Result> ToggleBookmarkEventAsync(int eventId, int userId)
        {
            try
            {
                var eventResult = await _eventRepository.GetByIdAsync(eventId);
                if (eventResult.IsFailed)
                    return Result.Fail(eventResult.Errors);
                _logger.LogInformation("Bookmark toggled for user {User Id} on event {EventId} (TODO: Implement full logic)", userId, eventId);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to toggle bookmark for event {EventId}, user {User Id}", eventId, userId);
                return Result.Fail($"Failed to toggle bookmark: {ex.Message}");
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

        private EventDto MapToDto(Event evt)
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

                Date = evt.Date,
                RegistrationDeadline = evt.RegistrationDeadline,

                Location = evt.Location,
                ImagePath = normalizedImagePath,  

                Priority = evt.Priority,
                Type = evt.Type,

                InterestedCount = evt.InterestedCount,
                IsBookmarked = false,  
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

                Date = dto.Date,
                RegistrationDeadline = dto.RegistrationDeadline,

                Location = dto.Location,
                ImagePath = NormalizeImagePath(dto.ImagePath),  

                Priority = dto.Priority,
                Type = dto.Type,
                InterestedCount = dto.InterestedCount
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
    }
}