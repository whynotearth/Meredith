namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class TextFormWidget : IFormWidget
    {
        private readonly string _value;

        public TextFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
        }

        public TextFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
        }

        public string Render()
        {
            return $"<p class=\"paragraph\">{_value}</p>";
        }
    }
}