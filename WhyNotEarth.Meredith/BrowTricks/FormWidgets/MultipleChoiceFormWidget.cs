using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class MultipleChoiceFormWidget : IFormWidget
    {
        private readonly bool _hasAnswer;
        private readonly List<string> _options;
        private readonly string _value;
        private readonly List<string> _values;

        public MultipleChoiceFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _options = new List<string>();
            _values = new List<string>();
            _hasAnswer = false;
        }

        public MultipleChoiceFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _options = formAnswer.Options ?? new List<string>();
            _values = formAnswer.Answers ?? new List<string>();
            _hasAnswer = true;
        }

        public string Render()
        {
            if (_hasAnswer)
            {
                return $@"<section class=""section"">
                        <p class=""question"">{_value}</p>
                        <p>{_values.FirstOrDefault()}</p>
                        </section>";
            }

            var result = new StringBuilder();
            result.Append("<section class=\"section\">");
            result.Append($"<p class=\"question mb-2\">{_value}</p>");

            foreach (var option in _options)
            {
                result.Append("<div class=\"flex items-center mb-1\">");

                result.Append(@"<img
                src=""https://res.cloudinary.com/whynotearth/image/upload/v1600070035/BrowTricks/static_backend/radio_ogjfjl.svg""
                class=""icon-radio mr-2""
                alt="""" />");

                result.Append($"<span>{option}</span>");
                result.Append("</div>");
            }

            result.Append("</section>");

            return result.ToString();
        }
    }
}