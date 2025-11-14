namespace CT.LnD.ContactManagement.Backend.DataAccess.Database.Constants
{
    public static class ContactSqlQueries
    {
        public const string GetPaginatedContacts = @"
            SELECT * FROM Contacts 
            WHERE UserId = {0} 
            AND Id NOT IN (SELECT ContactId FROM DeletedContacts) 
            ORDER BY Id OFFSET {1} ROW FETCH NEXT {2} ROWS ONLY";



        public const string GetContactById = @"
            SELECT * FROM Contacts 
            WHERE Id = {0} 
            AND Id NOT IN (SELECT ContactId FROM DeletedContacts)";


        public const string InsertContact = @"
                INSERT INTO Contacts (FirstName, LastName, Company, Notes, Tags, UserId)
                OUTPUT INSERTED.Id
                VALUES (@FirstName, @LastName, @Company, @Notes, @Tags, @UserId);";


        public const string GetEmailsByContactId = "SELECT * FROM EmailAddresses WHERE ContactId = {0}";
        public const string GetPhonesByContactId = "SELECT * FROM PhoneNumbers WHERE ContactId = {0}";
        public const string GetAddressesByContactId = "SELECT * FROM PhysicalAddresses WHERE ContactId = {0}";
        public const string GetPhoneNumberByNumber = "SELECT * FROM PhoneNumbers WHERE Number = {0}";
        public const string GetContactByIdNoFilter = "SELECT * FROM Contacts WHERE Id = {0}";
        public const string InsertPhoneNumber = "INSERT INTO PhoneNumbers (ContactId, CountryCode, Number, TypeId) VALUES ({0}, {1}, {2}, {3})";
        public const string InsertAddress = "INSERT INTO PhysicalAddresses (ContactId, TypeId, Street, Zipcode, City, Country, State) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})";
        public const string InsertEmail = "INSERT INTO EmailAddresses (ContactId, TypeId, Email) VALUES ({0}, {1}, {2})";
        public const string UpdateContact = "UPDATE Contacts SET FirstName = {0}, LastName = {1}, Company = {2}, Notes = {3}, UserId = {4} WHERE Id = {5}";
        public const string UpsertPhone = @"
        IF EXISTS (
            SELECT 1 FROM PhoneNumbers WHERE ContactId = {2} AND TypeId = {3}
        )
        BEGIN
            UPDATE PhoneNumbers
            SET CountryCode = {0}, Number = {1}
            WHERE ContactId = {2} AND TypeId = {3}
        END
        ELSE
        BEGIN
            INSERT INTO PhoneNumbers (CountryCode, Number, ContactId, TypeId)
            VALUES ({0},{1},{2},{3})
        END";


        public const string UpsertAddress = @"
        IF EXISTS (
            SELECT 1 FROM PhysicalAddresses WHERE ContactId = {5} AND TypeId = {6}
        )
        BEGIN
            UPDATE PhysicalAddresses
            SET Street = {0}, Zipcode = {1}, City = {2}, Country = {3}, State = {4}
            WHERE ContactId = {5}
        END
        ELSE
        BEGIN
            INSERT INTO PhysicalAddresses (Street, Zipcode, City, Country, State, ContactId)
            VALUES ({0}, {1}, {2}, {3}, {4}, {5})
        END";



        public const string UpsertEmail = @"
        IF EXISTS (
            SELECT 1 FROM EmailAddresses WHERE ContactId = {1} AND TypeId = {2}
        )
        BEGIN
            UPDATE EmailAddresses
            SET Email = {0}
            WHERE ContactId = {1} AND TypeId = {2}
        END
        ELSE
        BEGIN
            INSERT INTO EmailAddresses (Email, ContactId, TypeId)
            VALUES ({0}, {1}, {2})
        END";

        public const string MarkDeletedContact = "INSERT INTO DeletedContacts(ContactId) VALUES({0})";
        public const string UpdateAvatar = "UPDATE Contacts SET Avatar = {0} WHERE Id = {1}";
        public const string GetAllContactsByUserId = "SELECT * FROM Contacts WHERE UserId = {0} And Id NOT IN (SELECT ContactId AS Id From DeletedContacts)";


        public const string UpdateEmailAddress = "UPDATE EmailAddresses SET Email = {0} WHERE ContactId = {1} AND TypeId = {2}";
        public const string UpdatePhysicalAddress = "UPDATE PhysicalAddresses SET Street = {0}, Zipcode = {1}, City = {2}, Country = {3}, State = {4} WHERE ContactId = { 5 }";

        public const string UpdatePhoneNumber = "UPDATE PhoneNumbers SET CountryCode = {0}, Number = {1} WHERE ContactId = {2} AND TypeId = {3}";

        public const string UpdateBasicContactDetails = "UPDATE Contacts SET FirstName = {0}, LastName = {1}, Company = {2}, Notes = {3}, Tags = {4} WHERE Id = {5}";


        public const string SearchUsingTag = "SELECT * FROM Contacts WHERE Tags LIKE '%' + {0} + '%'AND UserId = {1}";

        public const string SearchUsingName = "SELECT * FROM Contacts WHERE (FirstName LIKE '%' + {0} + '%' OR LastName LIKE '%' + {0} + '%') AND UserId = {1}";
    }
}
