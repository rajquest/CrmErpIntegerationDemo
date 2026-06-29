namespace API.Models.Salesforce
{
    public class CareProgramEnrolleeProduct
    {
        public string Id { get; set; }
        public string OwnerId { get; set; }
        public bool? IsDeleted { get; set; }
        public string Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? SystemModstamp { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public DateTime? LastReferencedDate { get; set; }

        // Relationship / lookup fields
        public string CareProgramEnrolleeId { get; set; }
        public string CareProgramProductId { get; set; }
        public string CareProgramProviderId { get; set; }

        // Business / domain fields
        public string Status { get; set; }
        public string SourceSystemIdentifier { get; set; }
        public string SourceSystem { get; set; }

        // Custom fields (Salesforce __c suffix)
        public string Clinical_Trial_Program__c { get; set; }
        public string Cordella_ID__c { get; set; }
        public string Cuff_Size__c { get; set; }
        public DateTime? Expiration_Date__c { get; set; }
        public DateTime? First_Home_Use_Date__c { get; set; }
        public DateTime? Five_Year_End_of_Useful_Life_Date__c { get; set; }
        public DateTime? Four_Year_End_of_Useful_Life_Date__c { get; set; }
        public string IMEI__c { get; set; }
        public bool? Is_Visible_on_Patient_Card__c { get; set; }
        public string Lot__c { get; set; }
        public string Patient_Account__c { get; set; }
        public string Product_Code__c { get; set; }
        public string Product_Name__c { get; set; }
        public string Product_Record_Type__c { get; set; }
        public string Serial_Number__c { get; set; }
        public DateTime? Setup_Date__c { get; set; }
        public DateTime? Ship_Date__c { get; set; }
        public string Software_Version__c { get; set; }
        public DateTime? Two_Year_End_of_Useful_Life_Date__c { get; set; }
        public bool? Is_Replacement_Product__c { get; set; }
        public string Inventory_From_Site__c { get; set; }
        public string Parent_Product__c { get; set; }
        public string RMA__c { get; set; }
        public string Reader_Kit_Product__c { get; set; }
        public string Implant_Case_Report__c { get; set; }
        public double? Number_of_RMAs__c { get; set; }
        public string RMA_Case__c { get; set; }
    }
}
