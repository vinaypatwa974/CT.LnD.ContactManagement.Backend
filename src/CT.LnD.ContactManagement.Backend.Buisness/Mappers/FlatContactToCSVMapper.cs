using CsvHelper.Configuration;
using CT.LnD.ContactManagement.Backend.Dto.Business;

namespace CT.LnD.ContactManagement.Backend.Business.Mappers
{
    public class FlatContactToCSVMapper : ClassMap<ContactToCSVModel>
    {
        public FlatContactToCSVMapper()
        {
            _ = Map(m => m.FirstName).Name("First Name");
            _ = Map(m => m.LastName).Name("Last Name");
            _ = Map(m => m.Company).Name("Company");
            _ = Map(m => m.Notes).Name("Notes");
            _ = Map(m => m.Tags).Name("Tags");

            _ = Map(m => m.PrimaryPhoneNumber).Name("Primary Phone");
            _ = Map(m => m.PrimaryPhoneCountryCode).Name("Primary Phone Country Code");

            _ = Map(m => m.SecondaryPhoneNumber).Name("Secondary Phone");
            _ = Map(m => m.SecondaryPhoneCountryCode).Name("Secondary Phone Country Code");

            _ = Map(m => m.WorkPhoneNumber).Name("Work Phone");
            _ = Map(m => m.WorkPhoneCountryCode).Name("Work Phone Country Code");

            _ = Map(m => m.PrimaryEmailAddress).Name("Primary Email");
            _ = Map(m => m.SecondaryEmailAddress).Name("Secondary Email");
            _ = Map(m => m.WorkEmailAddress).Name("Work Email");

            _ = Map(m => m.HomeAddressStreet).Name("Home Street");
            _ = Map(m => m.HomeAddressCity).Name("Home City");
            _ = Map(m => m.HomeAddressState).Name("Home State");
            _ = Map(m => m.HomeAddressZipcode).Name("Home Zipcode");
            _ = Map(m => m.HomeAddressCountry).Name("Home Country");

            _ = Map(m => m.WorkAddressStreet).Name("Work Street");
            _ = Map(m => m.WorkAddressCity).Name("Work City");
            _ = Map(m => m.WorkAddressState).Name("Work State");
            _ = Map(m => m.WorkAddressZipcode).Name("Work Zipcode");
            _ = Map(m => m.WorkAddressCountry).Name("Work Country");

            _ = Map(m => m.OtherAddressStreet).Name("Other Street");
            _ = Map(m => m.OtherAddressCity).Name("Other City");
            _ = Map(m => m.OtherAddressState).Name("Other State");
            _ = Map(m => m.OtherAddressZipcode).Name("Other Zipcode");
            _ = Map(m => m.OtherAddressCountry).Name("Other Country");
        }

    }
}
