
using System.Xml.Serialization;

namespace PNBResponse
{
    [XmlRoot(ElementName = "GetAccountBalanceResponse", Namespace = "http://tempuri.org/")]
    public class AccountBalanceResponse
    {
        [XmlElement(ElementName = "AccountBalance", Namespace = "http://PNB_Inward_Remittance.AccountBalance")]
        public required AccountBalance AccountBalance { get; set; }
    }

    public class AccountBalance
    {
        [XmlAttribute(AttributeName = "AccountBalance_ResponseCode")]
        public required string ResponseCode { get; set; }

        [XmlAttribute(AttributeName = "AccountBalance_ResponseMessage")]
        public required string ResponseMessage { get; set; }
    }

    public class Deserialize
    {
        public static AccountBalanceResponse GetBalance_Response(string xmlString)
        {
            var serializer = new XmlSerializer(typeof(AccountBalanceResponse));
            using var reader = new StringReader(xmlString);
            return (AccountBalanceResponse)serializer?.Deserialize(reader)!;
        }
    }

    public class Demo
    {
        public static void Main()
        {
            string xmlResponse =
                $"<GetAccountBalanceResponse xmlns:bpws=\"http://docs.oasis-open.org/wsbpel/2.0/process/executable\" xmlns:mime=\"http://schemas.xmlsoap.org/wsdl/mime/\" xmlns:ns2=\"http://soa.oracle.com/commonservice/EventHandlerMessage\" xmlns:plnk=\"http://docs.oasis-open.org/wsbpel/2.0/plnktype\" xmlns:soapenc=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:wsdl=\"http://schemas.xmlsoap.org/wsdl/\" xmlns:s2=\"http://PNB_Inward_Remittance.AccountBalance\" xmlns:s3=\"http://PNB_Inward_Remittance.RemittanceStatusReceive\" xmlns:soap12=\"http://schemas.xmlsoap.org/wsdl/soap12/\" xmlns:tm=\"http://microsoft.com/wsdl/mime/textMatching/\" xmlns:http=\"http://schemas.xmlsoap.org/wsdl/http/\" xmlns:soap=\"http://schemas.xmlsoap.org/wsdl/soap/\" xmlns:tns=\"http://tempuri.org/\" xmlns=\"http://tempuri.org/\">" +
                    $"<s2:AccountBalance AccountBalance_ResponseCode=\"006\" AccountBalance_ResponseMessage=\"No Data Returned from PNB Core System.\"/>" +
                $"</GetAccountBalanceResponse>";

            var response = Deserialize.GetBalance_Response(xmlResponse);

            Console.WriteLine(response.AccountBalance.ResponseCode);
            Console.WriteLine(response.AccountBalance.ResponseMessage);
        }
    }
}
