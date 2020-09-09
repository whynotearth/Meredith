namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class AgreementRequestFormWidget : IFormWidget
    {
        private readonly string _value;
        private readonly bool _hasAnswer;

        public AgreementRequestFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _hasAnswer = false;
        }

        public AgreementRequestFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
        }

        public string Render()
        {
            return $"<p class=\"paragraph\"><input type=\"checkbox\" checked=\"{_hasAnswer}\">{_value}</p>";
        }
    }
}