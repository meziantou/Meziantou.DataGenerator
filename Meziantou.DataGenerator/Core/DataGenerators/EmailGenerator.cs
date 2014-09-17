using System.Collections.Generic;
using System.Net.Mail;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class EmailGenerator : DataGenerator
    {
        public EmailGenerator()
        {
            WellKnownDataType = WellKnownDataType.Email;
        }

        public override bool CanGenerate(Column column)
        {
            if (IsGeneratedColumn(column) || IsForeginKey(column))
                return false;

            return IsStringDbType(column);
        }

        public override int CompareTo(DataGenerator generator)
        {
            if (generator.WellKnownDataType == WellKnownDataType.FirstName ||
                generator.WellKnownDataType == WellKnownDataType.LastName ||
                generator.WellKnownDataType == WellKnownDataType.UserName)
            {
                return 1;
            }

            return base.CompareTo(generator);
        }


        protected override object GenerateCore(Project project, Column column)
        {
            IList<string> formats = new[]
            {
                "{0}.{2}@{5}.{6}", // FirstName.LastName@Domain
                "{1}.{2}@{5}.{6}", // FirstName[0].LastName@Domain
                "{3}@{5}.{6}",     // LastName@Domain
                "{2}.{0}@{5}.{6}", // LastName.FirstName@Domain
                "{2}.{1}@{5}.{6}", // LastName.FirstName[0]@Domain
                "{4}@{5}.{6}"      // username@Domain
            };

            for (int i = 0; i < 10; i++)
            {

                string firstName = project.CurrentRow.GetValue(WellKnownDataType.FirstName) as string ?? Random.NextFromList(ReferentialData.FirstNames);
                string lastName = project.CurrentRow.GetValue(WellKnownDataType.LastName) as string ?? Random.NextFromList(ReferentialData.LastNames);
                string domain = Random.NextFromList(ReferentialData.EmailDomainsWithoutLld);
                string tld = Random.NextFromList(ReferentialData.TopLevelDomains);
                string username = project.CurrentRow.GetValue(WellKnownDataType.UserName) as string ?? Random.NextFromList(ReferentialData.UserNames);

                string format = Random.NextFromList(formats);
                string email = string.Format(format, firstName, firstName[0], lastName, lastName[0], username, domain, tld);
                if (IsValid(email))
                    return email;
            }

            return null;
        }

        public static bool IsValid(string email)
        {
            try
            {
                new MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}