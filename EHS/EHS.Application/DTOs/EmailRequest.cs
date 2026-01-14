namespace EHS.Application.DTOs
{
    public class EmailRequest
    {
        public string ToEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; } // Plain text or prepocessed HTML
        public string TemplateName { get; set; } // For Razor templates
        public object TemplateModel { get; set; } // Data for the template
    }
}
