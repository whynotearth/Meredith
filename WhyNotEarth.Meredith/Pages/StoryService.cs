namespace WhyNotEarth.Meredith.Pages
{
    using System;
    using WhyNotEarth.Meredith.Data.Entity.Models;

    public class StoryService
    {

        public string GetCardType(Card.CardTypes cardType)
        {
            return cardType switch
            {
                Card.CardTypes.Card => "story-card",
                _ => throw new Exception($"Card type {cardType} not mapped."),
            };
        }
    }
}