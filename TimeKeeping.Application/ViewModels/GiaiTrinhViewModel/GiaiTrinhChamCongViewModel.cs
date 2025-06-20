using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TimeKeeping.Application.ViewModels.GiaiTrinhViewModel
{
    public class GiaiTrinhChamCongViewModel
    {
        [JsonPropertyName("viphamid")]
        public string? viphamid { get; set; }

        [JsonPropertyName("giaitrinhid")]
        public int? giaitrinhid { get; set; }

        [JsonPropertyName("maky")]
        public int? maky { get; set; }
    }
}
