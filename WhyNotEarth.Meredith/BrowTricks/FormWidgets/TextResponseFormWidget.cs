using System.Linq;

namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class TextResponseFormWidget : IFormWidget
    {
        private readonly string _answer;
        private readonly bool _hasAnswer;
        private readonly string _value;

        public TextResponseFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _hasAnswer = false;
            _answer = string.Empty;
        }

        public TextResponseFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
            _answer = formAnswer.Answers?.FirstOrDefault() ?? string.Empty;
        }

        public string Render()
        {
            if (!_hasAnswer)
            {
                return $"<p class=\"paragraph\">{_value}</p>";
            }

            return $"<p class=\"paragraph\">{_value}</p><p class=\"paragraph\">{_answer}</p>";
        }
    }
}