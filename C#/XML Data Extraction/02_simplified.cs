public class ResponseModel
{
    public static string GetBalance(string rawXML)
    {
        try
        {
            var doc = new XmlDocument();
            doc.LoadXml(rawXML);

            var nsManager = new XmlNamespaceManager(doc.NameTable);
            nsManager.AddNamespace("s2", "http://PNB_Inward_Remittance.AccountBalance");

            return doc.CreateNavigator()?
                .SelectSingleNode("//s2:AccountBalance/@AccountBalance_ResponseMessage", nsManager)?
                .Value;
        }
        catch
        {
            return string.Empty;
        }
    }
}
