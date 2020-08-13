using System;
using WhyNotEarth.Meredith.Public;

namespace WhyNotEarth.Meredith.App.Results.Api.v0.Public.Page
{
    public class StoryResult
    {
        public int Id { get; }

        public string Blur { get; } = "2px";

        public string Content { get; }

        public string CtaText { get; }

        public string? CtaLink { get; }

        public string Image { get; }

        public string? PosterUrl { get; }

        public string Type { get; }

        public StoryResult(int id, string content, string ctaText, string? ctaLink, string image,
            string? posterUrl, Card.CardTypes cardType)
        {
            Id = id;
            Content = content;
            CtaText = ctaText;
            CtaLink = ctaLink;
            Image = image;
            PosterUrl = posterUrl;
            Type = GetCardType(cardType);
        }

        private string GetCardType(Card.CardTypes cardType)
        {
            return cardType switch
            {
                Card.CardTypes.Card => "story-card",
                _ => throw new Exception($"Card type {cardType} not mapped.")
            };
        }
    }
}