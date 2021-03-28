using System;

namespace TelegramLevelAlerts.API.Models
{
    public class TimeFormat
    {
        public int Value { get; set; } = 1;
        private TimeFormat(int value)
        {
            Value = value;
        }

        public TimeFormat() { }
        public static TimeFormat Seconds => new(0);
        public static TimeFormat Minutes => new(1);
        public static TimeFormat Hours => new(2);
        public static TimeFormat Days => new(3);

        public override bool Equals(object obj)
        {
            var timeFormat = obj as TimeFormat;

            return timeFormat.Value == Value;
        }

        public override int GetHashCode() =>  base.GetHashCode();
    }
}
