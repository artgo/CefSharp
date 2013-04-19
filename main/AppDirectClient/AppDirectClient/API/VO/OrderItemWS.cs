using System;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    public class OrderItemWS
    {
        [XmlElementAttribute("paymentPlanId")]
        public long paymentPlanId { get; set; }

        [XmlElement("unit")]
        public PricingUnit unit { get; set; }

        [XmlElementAttribute("quantity")]
        public decimal quantity { get; set; }
    }
}