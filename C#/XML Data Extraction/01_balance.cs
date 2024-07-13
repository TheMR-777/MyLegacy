
using System.Xml;

namespace MySpace
{
	class Parser
	{
		public static (string code, string msg) ParseAccountBalanceResponse(string xml)
		{
			var doc = new XmlDocument();
			doc.LoadXml(xml);

			var nsManager = new XmlNamespaceManager(doc.NameTable);
			nsManager.AddNamespace("s2", "http://PNB_Inward_Remittance.AccountBalance");

			var accountBalanceNode = doc.SelectSingleNode("//s2:AccountBalance", nsManager);

			if (accountBalanceNode != null)
			{
				var responseCode = accountBalanceNode.Attributes["AccountBalance_ResponseCode"].Value;
				var responseMessage = accountBalanceNode.Attributes["AccountBalance_ResponseMessage"].Value;

				return (responseCode, responseMessage);
			}

			throw new InvalidOperationException("AccountBalance node not found in the XML.");
		}
	}

	class Demo
	{
		private static readonly string response =
			"<GetAccountBalanceResponse xmlns:bpws=\"http://docs.oasis-open.org/wsbpel/2.0/process/executable\" xmlns:mime=\"http://schemas.xmlsoap.org/wsdl/mime/\" xmlns:ns2=\"http://soa.oracle.com/commonservice/EventHandlerMessage\" xmlns:plnk=\"http://docs.oasis-open.org/wsbpel/2.0/plnktype\" xmlns:soapenc=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:wsdl=\"http://schemas.xmlsoap.org/wsdl/\" xmlns:s2=\"http://PNB_Inward_Remittance.AccountBalance\" xmlns:s3=\"http://PNB_Inward_Remittance.RemittanceStatusReceive\" xmlns:soap12=\"http://schemas.xmlsoap.org/wsdl/soap12/\" xmlns:tm=\"http://microsoft.com/wsdl/mime/textMatching/\" xmlns:http=\"http://schemas.xmlsoap.org/wsdl/http/\" xmlns:soap=\"http://schemas.xmlsoap.org/wsdl/soap/\" xmlns:tns=\"http://tempuri.org/\" xmlns=\"http://tempuri.org/\">" +
				"<s2:AccountBalance AccountBalance_ResponseCode=\"006\" AccountBalance_ResponseMessage=\"No Data Returned from PNB Core System.\"/>" +
			"</GetAccountBalanceResponse>";

		public static void Main()
		{
			try
			{
				var (responseCode, responseMessage) = Parser.ParseAccountBalanceResponse(response);
				Console.WriteLine($"Response Code    : {responseCode}");
				Console.WriteLine($"Response Message : {responseMessage}");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}
	}
}
