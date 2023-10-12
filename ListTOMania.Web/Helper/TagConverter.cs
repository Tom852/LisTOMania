using LisTOMania.Common.Model;

namespace ListTOMania.Web.Helper
{
    public static class TagConverter
    {
        public static List<string> GetStringListForBackend(string tags)
        {
            if (string.IsNullOrEmpty(tags))
            {
                return new();
            }

            return tags.Split().Select(s => s.Trim()).Distinct().ToList();
        }

        public static string GetSingleStringForFrontend(List<string> taglist)
        {
            return taglist != null ? string.Join(' ', taglist) : string.Empty;
        }
    }
}