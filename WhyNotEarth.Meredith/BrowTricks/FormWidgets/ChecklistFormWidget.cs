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
            result.Append("<section class=\"section\">");
            result.Append($"<p class=\"question mb-2\">{_value}</p>");

            foreach (var option in _options)
            {
                result.Append("<div class=\"flex items-center mb-1\">");

                var isChecked = _values.Contains(option);

                if (isChecked)
                {
                    result.Append(@"<img
                    src=""https://res.cloudinary.com/whynotearth/image/upload/v1600070035/BrowTricks/static_backend/boxtick_hxjopo.svg""
                    class=""icon-boxtick mr-2""
                    alt="""" />");
                }
                else
                {
                    result.Append(@"<img
                    src=""https://res.cloudinary.com/whynotearth/image/upload/v1600070035/BrowTricks/static_backend/checkbox_hhfv3l.svg""
                    class=""icon-checkbox mr-2""
                    alt="""" />");
                }

                result.Append($"<span>{option}</span>");
                result.Append("</div>");
            }

            result.Append("</section>");

            return result.ToString();
        }
    }
}