using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeKeeping.Application.Helpers
{
    public static class KyParserHelper
    {
        public static bool TryParseKy(string ky, out int thang, out int nam)
        {
            thang = 0;
            nam = 0;

            if (string.IsNullOrWhiteSpace(ky))
                return false;

            // Normalize Unicode để loại dấu tổ hợp (VD: á -> á)
            string normalized = ky.Normalize(NormalizationForm.FormD);
            var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
            string cleanedKy = new string(chars.ToArray()).Normalize(NormalizationForm.FormC);

            // Bỏ từ "Thang" (không dấu)
            cleanedKy = cleanedKy.Replace("Thang", "", StringComparison.OrdinalIgnoreCase).Trim();

            var parts = cleanedKy.Split('/');
            if (parts.Length != 2) return false;

            return int.TryParse(parts[0].Trim(), out thang) &&
                   int.TryParse(parts[1].Trim(), out nam);
        }
    }
}
