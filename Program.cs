using HtmlAgilityPack;

string[] coChci = new string[] { "Stav komunikace", "Teplota", "Zdánlivá teplota", "Atmosférický tlak", "Denní srážky", "Umístění", "Směr větru" };

string html = await getHtml("https://www.meteo-pocasi.cz/maps/cz/praha/1695-meteostanice-praha-20/");

List<string> vysledek = pocasi(html, coChci);

foreach (var text in vysledek)
{
    if (text == "-")
    {
        Console.WriteLine();
    }
    else
    {
        Console.Write($"{text} ");
    }
}

List<string> pocasi(string html, string[] hledaneVyrazy)
{
    List<string> output = new List<string>();

    HtmlDocument doc = new HtmlDocument();
    doc.LoadHtml(html);

    var nodes = doc.DocumentNode.SelectNodes("//div[@class='box']");

    if (nodes != null)
    {
        foreach (var node in nodes)
        {
            var headerNode = node.SelectSingleNode(".//div[@class='boxheader']");
            var headerNodeStrong = node.SelectSingleNode(".//div[@class='boxheader']/strong");
            foreach (var vyraz in hledaneVyrazy)
            {
                if (headerNode != null && headerNode.InnerText == vyraz)
                {
                    var statusNode = node.SelectSingleNode(".//div[@class='status_meteo_text']");
                    var LocationNode = node.SelectSingleNode(".//div[@class='cmbtext']");
                    if (statusNode != null)
                    {
                        string text = statusNode.InnerText;
                        output.Add(vyraz);
                        output.Add(text);
                        output.Add("-");
                    }
                    else if (LocationNode != null)
                    {
                        string text = LocationNode.InnerText;
                        output.Add(vyraz);
                        output.Add(text);
                        output.Add("-");
                    }
                }
                if (headerNodeStrong != null && headerNodeStrong.InnerText == vyraz)
                {
                    var hodnota = node.SelectSingleNode(".//div[@class='svalue']");
                    var jednotky = node.SelectSingleNode(".//div[@class='smark']");
                    if (jednotky != null)
                    {
                        string hodnotaText = hodnota.InnerText;
                        string jednotyText = jednotky.InnerText;
                        output.Add(vyraz);
                        output.Add(hodnotaText);
                        output.Add(jednotyText);
                        output.Add("-");
                    }
                    else if (hodnota != null)
                    {
                        string hodnotaText = hodnota.InnerText;
                        output.Add(vyraz);
                        output.Add(hodnotaText);
                        output.Add("-");
                    }
                }
            }
        }
    }
    return output;
}

async Task<string> getHtml(string link)
{
    using (HttpClient client = new HttpClient())
    {
        var response = await client.GetAsync(link);
        var content = await response.Content.ReadAsStringAsync();
        return content;
    }
}