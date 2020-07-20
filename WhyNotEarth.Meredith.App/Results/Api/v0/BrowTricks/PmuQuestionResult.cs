using WhyNotEarth.Meredith.Data.Entity.Models.Modules.BrowTricks;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.BrowTricks
{
    public class PmuQuestionResult
    {
        public int Id { get; }

        public string Question { get; }

        public PmuQuestionResult(PmuQuestion pmuQuestion)
        {
            Id = pmuQuestion.Id;
            Question = pmuQuestion.Question;
        }
    }
}