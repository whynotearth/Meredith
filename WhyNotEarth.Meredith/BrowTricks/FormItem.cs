using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WhyNotEarth.Meredith.BrowTricks
{
    public class FormItem
    {
        public int Id { get; set; }

        public int FormTemplateId { get; set; }

        public FormTemplate FormTemplate { get; set; } = null!;

        public int Order { get; set; }

        public FormItemType Type { get; set; }

        public bool IsRequired { get; set; }

        public string Value { get; set; } = null!;

        public List<string>? Options { get; set; }
    }

    public enum FormItemType : byte
    {
        [EnumMember(Value = "text")]
        Text = 1,

        [EnumMember(Value = "agreement_request")]
        AgreementRequest = 2,

        [EnumMember(Value = "text_response")]
        TextResponse = 3,

        [EnumMember(Value = "checklist")]
        Checklist = 4,

        [EnumMember(Value = "multiple_choice")]
        MultipleChoice = 5,

        [EnumMember(Value = "image")]
        Image = 6,

        [EnumMember(Value = "pdf")]
        Pdf = 7
    }
}