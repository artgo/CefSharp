// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.6.0.0
//    <NameSpace>AppDirect.WindowsClient.API.VO</NameSpace><Collection>Array</Collection><codeType>CSharp</codeType><EnableDataBinding>False</EnableDataBinding><EnableLazyLoading>False</EnableLazyLoading><TrackingChangesEnable>False</TrackingChangesEnable><GenTrackingClasses>False</GenTrackingClasses><HidePrivateFieldInIDE>False</HidePrivateFieldInIDE><EnableSummaryComment>False</EnableSummaryComment><VirtualProp>False</VirtualProp><PascalCase>True</PascalCase><BaseClassName>EntityBase</BaseClassName><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>False</UseBaseClass><GenBaseClass>False</GenBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net35</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>False</GenerateXMLAttributes><OrderXMLAttrib>False</OrderXMLAttrib><EnableEncoding>False</EnableEncoding><AutomaticProperties>True</AutomaticProperties><GenerateShouldSerialize>False</GenerateShouldSerialize><DisableDebug>False</DisableDebug><PropNameSpecified>Default</PropNameSpecified><Encoder>UTF8</Encoder><CustomUsings></CustomUsings><ExcludeIncludedTypes>False</ExcludeIncludedTypes><InitializeFields>All</InitializeFields><GenerateAllTypes>True</GenerateAllTypes>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace AppDirect.WindowsClient.API.VO
{
    using System;
    using System.Diagnostics;
    using System.Xml.Serialization;
    using System.Collections;
    using System.Xml.Schema;
    using System.ComponentModel;

    public partial class Product
    {
        [XmlElementAttribute("id")]
        public string Id { get; set; }
        [XmlElementAttribute("name")]
        public string Name { get; set; }
        [XmlElementAttribute("type")]
        public string Type { get; set; }
        [XmlElementAttribute("provider")]
        public ProductProvider Provider { get; set; }
        [XmlElementAttribute("listing")]
        public ProductListing Listing { get; set; }
        [XmlElementAttribute("overview")]
        public ProductOverview Overview { get; set; }
        [XmlElementAttribute("lastModified")]
        public long? LastModified { get; set; }
        [XmlElementAttribute("pricing")]
        public ProductPricing Pricing { get; set; }

        [XmlArrayItemAttribute("feature", IsNullable = false)]
        [XmlElementAttribute("features")]

        public ProductFeaturesFeature[] Features { get; set; }
        [XmlArrayItemAttribute("tag", IsNullable = false)]
        [XmlElementAttribute("tags")]
        public ProductTag[] Tags { get; set; }
        [XmlElementAttribute("href")]
        public string Href { get; set; }

        public Product()
        {
            this.Pricing = new ProductPricing();
            this.Overview = new ProductOverview();
            this.Listing = new ProductListing();
            this.Provider = new ProductProvider();
        }
    }

    public partial class ProductProvider
    {
        [XmlElementAttribute("uuid")]
        public string Uuid { get; set; }
        [XmlElementAttribute("name")]
        public string Name { get; set; }
    }

    public partial class ProductListing
    {
        [XmlElementAttribute("blurb")]
        public string Blurb { get; set; }
        [XmlElementAttribute("overview")]
        public string Overview { get; set; }
        [XmlElementAttribute("reviewCount")]
        public long? ReviewCount { get; set; }
    }

    public partial class ProductOverview
    {

        private ProductOverviewBenefit[] benefitsField;

        [XmlElementAttribute("splashTitle")]
        public string SplashTitle { get; set; }
        [XmlElementAttribute("splashDescription")]
        public string SplashDescription { get; set; }
        [XmlElementAttribute("imageUrl")]
        public string ImageUrl { get; set; }
        [XmlArrayItemAttribute("benefit", IsNullable = false)]
        [XmlElementAttribute("benefits")]
        public ProductOverviewBenefit[] Benefits { get; set; }
    }

    public partial class ProductOverviewBenefit
    {

        [XmlElementAttribute("description")]
        public string Description { get; set; }
        [XmlElementAttribute("title")]
        public string Title { get; set; }
    }

    public partial class ProductPricing
    {
        [XmlArrayItemAttribute("edition", IsNullable = false)]
        [XmlElementAttribute("editions")]
        public ProductPricingEdition[] Editions { get; set; }
    }

    public partial class ProductPricingEdition
    {
        [XmlElementAttribute("id")]
        public string Id { get; set; }
        [XmlElementAttribute("name")]
        public string Name { get; set; }
        [XmlElementAttribute("primary")]
        public string Primary { get; set; }
        [XmlElementAttribute("trial")]
        public ProductPricingEditionTrial Trial { get; set; }
        [XmlElementAttribute("expiredTrialGracePeriod")]
        public string ExpiredTrialGracePeriod { get; set; }

        [XmlArrayItemAttribute("plan", IsNullable = false)]
        [XmlElementAttribute("plans")]
        public ProductPricingEditionPlansPlan[] Plans { get; set; }

        [XmlArrayItemAttribute("item", IsNullable = false)]
        [XmlElementAttribute("items")]
        public ProductPricingEditionItemsItem[] Items { get; set; }

        public ProductPricingEdition()
        {
            this.Trial = new ProductPricingEditionTrial();
        }
    }

    public partial class ProductPricingEditionTrial
    {
        [XmlElementAttribute("length")]
        public long? Length { get; set; }
        [XmlElementAttribute("unit")]
        public string Unit { get; set; }
    }

    public partial class ProductPricingEditionPlans
    {
        [XmlElementAttribute("plan")]
        public ProductPricingEditionPlansPlan Plan { get; set; }

        public ProductPricingEditionPlans()
        {
            this.Plan = new ProductPricingEditionPlansPlan();
        }
    }

    public partial class ProductPricingEditionPlansPlan
    {
        [XmlElementAttribute("id")]
        public string Id { get; set; }
        [XmlElementAttribute("frequency")]
        public string Frequency { get; set; }
        [XmlElementAttribute("contract")]
        public ProductPricingEditionPlansPlanContract Contract { get; set; }
        [XmlElementAttribute("allowCustomUsage")]
        public string AllowCustomUsage { get; set; }
        [XmlElementAttribute("keepBillDateOnUsageChange")]
        public string KeepBillDateOnUsageChange { get; set; }
        [XmlElementAttribute("separatePrepaid")]
        public string SeparatePrepaid { get; set; }
        [XmlElementAttribute("isPrimaryPrice")]
        public string IsPrimaryPrice { get; set; }

        [XmlArrayItemAttribute("cost", IsNullable = false)]
        [XmlElementAttribute("costs")]
        public ProductPricingEditionPlansPlanCost[] Costs { get; set; }

        [XmlElementAttribute("discountDetails")]
        public ProductPricingEditionPlansPlanDiscountDetails DiscountDetails { get; set; }

        public ProductPricingEditionPlansPlan()
        {
            this.DiscountDetails = new ProductPricingEditionPlansPlanDiscountDetails();
            this.Contract = new ProductPricingEditionPlansPlanContract();
        }
    }

    public partial class ProductPricingEditionPlansPlanContract
    {
        [XmlElementAttribute("blockContractDowngrades")]
        public string BlockContractDowngrades { get; set; }
        [XmlElementAttribute("blockContractUpgrades")]
        public string BlockContractUpgrades { get; set; }
        [XmlElementAttribute("blockSwitchToShorterContract")]
        public string BlockSwitchToShorterContract { get; set; }
    }

    public partial class ProductPricingEditionPlansPlanCost
    {
        [XmlElementAttribute("unit")]
        public string Unit { get; set; }
        [XmlElementAttribute("minUnits")]
        public decimal? MinUnits { get; set; }
        [XmlElementAttribute("meteredUsage")]
        public bool? MeteredUsage { get; set; }
        [XmlElementAttribute("pricePerIncrement")]
        public string PricePerIncrement { get; set; }
        [XmlElementAttribute("blockContractDecrease")]
        public string BlockContractDecrease { get; set; }
        [XmlElementAttribute("blockContractIncrease")]
        public string BlockContractIncrease { get; set; }
        [XmlElementAttribute("blockOriginalContractDecrease")]
        public string BlockOriginalContractDecrease { get; set; }

        [XmlArrayItemAttribute("amount", IsNullable = false)]
        [XmlElementAttribute("amounts")]
        public ProductPricingEditionPlansPlanCostAmount[] Amounts { get; set; }
    }

    public partial class ProductPricingEditionPlansPlanCostAmount
    {
        [XmlElementAttribute("currency")]
        public string Currency { get; set; }
        [XmlElementAttribute("value")]
        public decimal? Value { get; set; }
    }

    public partial class ProductPricingEditionPlansPlanDiscountDetails
    {
        [XmlElementAttribute("description")]
        public string Description { get; set; }
        [XmlElementAttribute("percentage")]
        public decimal? Percentage { get; set; }
    }

    public partial class ProductPricingEditionItemsItem
    {
        [XmlElementAttribute("unit")]
        public string Unit { get; set; }
        [XmlElementAttribute("amount")]
        public decimal? Amount { get; set; }
        [XmlIgnoreAttribute()]
        [XmlElementAttribute("amountSpecified")]
        public Boolean AmountSpecified { get; set; }
        [XmlElementAttribute("unlimited")]
        public string Unlimited { get; set; }
    }

    public partial class ProductFeaturesFeature
    {
        [XmlElementAttribute("header")]
        public string Header { get; set; }
        [XmlElementAttribute("description")]
        public string Description { get; set; }
    }

    public partial class ProductTag
    {
        [XmlElementAttribute("type")]
        public string Type { get; set; }
        [XmlElementAttribute("id")]
        public string Id { get; set; }
        [XmlElementAttribute("name")]
        public string Name { get; set; }
        [XmlElementAttribute("showBadge")]
        public string ShowBadge { get; set; }

        [XmlArrayItemAttribute("tag", IsNullable = false)]
        [XmlElementAttribute("children")]
        public ProductTagChildrenTag[] Children { get; set; }
    }

    public partial class ProductTagChildrenTag
    {
        [XmlElementAttribute("type")]
        public string Type { get; set; }
        [XmlElementAttribute("id")]
        public string Id { get; set; }
        [XmlElementAttribute("name")]
        public string Name { get; set; }
        [XmlElementAttribute("showBadge")]
        public string ShowBadge { get; set; }
    }
}
