using System.Collections.Generic;
using System.Linq;

namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class PdfFormWidget : IFormWidget
    {
        private readonly bool _hasAnswer;
        private readonly string _value;
        private readonly string _url;

        public PdfFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _hasAnswer = false;
            _url = formItem.Options?.FirstOrDefault() ?? string.Empty;
        }

        public PdfFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
            _url = formAnswer.Options?.FirstOrDefault() ?? string.Empty;
        }

        public string Render()
        {
            return
                $"<object data=\"{_url}\" type=\"application/pdf\" width=\"100%\" height=\"100%\">" +
                $"<input type=\"checkbox\" checked=\"{_hasAnswer}\">" +
                $"<p class=\"paragraph\">{_value}</p>";
        }
    }
}