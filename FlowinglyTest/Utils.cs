
namespace FlowinglyTest;

public class Utils
{
    public static readonly double salesTax = 15;
    public static string GetXmlValueFromEmail(string actualStr, string strFirst, string strLast)
    {
        var startingIndex = actualStr.IndexOf(strFirst) + strFirst.Length;
        var endingIndex = actualStr.IndexOf(strLast) - startingIndex;

         return actualStr.Substring(startingIndex, endingIndex).Trim();
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
                var xmlValue = GetXmlValueFromEmail(emailContent, startingXmlTag, endingXmlTag);
                if (!xmlValue.StartsWith('<'))
                {
                    XmlText xmlText = xmlDocument.CreateTextNode(xmlValue);
                    childElement.AppendChild(xmlText);
                    rootElement.FirstChild?.AppendChild(childElement);
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

    public static string CalculateTotalExclTax(string? originalTotal)
    {
        var remainingTax = Convert.ToDouble(100) / (100 + salesTax);
        var salesTaxAmount = Math.Round(Convert.ToDouble(originalTotal) - Convert.ToDouble(originalTotal) * remainingTax, 2);
        return salesTaxAmount.ToString("N");
    }
}
