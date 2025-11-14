using CsvHelper;
using CsvHelper.Configuration;
using CT.LnD.ContactManagement.Backend.Dto.Business;
using CT.LnD.ContactManagement.Backend.Dto.Hosting;
using System.ComponentModel.DataAnnotations;

public class CSVContactMapper : ClassMap<CSVContactModel>
{
    public CSVContactMapper()
    {
        _ = Map(m => m.FirstName).Name("First Name").Validate(field =>
        {
            return !string.IsNullOrWhiteSpace(field.Field);
        });

        _ = Map(m => m.LastName).Name("Last Name").Validate(field =>
        {
            return !string.IsNullOrWhiteSpace(field.Field);
        });

        _ = Map(m => m.Tags).Name("Tags").Validate(field =>
        {
            return !string.IsNullOrWhiteSpace(field.Field);
        });

        _ = Map(m => m.Notes).Name("Notes").Validate(field =>
        {
            return !string.IsNullOrWhiteSpace(field.Field);
        });

        _ = Map(m => m.Company).Name("Company").Validate(field =>
        {
            return !string.IsNullOrWhiteSpace(field.Field);
        });


        _ = Map(m => m.PrimaryContactNumber).Convert(row =>
        {
            string number = row.Row.GetField("Primary Phone Number");
            string code = row.Row.GetField("Primary Phone Country Code");

            PhoneNumberDto phone = new()
            {
                CountryCode = code,
                Number = number,
                TypeId = 1
            };

            ValidationContext validationContext = new(phone);
            List<ValidationResult> results = [];

            bool isValid = Validator.TryValidateObject(phone, validationContext, results, true);

            if (!isValid)
            {
                CsvContext csvContext = row.Row.Context;
                string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Phone number validation failed - {errorMessage}");
            }

            return phone;
        });

        _ = Map(m => m.SecondaryContactNumber).Convert(row =>
        {
            if (row.Row.TryGetField("Secondary Phone Country Code", out string code) &&
                row.Row.TryGetField("Secondary Phone Number", out string number))
            {
                PhoneNumberDto phone = new()
                {
                    CountryCode = code,
                    Number = number,
                    TypeId = 2
                };

                ValidationContext validationContext = new(phone);
                List<ValidationResult> results = [];

                bool isValid = Validator.TryValidateObject(phone, validationContext, results, true);

                if (!isValid)
                {
                    CsvContext csvContext = row.Row.Context;
                    string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                    throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Phone number validation failed - {errorMessage}");
                }

                return phone;
            }

            return null;
        });

        _ = Map(m => m.WorkPhoneNumber).Convert(row =>
        {
            if (row.Row.TryGetField("Work Phone Country Code", out string code) &&
                row.Row.TryGetField("Work Phone Number", out string number))
            {
                PhoneNumberDto phone = new()
                {
                    CountryCode = code,
                    Number = number,
                    TypeId = 3
                };

                ValidationContext validationContext = new(phone);
                List<ValidationResult> results = [];

                bool isValid = Validator.TryValidateObject(phone, validationContext, results, true);

                if (!isValid)
                {
                    CsvContext csvContext = row.Row.Context;
                    string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                    throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Phone number validation failed - {errorMessage}");
                }

                return phone;


            }
            return null;
        });

        _ = Map(m => m.PrimaryEmailAddress).Convert(row =>
        {
            if (row.Row.TryGetField("Primary Email Address", out string email))
            {
                EmailAddressDto emailAddress = new()
                {
                    Email = email,
                    TypeId = 1
                };


                ValidationContext validationContext = new(emailAddress);
                List<ValidationResult> results = [];

                bool isValid = Validator.TryValidateObject(emailAddress, validationContext, results, true);

                if (!isValid)
                {
                    CsvContext csvContext = row.Row.Context;
                    string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                    throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Email Address validation failed - {errorMessage}");
                }

                return emailAddress;


            }
            return null;
        });

        _ = Map(m => m.SecondaryEmailAddress).Convert(row =>
        {
            if (row.Row.TryGetField("Secondary Email Address", out string email))
            {
                EmailAddressDto emailAddress = new()
                {
                    Email = email,
                    TypeId = 2
                };


                ValidationContext validationContext = new(emailAddress);
                List<ValidationResult> results = [];

                bool isValid = Validator.TryValidateObject(emailAddress, validationContext, results, true);

                if (!isValid)
                {
                    CsvContext csvContext = row.Row.Context;
                    string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                    throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Email Address validation failed - {errorMessage}");
                }

                return emailAddress;


            }
            return null;
        });

        _ = Map(m => m.WorkEmailAddress).Convert(row =>
        {
            if (row.Row.TryGetField("Work Email Address", out string email))
            {
                EmailAddressDto emailAddress = new()
                {
                    Email = email,
                    TypeId = 3
                };



                ValidationContext validationContext = new(emailAddress);
                List<ValidationResult> results = [];

                bool isValid = Validator.TryValidateObject(emailAddress, validationContext, results, true);

                if (!isValid)
                {
                    CsvContext csvContext = row.Row.Context;
                    string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                    throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Email Address validation failed - {errorMessage}");
                }

                return emailAddress;



            }


            return null;
        });

        _ = Map(m => m.HomeAddress).Convert(row =>
        {
            if (row.Row.TryGetField("Home Address Street", out string street) &&
                row.Row.TryGetField("Home Address City", out string city) &&
                row.Row.TryGetField("Home Address State", out string state) &&
                row.Row.TryGetField("Home Address Country", out string country) &&
                row.Row.TryGetField("Home Address Zipcode", out string zip))
            {
                PhysicalAddressDto physicalAddress = new()
                {
                    Street = street,
                    City = city,
                    State = state,
                    Country = country,
                    Zipcode = zip,
                    TypeId = 1
                };

                ValidationContext validationContext = new(physicalAddress);
                List<ValidationResult> results = [];

                bool isValid = Validator.TryValidateObject(physicalAddress, validationContext, results, true);

                if (!isValid)
                {
                    CsvContext csvContext = row.Row.Context;
                    string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                    throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Phyical Address validation failed - {errorMessage}");
                }

                return physicalAddress;

            }
            return null;
        });

        _ = Map(m => m.WorkAddress).Convert(row =>
        {
            if (row.Row.TryGetField("Work Address Street", out string street) &&
                row.Row.TryGetField("Work Address City", out string city) &&
                row.Row.TryGetField("Work Address State", out string state) &&
                row.Row.TryGetField("Work Address Country", out string country) &&
                row.Row.TryGetField("Work Address Zipcode", out string zip))
            {
                PhysicalAddressDto physicalAddress = new()
                {
                    Street = street,
                    City = city,
                    State = state,
                    Country = country,
                    Zipcode = zip,
                    TypeId = 2
                };

                ValidationContext validationContext = new(physicalAddress);
                List<ValidationResult> results = [];

                bool isValid = Validator.TryValidateObject(physicalAddress, validationContext, results, true);

                if (!isValid)
                {
                    CsvContext csvContext = row.Row.Context;
                    string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                    throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Phyical Address validation failed - {errorMessage}");
                }

                return physicalAddress;


            }
            return null;
        });

        _ = Map(m => m.OtherAddress).Convert(row =>
        {
            if (row.Row.TryGetField("Other Address Street", out string street) &&
                row.Row.TryGetField("Other Address City", out string city) &&
                row.Row.TryGetField("Other Address State", out string state) &&
                row.Row.TryGetField("Other Address Country", out string country) &&
                row.Row.TryGetField("Other Address Zipcode", out string zip))
            {
                PhysicalAddressDto physicalAddress = new()
                {
                    Street = street,
                    City = city,
                    State = state,
                    Country = country,
                    Zipcode = zip,
                    TypeId = 3
                };

                ValidationContext validationContext = new(physicalAddress);
                List<ValidationResult> results = [];

                bool isValid = Validator.TryValidateObject(physicalAddress, validationContext, results, true);

                if (!isValid)
                {
                    CsvContext csvContext = row.Row.Context;
                    string errorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Unknown error";

                    throw new CsvHelperException(csvContext, $"Row {csvContext.Parser.Row}: Phyical Address validation failed - {errorMessage}");
                }

                return physicalAddress;


            }
            return null;
        });
    }
}
