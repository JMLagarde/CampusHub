using CampusHub.Application.DTO;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace CampusHub.Application.Helpers
{
    public static class ValidationHelper
    {
        public static ValidationResult ValidateCreateMarketplaceItem(CreateMarketplaceItemDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Title is required");
            else if (dto.Title.Length > 100)
                errors.Add("Title cannot exceed 100 characters");

            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Description is required");
            else if (dto.Description.Length > 500)
                errors.Add("Description cannot exceed 500 characters");

            if (dto.Price <= 0)
                errors.Add("Price must be greater than 0");
            else if (dto.Price > 999999)
                errors.Add("Price cannot exceed ₱999,999");

            if (!string.IsNullOrEmpty(dto.ImageUrl) && !IsValidUrl(dto.ImageUrl))
                errors.Add("Please enter a valid URL");

            if (!string.IsNullOrEmpty(dto.ContactNumber) && !IsValidPhoneNumber(dto.ContactNumber))
                errors.Add("Please enter a valid contact number");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        public static ValidationResult ValidateUpdateMarketplaceItem(UpdateMarketplaceItemDto dto)
        {
            var errors = new List<string>();

            if (dto.Id <= 0)
                errors.Add("Invalid item ID");

            if (string.IsNullOrWhiteSpace(dto.Title))
                errors.Add("Title is required");
            else if (dto.Title.Length > 100)
                errors.Add("Title cannot exceed 100 characters");

            if (string.IsNullOrWhiteSpace(dto.Description))
                errors.Add("Description is required");
            else if (dto.Description.Length > 500)
                errors.Add("Description cannot exceed 500 characters");

            if (dto.Price <= 0)
                errors.Add("Price must be greater than 0");
            else if (dto.Price > 999999)
                errors.Add("Price cannot exceed ₱999,999");

            if (!string.IsNullOrEmpty(dto.ImageUrl) && !IsValidUrl(dto.ImageUrl))
                errors.Add("Please enter a valid URL");

            if (!string.IsNullOrEmpty(dto.ContactNumber) && !IsValidPhoneNumber(dto.ContactNumber))
                errors.Add("Please enter a valid contact number");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        public static ValidationResult ValidateCreateUser(CreateUserDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Username))
                errors.Add("Username is required");
            else if (dto.Username.Length > 50)
                errors.Add("Username cannot exceed 50 characters");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                errors.Add("Full name is required");
            else if (dto.FullName.Length > 100)
                errors.Add("Full name cannot exceed 100 characters");

            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required");
            else if (dto.Password.Length < 6 || dto.Password.Length > 100)
                errors.Add("Password must be between 6 and 100 characters");

            if (!string.IsNullOrEmpty(dto.Email) && !IsValidEmail(dto.Email))
                errors.Add("Please enter a valid email address");

            if (!string.IsNullOrEmpty(dto.ContactNumber) && !IsValidPhoneNumber(dto.ContactNumber))
                errors.Add("Please enter a valid contact number");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        public static ValidationResult ValidateLogin(LoginDto dto)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(dto.Username))
                errors.Add("Username is required");

            if (string.IsNullOrWhiteSpace(dto.Password))
                errors.Add("Password is required");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        public static ValidationResult ValidateToggleLike(ToggleLikeDto dto)
        {
            var errors = new List<string>();

            if (dto.ItemId <= 0 && dto.MarketplaceItemId <= 0)
                errors.Add("Valid item ID is required");

            if (dto.UserId <= 0)
                errors.Add("Valid user ID is required");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        public static ValidationResult ValidateItemStatusOperation(ItemStatusOperationDto dto)
        {
            var errors = new List<string>();

            if (dto.ItemId <= 0)
                errors.Add("Valid item ID is required");

            if (dto.UserId <= 0)
                errors.Add("Valid user ID is required");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        public static ValidationResult ValidateUpdateUserProfile(UpdateUserProfileDto dto)
        {
            var errors = new List<string>();

            if (dto.Id <= 0)
                errors.Add("Valid user ID is required");

            if (string.IsNullOrWhiteSpace(dto.FullName))
                errors.Add("Full name is required");
            else if (dto.FullName.Length > 100)
                errors.Add("Full name cannot exceed 100 characters");

            if (!string.IsNullOrEmpty(dto.Email) && !IsValidEmail(dto.Email))
                errors.Add("Please enter a valid email address");

            if (!string.IsNullOrEmpty(dto.ContactNumber) && !IsValidPhoneNumber(dto.ContactNumber))
                errors.Add("Please enter a valid contact number");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }

        private static bool IsValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result)
                && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private static bool IsValidPhoneNumber(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^[\d\s\+\-\(\)]+$") && phoneNumber.Any(char.IsDigit);
        }
        public static ValidationResult ValidateCreateReport(CreateReportDto dto)
        {
            var errors = new List<string>();

            if (dto.MarketplaceItemId <= 0)
                errors.Add("Valid marketplace item ID is required");

            if (dto.ReporterId <= 0)
                errors.Add("Valid reporter ID is required");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                errors.Add("Reason is required");
            else if (dto.Reason.Length > 500)
                errors.Add("Reason cannot exceed 500 characters");

            if (!string.IsNullOrEmpty(dto.Description) && dto.Description.Length > 1000)
                errors.Add("Description cannot exceed 1000 characters");

            return new ValidationResult
            {
                IsValid = !errors.Any(),
                Errors = errors
            };
        }
    }

    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}