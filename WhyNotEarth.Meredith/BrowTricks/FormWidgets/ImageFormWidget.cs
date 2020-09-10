using System.Linq;

namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class ImageFormWidget : IFormWidget
    {
        private readonly bool _hasAnswer;
        private readonly string _value;
        private readonly string _url;

        public ImageFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _hasAnswer = false;
            _url = formItem.Options?.FirstOrDefault() ?? string.Empty;
        }

        public ImageFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
            _url = formAnswer.Options?.FirstOrDefault() ?? string.Empty;
        }

        public string Render()
        {
            return
                $"<img src=\"{_url}\">" +
                $"<input type=\"checkbox\" checked=\"{_hasAnswer}\">" +
                $"<p class=\"paragraph\">{_value}</p>";
        }
    }
}