using System;
using System.Collections.Generic;
using System.Linq;
using WhyNotEarth.Meredith.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class FormTemplateResult
    {
        public int? Id { get; }

        public string Name { get; }

        public List<FormItemResult>? Items { get; }

        public DateTime? CreatedAt { get; }

        public FormTemplateResult(FormTemplate formTemplate)
        {
            Id = formTemplate.Id;
            Name = formTemplate.Name;
            Items = formTemplate.Items.Select(item => new FormItemResult(item)).ToList();
            CreatedAt = formTemplate.CreatedAt;
        }
    }

    public class FormItemResult
    {
        public int Id { get; }

        public FormItemType Type { get; }

        public bool IsRequired { get; }

        public string Value { get; }

        public List<string>? Options { get; }

        public FormItemResult(FormItem formItem)
        {
            Id = formItem.Id;
            Type = formItem.Type;
            IsRequired = formItem.IsRequired;
            Value = formItem.Value;
            Options = formItem.Options;
        }
    }
}