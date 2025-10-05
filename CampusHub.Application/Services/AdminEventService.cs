using CampusHub.Application.DTO;
using CampusHub.Application.DTOs;
using CampusHub.Application.Interfaces;
using CampusHub.Domain.Entities;
using FluentResults;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CampusHub.Application.Services
{
    public class AdminEventService : IAdminEventService
    {
        private readonly IAdminEventRepository _repository;
        private readonly ILogger<AdminEventService> _logger;
        private readonly string _webRootPath;

        public AdminEventService(
            IAdminEventRepository repository,
            ILogger<AdminEventService> logger,
            string webRootPath)
        {
            _repository = repository;
            _logger = logger;
            _webRootPath = webRootPath;
        }

        public async Task<Result<string>> UploadEventImageAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Result.Fail<string>(new Error("No file uploaded")
                        .WithMetadata("StatusCode", 400));
                }

                const long maxFileSize = 5 * 1024 * 1024;
                if (file.Length > maxFileSize)
                {
                    return Result.Fail<string>(new Error("File size must be less than 5MB")
                        .WithMetadata("StatusCode", 400));
                }

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(extension))
                {
                    return Result.Fail<string>(new Error("Invalid file type. Only images (JPEG, PNG, GIF, WebP) are allowed.")
                        .WithMetadata("StatusCode", 400));
                }

                var uploadsFolder = Path.Combine(_webRootPath, "uploads", "events");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // FIXED: Return JUST the filename (no path prefix!)
                // Log the full relative path if needed for debugging
                var relativePathForLog = $"/uploads/events/{uniqueFileName}";
                _logger.LogInformation("Image uploaded successfully: {FileName} -> {RelativePath}", file.FileName, relativePathForLog);

                return Result.Ok(uniqueFileName);  // e.g., "a52ff62f-3d5b-4336-a553-8a385025ee55.jpg"
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading image: {FileName}", file?.FileName);
                return Result.Fail<string>(new Error("An error occurred while uploading the image")
                    .CausedBy(ex));
            }
        }
        public async Task<Result<List<EventDto>>> GetAllEventsAsync()
        {
            try
            {
                var result = await _repository.GetAllEventsAsync();

                if (result.IsFailed)
                    return Result.Fail<List<EventDto>>(result.Errors);

                var dtos = result.Value.Select(MapToDto).ToList();
                return Result.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllEventsAsync service method");
                return Result.Fail<List<EventDto>>(new Error("An error occurred while retrieving events")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<EventDto>> GetEventByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return Result.Fail<EventDto>(new Error("Invalid event ID")
                        .WithMetadata("StatusCode", 400));

                var result = await _repository.GetEventByIdAsync(id);

                if (result.IsFailed)
                    return Result.Fail<EventDto>(result.Errors);

                return Result.Ok(MapToDto(result.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetEventByIdAsync service method for event {EventId}", id);
                return Result.Fail<EventDto>(new Error($"An error occurred while retrieving event {id}")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<EventDto>> CreateEventAsync(EventDto eventDto)
        {
            try
            {
                var validation = ValidateEventDto(eventDto);
                if (validation.IsFailed)
                    return Result.Fail<EventDto>(validation.Errors);

                var evt = MapToEntity(eventDto);
                var result = await _repository.CreateEventAsync(evt);

                if (result.IsFailed)
                    return Result.Fail<EventDto>(result.Errors);

                _logger.LogInformation("Event created successfully: {EventTitle}", evt.Title);
                return Result.Ok(MapToDto(result.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateEventAsync service method");
                return Result.Fail<EventDto>(new Error("An error occurred while creating the event")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<EventDto>> UpdateEventAsync(int id, EventDto eventDto)
        {
            try
            {
                if (id <= 0)
                    return Result.Fail<EventDto>(new Error("Invalid event ID")
                        .WithMetadata("StatusCode", 400));

                var validation = ValidateEventDto(eventDto);
                if (validation.IsFailed)
                    return Result.Fail<EventDto>(validation.Errors);

                var existingResult = await _repository.GetEventByIdAsync(id);
                if (existingResult.IsFailed)
                    return Result.Fail<EventDto>(existingResult.Errors);

                var existingEvent = existingResult.Value;
                UpdateEventEntity(existingEvent, eventDto);

                var updateResult = await _repository.UpdateEventAsync(existingEvent);
                if (updateResult.IsFailed)
                    return Result.Fail<EventDto>(updateResult.Errors);

                _logger.LogInformation("Event updated successfully: {EventId}", id);
                return Result.Ok(MapToDto(updateResult.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateEventAsync service method for event {EventId}", id);
                return Result.Fail<EventDto>(new Error($"An error occurred while updating event {id}")
                    .CausedBy(ex));
            }
        }

        public async Task<Result> DeleteEventAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return Result.Fail(new Error("Invalid event ID")
                        .WithMetadata("StatusCode", 400));

                var result = await _repository.DeleteEventAsync(id);

                if (result.IsFailed)
                    return Result.Fail(result.Errors);

                _logger.LogInformation("Event deleted successfully: {EventId}", id);
                return Result.Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteEventAsync service method for event {EventId}", id);
                return Result.Fail(new Error($"An error occurred while deleting event {id}")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<CollegeDto>>> GetAllCollegesAsync()
        {
            try
            {
                var result = await _repository.GetAllCollegesAsync();

                if (result.IsFailed)
                    return Result.Fail<List<CollegeDto>>(result.Errors);

                var dtos = result.Value.Select(c => new CollegeDto
                {
                    CollegeId = c.CollegeId,
                    CollegeName = c.CollegeName
                }).ToList();

                return Result.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCollegesAsync service method");
                return Result.Fail<List<CollegeDto>>(new Error("An error occurred while retrieving colleges")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<AdminProgramDto>>> GetAllProgramsAsync()
        {
            try
            {
                var result = await _repository.GetAllProgramsAsync();

                if (result.IsFailed)
                    return Result.Fail<List<AdminProgramDto>>(result.Errors);

                var dtos = result.Value.Select(p => new AdminProgramDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CollegeId = p.CollegeId
                }).ToList();

                return Result.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllProgramsAsync service method");
                return Result.Fail<List<AdminProgramDto>>(new Error("An error occurred while retrieving programs")
                    .CausedBy(ex));
            }
        }

        public async Task<Result<List<AdminProgramDto>>> GetProgramsByCollegeAsync(int collegeId)
        {
            try
            {
                if (collegeId <= 0)
                    return Result.Fail<List<AdminProgramDto>>(new Error("Invalid college ID")
                        .WithMetadata("StatusCode", 400));

                var result = await _repository.GetProgramsByCollegeAsync(collegeId);

                if (result.IsFailed)
                    return Result.Fail<List<AdminProgramDto>>(result.Errors);

                var dtos = result.Value.Select(p => new AdminProgramDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    CollegeId = p.CollegeId
                }).ToList();

                return Result.Ok(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetProgramsByCollegeAsync service method for college {CollegeId}", collegeId);
                return Result.Fail<List<AdminProgramDto>>(new Error($"An error occurred while retrieving programs for college {collegeId}")
                    .CausedBy(ex));
            }
        }

        private Result ValidateEventDto(EventDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Event title is required");

            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Event description is required");

            if (dto.CollegeId <= 0)
                errors.Add("Valid college is required");

            if (string.IsNullOrWhiteSpace(dto.Location))
                errors.Add("Event location is required");

            if (dto.Date == default)
                errors.Add("Event date is required");

            if (string.IsNullOrWhiteSpace(dto.Type))
                errors.Add("Event type is required");

            if (errors.Any())
                return Result.Fail(new Error(string.Join("; ", errors))
                    .WithMetadata("StatusCode", 400));

            return Result.Ok();
        }

        private void UpdateEventEntity(Event existingEvent, EventDto dto)
        {
            existingEvent.Title = dto.Title;
            existingEvent.Description = dto.Description;
            existingEvent.CollegeId = dto.CollegeId;
            existingEvent.ProgramId = dto.ProgramId;
            existingEvent.CampusLocation = (CampusLocation)dto.CampusLocationId;
            existingEvent.Date = dto.Date;
            existingEvent.RegistrationDeadline = dto.RegistrationDeadline;
            existingEvent.Location = dto.Location;
            existingEvent.ImagePath = dto.ImagePath;
            existingEvent.Priority = dto.Priority;
            existingEvent.Type = dto.Type;
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
                ImagePath = evt.ImagePath,
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
                ImagePath = dto.ImagePath,
                Priority = dto.Priority,
                Type = dto.Type,
                InterestedCount = 0
            };
        }
    }
}