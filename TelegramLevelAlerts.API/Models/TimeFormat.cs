namespace TelegramLevelAlerts.API.Models
{
    public class TimeFormat
    {
        public int Value { get; set; }
        private TimeFormat(int value)
        {
            Value = value;
        }
        public static TimeFormat Seconds => new(0);
        public static TimeFormat Minutes => new(1);
        public static TimeFormat Hours => new(2);
        public static TimeFormat Days => new(3);
    }
}
