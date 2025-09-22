using System;
using System.Text.Json;
using System.Threading.Tasks;
using Hangfire;
using ModuWeb.Api.Services;

namespace ModuWeb.Api.Services
{
    public interface IReminderService
    {
        void ScheduleReminder(string contentJson);
    }

    public class ReminderService : IReminderService
    {
        private readonly SmtpEmailService _emailService;
        public ReminderService(SmtpEmailService emailService)
        {
            _emailService = emailService;
        }

        public void ScheduleReminder(string contentJson)
        {
            Console.WriteLine($"[ReminderService] ScheduleReminder called at {DateTime.UtcNow:O}");
            Console.WriteLine($"[ReminderService] contentJson: {contentJson}");
            // Parse booking info from contentJson
            var doc = JsonDocument.Parse(contentJson);
            var root = doc.RootElement;
            // Handle double-serialized content (if 'content' property exists and is a string)
            if (root.TryGetProperty("content", out var contentProp) && contentProp.ValueKind == JsonValueKind.String)
            {
                var innerJson = contentProp.GetString();
                if (string.IsNullOrWhiteSpace(innerJson))
                {
                    Console.WriteLine("[ReminderService] Inner content string is null or empty, aborting.");
                    return;
                }
                Console.WriteLine($"[ReminderService] Detected inner content, reparsing: {innerJson}");
                doc = JsonDocument.Parse(innerJson);
                root = doc.RootElement;
            }

            if (!root.TryGetProperty("reminderTime", out var reminderTimeProp) ||
                !root.TryGetProperty("email", out var emailProp) ||
                !root.TryGetProperty("name", out var nameProp))
            {
                Console.WriteLine("[ReminderService] Missing required fields in contentJson");
                return;
            }

            if (!DateTime.TryParse(reminderTimeProp.GetString(), out var reminderTimeUtc))
            {
                Console.WriteLine("[ReminderService] Invalid reminderTime format");
                return;
            }

            // Convert UTC to local server time
            var reminderTimeLocal = reminderTimeUtc.ToLocalTime();

            var to = emailProp.GetString();
            var subject = "Booking Reminder";
            var body = $"Hello {nameProp.GetString()}, this is your booking reminder.";

            // Null checks before scheduling job
            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
            {
                Console.WriteLine("[ReminderService] Null or empty email parameters, skipping job");
                return;
            }

            Console.WriteLine($"[ReminderService] Scheduling reminder for {to} at {reminderTimeLocal:O} (local time)");
            // Schedule Hangfire job
            BackgroundJob.Schedule(() => SendReminderEmail(to, subject, body), reminderTimeLocal - DateTime.Now);
        }

        // This method must be public and static or instance for Hangfire
        public async Task SendReminderEmail(string to, string subject, string body)
        {
            Console.WriteLine($"[ReminderService] SendReminderEmail called for {to} at {DateTime.UtcNow:O}");
            if (string.IsNullOrWhiteSpace(to) || string.IsNullOrWhiteSpace(subject) || string.IsNullOrWhiteSpace(body))
            {
                Console.WriteLine("[ReminderService] Null or empty email parameters in SendReminderEmail, skipping send");
                return;
            }
            await _emailService.SendEmailAsync(to, subject, body);
            Console.WriteLine($"[ReminderService] Email sent to {to}");
        }
    }
}
