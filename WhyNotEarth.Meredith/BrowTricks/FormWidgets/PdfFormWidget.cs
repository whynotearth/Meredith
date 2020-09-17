using System;
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
            _url = ConvertUrl(formItem.Options?.FirstOrDefault());
        }

        public PdfFormWidget(FormAnswer formAnswer)
        {
            _value = formAnswer.Question;
            _hasAnswer = true;
            _url = ConvertUrl(formAnswer.Options?.FirstOrDefault());
        }

        private string ConvertUrl(string? url)
        {
            // This seems very iffy
            // I tried using the Cloudinary library but couldn't figure out how to do it
            // It was just returning the PDF URL again
            if (url is null)
            {
                return string.Empty;
            }

            url = url.ToLower();

            var index = url.LastIndexOf(".pdf", StringComparison.Ordinal);

            if (index == -1)
            {
                return url;
            }

            return url.Remove(index, ".pdf".Length).Insert(index, ".png");
        }

        public string Render()
        {
            if (_hasAnswer)
            {
                return $@"
<section class=""section"">
    <p class=""question mb-2"">{_value}</p>
    <img class=""mb-2 mt-4"" src=""{_url}"" alt="""" />
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
    <img class=""mb-2 mt-4"" src=""{_url}"" alt="""" />
</section>";
        }
    }
}