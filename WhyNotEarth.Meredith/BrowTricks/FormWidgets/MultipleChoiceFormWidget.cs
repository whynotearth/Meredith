using System;
using System.Collections.Generic;
using System.Text;

namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class MultipleChoiceFormWidget : IFormWidget
    {
        private readonly List<string> _options;
        private readonly string _value;
        private readonly List<string> _values;

        public MultipleChoiceFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _options = new List<string>();
            _values = new List<string>();
        }

        public MultipleChoiceFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _options = formAnswer.Options ?? new List<string>();
            _values = formAnswer.Answers ?? new List<string>();
        }

        public string Render()
        {
            var group = Guid.NewGuid();

            var result = new StringBuilder();
            result.Append($"<p class=\"paragraph\">{_value}</p>");
            result.Append("<div>");

            foreach (var option in _options)
            {
                var id = Guid.NewGuid();

                result.Append(
                    $"<input type=\"radio\" id=\"{id}\" name=\"{group}\" value=\"{option}\" checked=\"{_values.Contains(option)}\">" +
                    $"<label for=\"{id}\">{option}</label>>");
            }

            result.Append("</div>");
            return result.ToString();
        }
    }
}