using HtmlAgilityPack;
using System.Globalization;

namespace EstevesPriceAlert.Functions;

public static class PriceUtils
{
    private static readonly HttpClient Http = new(new HttpClientHandler
    {
        AutomaticDecompression = System.Net.DecompressionMethods.GZip | System.Net.DecompressionMethods.Deflate
    })
    {
        Timeout = TimeSpan.FromSeconds(15)
    };

    private static readonly Dictionary<string, string[]> XPathsByHost = new(StringComparer.OrdinalIgnoreCase)
    {
        ["amazon.com"] = new[] { "//*[@id='corePrice_feature_div']//*[contains(@class,'a-offscreen')]", "//*[starts-with(@id,'priceblock_')]" },
        ["amazon.com.br"] = new[] { "//*[@id='corePrice_feature_div']//*[contains(@class,'a-offscreen')]", "//*[starts-with(@id,'priceblock_')]" },

        ["www.magazineluiza.com.br"] = new[] { "//*[contains(@class,'price-template__text')]", "//*[@data-testid='price-value']" },

        ["www.mercadolivre.com.br"] = new[] { "//*[contains(@class,'andes-money-amount__fraction')]", "//*[@itemprop='price']" },
        ["mercadolivre.com.br"] = new[] { "//*[contains(@class,'andes-money-amount__fraction')]", "//*[@itemprop='price']" },

        ["www.submarino.com.br"] = new[] { "//*[@id='product-price']", "//*[contains(@class,'price-sales')]" },
        ["submarino.com.br"] = new[] { "//*[@id='product-price']", "//*[contains(@class,'price-sales')]" },

        ["www.shoptime.com.br"] = new[] { "//*[contains(@class,'value') and contains(., 'R$')]" },

        ["www.americanas.com.br"] = new[] { "//*[contains(@id,'product-price') or contains(@class,'price-tag')]" },

        ["www.bestbuy.com"] = new[] { "//div[contains(@class,'priceView-hero-price')]", "//*[@itemprop='price']" },

        ["www.udemy.com"] = new[] { "//div[contains(@class,'price-text--price-part--')]" },

        ["www.walmart.com.br"] = new[] { "//*[contains(@class,'price-characteristic')]", "//*[@itemprop='price']" }
    };


    public static async Task<decimal?> GetBestPriceAsync(IEnumerable<string> urls, CancellationToken ct)
    {
        decimal? best = null;

        foreach (var url in urls.Where(u => !string.IsNullOrWhiteSpace(u)).Distinct())
        {
            var p = await TryGetPriceAsync(url, ct);
            if (p.HasValue)
                best = best.HasValue ? Math.Min(best.Value, p.Value) : p.Value;
        }

        return best;
    }

    private static async Task<decimal?> TryGetPriceAsync(string url, CancellationToken ct)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, url);
        req.Headers.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        using var resp = await Http.SendAsync(req, ct);
        resp.EnsureSuccessStatusCode();

        var html = await resp.Content.ReadAsStringAsync(ct);

        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        var host = new Uri(url).Host;
        var xpaths = ResolveXPaths(host);

        foreach (var xp in xpaths)
        {
            var node = doc.DocumentNode.SelectSingleNode(xp);
            if (node == null) continue;

            var raw = node.GetAttributeValue("content", null) ?? node.InnerText;
            var parsed = ParsePrice(raw);
            if (parsed.HasValue) return parsed;
        }

        var fb = doc.DocumentNode.SelectSingleNode("//*[contains(text(),'R$') or contains(text(),'$')]");
        return ParsePrice(fb?.InnerText);
    }

    private static string[] ResolveXPaths(string host)
    {
        foreach (var kv in XPathsByHost)
            if (host.EndsWith(kv.Key, StringComparison.OrdinalIgnoreCase))
                return kv.Value;

        return new[]
        {
            "//*[@itemprop='price']",
            "//*[contains(@class,'price') or contains(@id,'price')]"
        };
    }

    private static decimal? ParsePrice(string? raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return null;

        var s = raw.Replace("\u00A0", " ")
                   .Replace("R$", "", StringComparison.OrdinalIgnoreCase)
                   .Replace("$", "")
                   .Trim();

        if (decimal.TryParse(s, NumberStyles.Any, new CultureInfo("pt-BR"), out var br)) return br;
        if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out var en)) return en;

        var filtered = System.Text.RegularExpressions.Regex.Replace(s, @"[^\d,\.]", "");
        if (decimal.TryParse(filtered, NumberStyles.Any, new CultureInfo("pt-BR"), out br)) return br;
        if (decimal.TryParse(filtered, NumberStyles.Any, CultureInfo.InvariantCulture, out en)) return en;

        return null;
    }
}