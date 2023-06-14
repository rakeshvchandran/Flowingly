using System.Xml;
using System.Text.RegularExpressions;

namespace FlowinglyTest;

public class Utils
{
    public static readonly double salesTax = 15;
    public static async Task<string> GetXmlValueFromEmail(string ActualStr, string StrFirst, string StrLast)
    {
        var startingIndex = ActualStr.IndexOf(StrFirst) + StrFirst.Length;
        var endingIndex = ActualStr.IndexOf(StrLast) - startingIndex;

         return ActualStr.Substring(startingIndex, endingIndex).Trim();

        //return ActualStr.Substring(ActualStr.IndexOf(StrFirst) + StrFirst.Length,
        //      (ActualStr.Substring(ActualStr.IndexOf(StrFirst))).IndexOf(StrLast) + StrLast.Length);
    }

    public async static Task<XmlDocument> BuildXml(string emailContent)
    {
        var pattern = "<[^>]*>";

        MatchCollection matches = Regex.Matches(emailContent, pattern);
        XmlDocument xmlDocument = new XmlDocument();

        XmlDeclaration xmlDeclaration = xmlDocument.CreateXmlDeclaration("1.0", "UTF-8", null);
        XmlElement rootElement = xmlDocument.CreateElement("Expenses");
        xmlDocument.AppendChild(rootElement);
        char[] charsToTrim = { '<', '>', '/' };

        foreach (Match match in matches)
        {
            if (!match.Groups[0].Value.Contains('/'))
            {
                var tagName = match.Groups[0].Value.Trim(charsToTrim);
                var startingXmlTag = $"<{tagName}>";
                var endingXmlTag = $"</{tagName}>";
                XmlElement childElement = xmlDocument.CreateElement(tagName);
                var xmlValue = await GetXmlValueFromEmail(emailContent, startingXmlTag, endingXmlTag);
                if (!xmlValue.StartsWith('<'))
                {
                    XmlText xmlText = xmlDocument.CreateTextNode(xmlValue);
                    childElement.AppendChild(xmlText);
                    rootElement.FirstChild.AppendChild(childElement);
                }
                else
                {
                    rootElement.AppendChild(childElement);
                }

                
                xmlDocument.Save(Directory.GetCurrentDirectory() + "//document.xml");
            }

        }
        return xmlDocument;
    }

    public static async Task<string> CalculateTotalExclTax(string? originalTotal)
    {
        var remainingTax = Convert.ToDouble(100) / (100 + salesTax);
        var salesTaxAmount = Math.Round(Convert.ToDouble(originalTotal) - Convert.ToDouble(originalTotal) * remainingTax,2);
        return salesTaxAmount.ToString("N");
    }
}
