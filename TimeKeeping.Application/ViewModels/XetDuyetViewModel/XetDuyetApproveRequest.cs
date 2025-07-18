using System;
using System.Text.Json.Serialization;

namespace TimeKeeping.Application.ViewModels.XetDuyetViewModel
{
    public class XetDuyetApproveRequest
    {
        [JsonPropertyName("viphamid")]
        public string viphamid { get; set; } = string.Empty;
        
        [JsonPropertyName("maky")]
        public int? maky { get; set; }
    }
}
