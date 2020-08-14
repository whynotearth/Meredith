namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.SendGrid
{
    public class EmailStatsResult
    {
        public int MonthlyActiveUsers { get; }
        public int MonthlySentEmails { get; }

        public EmailStatsResult(int monthlyActiveUsers, int monthlySentEmails)
        {
            MonthlyActiveUsers = monthlyActiveUsers;
            MonthlySentEmails = monthlySentEmails;
        }
    }
}