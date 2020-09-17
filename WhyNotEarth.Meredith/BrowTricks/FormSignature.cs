using System;
using System.Collections.Generic;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class FormSignature
    {
        public int Id { get; set; }

        public int FormTemplateId { get; set; }

        public FormTemplate FormTemplate { get; set; } = null!;

        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        public string Name { get; set; } = null!;

        public List<FormAnswer> Answers { get; set; } = new List<FormAnswer>();

        public string? NotificationCallBackUrl { get; set; }

        public string? Html { get; set; }

        public string? PdfPath { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}