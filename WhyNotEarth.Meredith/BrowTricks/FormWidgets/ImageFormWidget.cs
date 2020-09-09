namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class ImageFormWidget : IFormWidget
    {
        private readonly bool _hasAnswer;
        private readonly string _value;

        public ImageFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _hasAnswer = false;
        }

        public ImageFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
        }

        public string Render()
        {
            return
                $"<img src=\"{_value}\">" +
                $"<input type=\"checkbox\" checked=\"{_hasAnswer}\">" +
                "<p class=\"paragraph\">" +
                    "By selecting this box, I agree that I have read and understood the above file and agree to the terms stated." +
                "</p>";
        }
    }
}