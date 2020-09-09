using System;
using System.Collections.Generic;
using System.Text;

namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class ChecklistFormWidget : IFormWidget
    {
        private readonly List<string> _options;
        private readonly string _value;
        private readonly List<string> _values;

        public ChecklistFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _options = new List<string>();
            _values = new List<string>();
        }

        public ChecklistFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _options = formAnswer.Options ?? new List<string>();
            _values = formAnswer.Answers ?? new List<string>();
        }

        public string Render()
        {
            var result = new StringBuilder();
            result.Append($"<p class=\"paragraph\">{_value}</p>");

            foreach (var option in _options)
            {
                var id = Guid.NewGuid();

                result.Append("<div>");

                var isChecked = _values.Contains(option) ? "checked" : string.Empty;
                result.Append($"<input type=\"checkbox\" id=\"{id}\" name=\"{option}\" {isChecked}>");

                result.Append($"<label for=\"{id}\">{option}</label>");

                result.Append("</div>");
            }

            return result.ToString();
        }
    }
}