using System.Linq;

namespace WhyNotEarth.Meredith.BrowTricks.FormWidgets
{
    internal class PdfFormWidget : IFormWidget
    {
        private readonly bool _hasAnswer;
        private readonly string _value;
        private readonly string _url;

        public PdfFormWidget(FormItem formItem)
        {
            _value = formItem.Value;
            _hasAnswer = false;
            _url = formItem.Options?.FirstOrDefault() ?? string.Empty;
        }

        public PdfFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
            _url = formAnswer.Options?.FirstOrDefault() ?? string.Empty;
        }

        public string Render()
        {
            if (_hasAnswer)
            {
                return $@"
<section class=""section"">
    <p class=""question mb-2"">{_value}</p>
    <object class=""mb-2 mt-4"" data=""{_url}"" type=""application/pdf"" width=""100%"" height=""100%"" />
    <p class=""flex items-center"">
        <img
            class=""icon-boxtick mr-2""
            src=""https://res.cloudinary.com/whynotearth/image/upload/v1600070035/BrowTricks/static_backend/boxtick_hxjopo.svg""
            alt="""" />
        <span>Accepted</span>
    </p>
</section>";
            }

            return $@"
<section class=""section"">
    <p class=""question mb-2"">{_value}</p>
    <object class=""mb-2 mt-4"" data=""{_url}"" type=""application/pdf"" width=""100%"" height=""100%"" />
</section>";
        }
    }
}