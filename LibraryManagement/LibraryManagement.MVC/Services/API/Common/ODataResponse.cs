using System.Text.Json.Serialization;

namespace LibraryManagement.MVC.Services.API.Common
{
    public class ODataResponse<T>
    {
        [JsonPropertyName("@odata.count")]
        public int? Count { get; set; }

        [JsonPropertyName("value")]
        public List<T> Value { get; set; } = new();
    }
}
