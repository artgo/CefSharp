using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AppDirect.WindowsClient.API.VO
{
    [Serializable]
    [XmlRootAttribute("subscription")]
    public class SubscriptionWS
    {
        [XmlElementAttribute("id", Order = 1)]
        public string id { get; set; }

        [XmlElementAttribute("company", IsNullable = true, Order = 2)]
        public CompanyWS company { get; set; }

        [XmlElementAttribute("user", IsNullable = true, Order = 3)]
        public UserWS user { get; set; }

        [XmlElementAttribute("paymentPlanId", Order = 4)]
        public string paymentPlanId { get; set; }

        [XmlElementAttribute("currency", Order = 5)]
        public string currency { get; set; }

        [XmlElementAttribute("status", Order = 6)]
        public CompanyEntitlementStatus? status { get; set; }

        [XmlElementAttribute("discountCodeId", Order = 7)]
        public long? discountCodeId { get; set; }

        [XmlElementAttribute("purchaseOrderNumber", Order = 8)]
        public string purchaseOrderNumber { get; set; }

        [XmlElementAttribute("createdOn", Order = 9)]
        public DateTime? createdOn { get; set; }

        [XmlElementAttribute("lastModified", Order = 10)]
        public DateTime? lastModified { get; set; }

        [XmlElementAttribute("startDateTime", Order = 11)]
        public DateTime? startDateTime { get; set; }

        [XmlElementAttribute("endDateTime", Order = 12)]
        public DateTime? endDateTime { get; set; }

        [XmlElementAttribute("billingEndDateTime", Order = 13)]
        public DateTime? billingEndDateTime { get; set; }

        [XmlElementAttribute("nextInvoiceDateTime", Order = 14)]
        public DateTime? nextInvoiceDateTime { get; set; }

        [XmlElementAttribute("totalPrice", Order = 15)]
        public decimal? totalPrice { get; set; }

        [XmlArrayItem("item", IsNullable = false)]
        [XmlArray("items", IsNullable = false, Order = 16)]
        public List<OrderItemWS> items = new List<OrderItemWS>();

        [XmlArrayItem("parameter", IsNullable = false)]
        [XmlArray("parameters", IsNullable = false, Order = 17)]
        public List<ParameterWS> parameters = new List<ParameterWS>();
    }
}