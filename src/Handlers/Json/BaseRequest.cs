using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LinkvertiseBypass.Handlers.Json
{
    public class BaseRequest
    {
        public required RootMain root { get; set; }

        public class LinkIdentification
        {
            [JsonPropertyName("userIdAndUrl")]
            public required UserIdAndUrl UserIdAndUrl { get; set; }
        }

        public class RootMain
        {
            [JsonPropertyName("operationName")]
            public required string OperationName { get; set; }

            [JsonPropertyName("variables")]
            public required Variables Variables { get; set; }

            [JsonPropertyName("query")]
            public required string Query { get; set; }
        }

        public class UserIdAndUrl
        {
            [JsonPropertyName("user_id")]
            public required string UserId { get; set; }

            [JsonPropertyName("url")]
            public required string Url { get; set; }
        }

        public class Variables
        {
            [JsonPropertyName("linkIdentificationInput")]
            public required LinkIdentification LinkIdentification { get; set; }

            [JsonPropertyName("origin")]
            public required string Origin { get; set; }

            [JsonPropertyName("additional_data")]
            public required AdditionalData AdditionalData { get; set; }

            [JsonPropertyName("completeDetailPageContentInput")]
            public AdCompletedToken.CompleteDetailPageContent? CompleteDetailPageContent { get; set; }

            [JsonPropertyName("token")]
            public string? Token { get; set; }
        }

        public class AdditionalData
        {
            [JsonPropertyName("taboola")]
            public required Taboola Taboola { get; set; }
        }

        public class Taboola
        {
            [JsonPropertyName("user_id")]
            public required string UserId { get; set; }

            [JsonPropertyName("url")]
            public required string Url { get; set; }
        }

        public static string GetOperation<T>()
        {
            if (typeof(T) == typeof(AdAccessToken))
            {
                return "getDetailPageContent";
            }

            if (typeof(T) == typeof(AdCompletedToken))
            {
                return "completeDetailPageContent";
            }

            if (typeof(T) == typeof(FinalResponse))
            {
                return "getDetailPageTarget";
            }

            return "";
        }

        public static string GetMutationQuery<T>()
        {
            if (typeof(T) == typeof(AdAccessToken))
            {
                return "mutation getDetailPageContent($linkIdentificationInput: PublicLinkIdentificationInput!, $origin: String, $additional_data: CustomAdOfferProviderAdditionalData!) {  getDetailPageContent(    linkIdentificationInput: $linkIdentificationInput    origin: $origin    additional_data: $additional_data  ) {    access_token    premium_subscription_active    link {      video_url      short_link_title      recently_edited      short_link_title      description      url      seo_faqs {        body        title        __typename      }      target_host      last_edit_at      link_images {        url        __typename      }      title      thumbnail_url      view_count      is_trending      recently_edited      seo_faqs {        title        body        __typename      }      percentage_rating      is_premium_only_link      publisher {        id        name        subscriber_count        __typename      }      __typename    }    linkCustomAdOffers {      title      call_to_action      description      countdown      completion_token      provider      provider_additional_payload {        taboola {          available_event_url          visible_event_url          __typename        }        __typename      }      media {        type        ... on UrlMediaResource {          content_type          resource_url          __typename        }        __typename      }      clickout_action {        type        ... on CustomAdOfferClickoutUrlAction {          type          clickout_url          __typename        }        __typename      }      __typename    }    link_recommendations {      short_link_title      target_host      id      url      publisher {        id        name        __typename      }      last_edit_at      link_images {        url        __typename      }      title      thumbnail_url      view_count      is_trending      recently_edited      percentage_rating      publisher {        name        __typename      }      __typename    }    target_access_information {      remaining_accesses      daily_access_limit      __typename    }    __typename  }}";
            }

            if (typeof(T) == typeof(AdCompletedToken))
            {
                return "mutation completeDetailPageContent($linkIdentificationInput: PublicLinkIdentificationInput!, $completeDetailPageContentInput: CompleteDetailPageContentInput!) {  completeDetailPageContent(    linkIdentificationInput: $linkIdentificationInput    completeDetailPageContentInput: $completeDetailPageContentInput  ) {    CUSTOM_AD_STEP    TARGET    __typename  }}";
            }

            if (typeof(T) == typeof(FinalResponse))
            {
                return "mutation getDetailPageTarget($linkIdentificationInput: PublicLinkIdentificationInput!, $token: String!) {  getDetailPageTarget(    linkIdentificationInput: $linkIdentificationInput    token: $token  ) {    type    url    paste    short_link_title    __typename  }}";
            }

            return "";
        }
    }
}
