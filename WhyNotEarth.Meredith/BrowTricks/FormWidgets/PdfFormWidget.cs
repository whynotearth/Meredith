namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class PdfFormWidget : IFormWidget
    {
        private readonly bool _hasAnswer;
        private readonly string _value;

        public PdfFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _hasAnswer = false;
        }

        public PdfFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
        }

        public string Render()
        {
            return
                $"<object data=\"{_value}\" type=\"application/pdf\" width=\"100%\" height=\"100%\">" +
                $"<input type=\"checkbox\" checked=\"{_hasAnswer}\">" +
                "<p class=\"paragraph\">" +
                    "By selecting this box, I agree that I have read and understood the above file and agree to the terms stated." +
                "</p>";
        }
    }
}