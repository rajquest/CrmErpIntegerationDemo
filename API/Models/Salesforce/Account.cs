namespace API.Models.Salesforce
{
    public class Account
    {
        public string Id { get; set; }
        public bool? IsDeleted { get; set; }
        public string MasterRecordId { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Salutation { get; set; }
        public string MiddleName { get; set; }
        public string Suffix { get; set; }
        public string Type { get; set; }
        public string RecordTypeId { get; set; }
        public string ParentId { get; set; }

        // 🔹 Billing Info
        public string BillingStreet { get; set; }
        public string BillingCity { get; set; }
        public string BillingState { get; set; }
        public string BillingPostalCode { get; set; }
        public string BillingCountry { get; set; }
        public string BillingStateCode { get; set; }
        public string BillingCountryCode { get; set; }
        public double? BillingLatitude { get; set; }
        public double? BillingLongitude { get; set; }
        public string BillingGeocodeAccuracy { get; set; }
        public string BillingAddress { get; set; }

        // 🔹 Shipping Info
        public string ShippingStreet { get; set; }
        public string ShippingCity { get; set; }
        public string ShippingState { get; set; }
        public string ShippingPostalCode { get; set; }
        public string ShippingCountry { get; set; }
        public string ShippingStateCode { get; set; }
        public string ShippingCountryCode { get; set; }
        public double? ShippingLatitude { get; set; }
        public double? ShippingLongitude { get; set; }
        public string ShippingGeocodeAccuracy { get; set; }
        public string ShippingAddress { get; set; }

        // 🔹 Core details
        public string Phone { get; set; }
        public string AccountNumber { get; set; }
        public string Website { get; set; }
        public string PhotoUrl { get; set; }
        public string Industry { get; set; }
        public int? NumberOfEmployees { get; set; }
        public string Description { get; set; }
        public string Site { get; set; }

        // 🔹 Ownership and audit
        public string OwnerId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string CreatedById { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string LastModifiedById { get; set; }
        public DateTime? SystemModstamp { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public DateTime? LastViewedDate { get; set; }
        public DateTime? LastReferencedDate { get; set; }

        // 🔹 Person Account fields
        public bool? IsPersonAccount { get; set; }
        public string PersonContactId { get; set; }
        public string PersonMobilePhone { get; set; }
        public string PersonEmail { get; set; }
        public string PersonTitle { get; set; }
        public string PersonDepartment { get; set; }
        public DateTime? PersonBirthdate { get; set; }
        public DateTime? PersonLastCURequestDate { get; set; }
        public DateTime? PersonLastCUUpdateDate { get; set; }
        public string PersonEmailBouncedReason { get; set; }
        public DateTime? PersonEmailBouncedDate { get; set; }
        public string PersonIndividualId { get; set; }
        public string PersonMaritalStatus { get; set; }
        public string PersonGender { get; set; }
        public DateTime? PersonDeceasedDate { get; set; }
        public double? PersonSequenceInMultipleBirth { get; set; }

        // 🔹 Coordinates and status
        public bool? IsActive { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public DateTime? EndDate { get; set; }

        // 🔹 Source and integration
        public string SourceSystemIdentifier { get; set; }
        public DateTime? SourceSystemModifiedDate { get; set; }
        public bool? IsPartner { get; set; }
        public bool? IsCustomerPortal { get; set; }
        public string ChannelProgramName { get; set; }
        public string ChannelProgramLevelName { get; set; }

        // 🔹 Healthcare / Clinical trial custom fields
        public string Clinical_Specialist__c { get; set; }
        public DateTime? Date_Withdrawn__c { get; set; }
        public DateTime? Date_of_Enrollment__c { get; set; }
        public DateTime? Date_of_Implant_Anticipated__c { get; set; }
        public DateTime? Date_of_Implant__c { get; set; }
        public string Enrollment_Status__c { get; set; }
        public string Patient_First_Name__c { get; set; }
        public string Patient_Last_Name__c { get; set; }
        public string Patient_Middle_Name__c { get; set; }
        public string Patient_Email__c { get; set; }
        public string Patient_Phone__c { get; set; }
        public string Patient_Cordella_ID__c { get; set; }
        public string Site__c { get; set; }
        public string Site_Number__c { get; set; }
        public string Implanting_Physician__c { get; set; }
        public string Region__c { get; set; }
        public string Status__c { get; set; }

        // 🔹 Health Cloud fields (subset)
        public string HealthCloudGA__Age__pc { get; set; }
        public DateTime? HealthCloudGA__BirthDate__pc { get; set; }
        public string HealthCloudGA__Gender__pc { get; set; }
        public string HealthCloudGA__MedicalRecordNumber__pc { get; set; }
        public string HealthCloudGA__IndividualId__pc { get; set; }
        public string HealthCloudGA__SourceSystemId__pc { get; set; }
        public string HealthCloudGA__SourceSystem__pc { get; set; }

        // 🔹 Miscellaneous / derived
        public string AccountSource { get; set; }
        public string SicDesc { get; set; }
        public string OperatingHoursId { get; set; }
        public bool? IsPriorityRecord { get; set; }
    }
}
