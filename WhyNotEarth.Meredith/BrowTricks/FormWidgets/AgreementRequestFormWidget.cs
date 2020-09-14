namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class AgreementRequestFormWidget : IFormWidget
    {
        private readonly bool _hasAnswer;
        private readonly string _value;

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
            if (_hasAnswer)
            {
                return
$@"<section class=""section"">
    <p class=""question mb-2"">{_value}</p>
    <p class=""flex items-center"">
        <img class=""icon-boxtick mr-2""
            src=""https://res.cloudinary.com/whynotearth/image/upload/v1600070035/BrowTricks/static_backend/boxtick_hxjopo.svg""
            alt="""" />
        <span>Accepted</span>
    </p>
</section>";
            }

            return
$@"<section class=""section"">
    <p class=""question mb-2"">{_value}</p>
</section>";
        }
    }
}