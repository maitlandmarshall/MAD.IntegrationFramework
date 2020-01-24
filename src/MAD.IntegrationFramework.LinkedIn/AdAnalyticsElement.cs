using System.Collections.Generic;

namespace MAD.IntegrationFramework.LinkedIn
{
    public class AdAnalyticsElement
    {
        public long ActionClicks { get; set; }
        public long AdUnitClicks { get; set; }
        public long CardClicks { get; set; }
        public long CardImpressions { get; set; }

        public long Clicks { get; set; }
        public long CommentLikes { get; set; }
        public long Comments { get; set; }

        public long CompanyPageClicks { get; set; }

        public decimal ConversionValueInLocalCurrency { get; set; }
        public decimal CostInLocalCurrency { get; set; }

        public decimal CostInUsd { get; set; }

        public LinkedInApiDateRange DateRange { get; set; }

        public long ExternalWebsiteConversions { get; set; }
        public long ExternalWebsitePostClickConversions { get; set; }
        public long ExternalWebsitePostViewConversions { get; set; }

        public long Follows { get; set; }
        public long FullScreenPlays { get; set; }
        public long Impressions { get; set; }
        public long LandingPageClicks { get; set; }
        public long LeadGenerationMailContactInfoShares { get; set; }
        public long LeadGenerationMailInterestedClicks { get; set; }

        public long Likes { get; set; }
        public long OneClickLeadFormOpens { get; set; }
        public long OneClickLeads { get; set; }

        public long Opens { get; set; }
        public long OtherEngagements { get; set; }

        public List<string> PivotValues { get; set; }

        public long Reactions { get; set; }
        public long Sends { get; set; }
        public long Shares { get; set; }
        public long TextUrlClicks { get; set; }
        public long TotalEngagements { get; set; }

        public long VideoCompletions { get; set; }
        public long VideoFirstQuartileCompletions { get; set; }
        public long VideoMidpointCompletions { get; set; }

        public long VideoViews { get; set; }
        public long ViralCardClicks { get; set; }

        public long ViralCardImpressions { get; set; }
        public long ViralClicks { get; set; }
        public long ViralCommentLikes { get; set; }
        public long ViralComments { get; set; }
        public long ViralCompanyPageClicks { get; set; }
        public long ViralExternalWebsiteConversions { get; set; }
        public long ViralExternalWebsitePostClickConversions { get; set; }
        public long ViralExternalWebsitePostViewConversions { get; set; }

        public long ViralFollows { get; set; }
        public long ViralFullScreenPlays { get; set; }
        public long ViralImpressions { get; set; }

        public long ViralLandingPageClicks { get; set; }
        public long ViralLikes { get; set; }
        public long ViralOneClickLeadFormOpens { get; set; }

        public long ViralOneClickLeads { get; set; }

        public long ViralOtherEngagements { get; set; }

        public long ViralReactions { get; set; }
        public long ViralShares { get; set; }
        public long ViralTotalEngagements { get; set; }

        public long ViralVideoCompletions { get; set; }
        public long ViralVideoFirstQuartileCompletions { get; set; }
        public long ViralVideoMidpointCompletions { get; set; }
        public long ViralVideoStarts { get; set; }
        public long ViralVideoThirdQuartileCompletions { get; set; }
        public long ViralVideoViews { get; set; }
    }
}
