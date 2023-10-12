using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileSystemGlobbing.Internal;
using System.Runtime.Intrinsics.X86;
using System;
using System.Text.RegularExpressions;

public static class HtmlHelperExtensions
{

    public static string Shorten(this IHtmlHelper htmlHelper, string designation, int maxLength)
    {
        if (maxLength <= 10)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength));
        }
        if (designation.Length <= maxLength)
        {
            return designation;
        }

        return designation[..(maxLength-5)] + "..." + designation[^4..];
    }

    public static IHtmlContent HighlightPercentages(this IHtmlHelper htmlHelper, string designation)
    {
        var content = new HtmlContentBuilder();
        content.AppendHtml(Akt(Stk(Price(XForY(Percentage(designation))))));
        return content;
    }

    public static string Percentage(string designation)
    {
        string pattern = @"\d{1,2}%"; // Regex pattern for finding percentage values
        string replacement = "<span class=\"text-danger\">$&</span>";
        string result = Regex.Replace(designation, pattern, replacement);
        return result;
    }

    public static string Akt(string designation)
    {
        foreach (var replacement in new string[] { "Akt", "Aktion", "Rabatt" })
        {
            designation = designation.Replace(replacement, $"<span class=\"text-warning\">{replacement}</span>");
        }
        return designation;
    }

    public static string XForY(string designation)
    {
        string pattern = @"\d ?f[ü|o]r ?\d";
        string replacement = "<span class=\"text-success\">$&</span>";
        string result = Regex.Replace(designation, pattern, replacement);
        return result;
    }

    public static string Stk(string designation)
    {
        string pattern = @"(ab )?\d{1,2} ?Stk\.?( nach Wahl)?";
        string replacement = "<span class=\"text-primary\">$&</span>";
        string result = Regex.Replace(designation, pattern, replacement);
        return result;
    }

    public static string Price(string designation)
    {
        string pricePattern = @"\d+\.(\d+|\-{2})";
        string pattern = @$"{pricePattern}( statt {pricePattern})?";
        string replacement = "<span class=\"text-info\">$&</span>";
        string result = Regex.Replace(designation, pattern, replacement);
        return result;
    }
}