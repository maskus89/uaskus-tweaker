namespace UaskusTweaks.Models;

public enum LogLevel { Info, Success, Warning, Error }

public class LogEntry
{
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public string Message { get; set; } = string.Empty;
    public LogLevel LogLevel { get; set; } = LogLevel.Info;
}
