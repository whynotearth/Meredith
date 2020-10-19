using System.Collections.Generic;
using System.Text;

namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class PdfFormWidget : IFormWidget
    {
        private readonly bool _hasAnswer;
        private readonly List<string> _urls;
        private readonly string _value;

        public PdfFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _hasAnswer = false;
            _urls = formItem.Options ?? new List<string>();
        }

        public PdfFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
            _urls = formAnswer.Options ?? new List<string>();
        }

        public string Render()
        {
            var result = new StringBuilder();

            result.AppendLine(
                "<section class=\"section\">" +
                $"<p class=\"question mb-2\">{_value}</p>"
            );

            foreach (var url in _urls)
            {
                result.AppendLine($"<img class=\"mb-2 mt-4\" src=\"{url}\" alt=\"\" />");
            }

            if (_hasAnswer)
            {
                result.AppendLine(
                    "<p class=\"flex items-center\">" +
                        "<img" +
                            "class=\"icon-boxtick mr-2\"" +
                            "src=\"https://res.cloudinary.com/whynotearth/image/upload/v1600070035/BrowTricks/static_backend/boxtick_hxjopo.svg\"" +
                            "alt=\"\" />" +
                        "<span>Accepted</span>" +
                    "</p>"
                );
            }

            result.AppendLine("</section>");

            return result.ToString();
        }
    }
}