﻿namespace Api.Models
{
    public class MessageRequest
    {
        public string? Message { get; set; }
        public string? To { get; set; }
        public string? From { get; set; }
        public int TimeToLifeSec { get; set; }

    }
}
