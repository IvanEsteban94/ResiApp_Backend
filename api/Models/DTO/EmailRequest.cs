﻿namespace api.Models.DTO
{
    public class EmailRequest
    {
        public string To { get; set; }            
        public string Subject { get; set; }         
        public string Body { get; set; }
    }
}
