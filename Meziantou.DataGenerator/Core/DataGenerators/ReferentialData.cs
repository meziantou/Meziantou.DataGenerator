using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Meziantou.DataGenerator.Diagnostics;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class ReferentialData
    {
        private static readonly Lazy<List<string>> _lastNames;
        private static readonly Lazy<List<string>> _femaleFirstNames;
        private static readonly Lazy<List<string>> _maleFirstNames;
        private static readonly Lazy<List<string>> _userNames;
        private static readonly Lazy<List<string>> _emailDomainsWithoutLld;
        private static readonly Lazy<List<string>> _topLevelDomains;

        static ReferentialData()
        {
            _femaleFirstNames = new Lazy<List<string>>(() => LoadValuesFromResources("FirstNameFemale.txt").ToList());
            _maleFirstNames = new Lazy<List<string>>(() => LoadValuesFromResources("FirstNameMale.txt").ToList());
            _lastNames = new Lazy<List<string>>(() => LoadValuesFromResources("LastName.txt").ToList());
            _userNames = new Lazy<List<string>>(() => LoadValuesFromResources("UserNames.txt").ToList());
            _emailDomainsWithoutLld = new Lazy<List<string>>(() => LoadValuesFromResources("EmailDomainsWithoutTld.txt").ToList());
            _topLevelDomains = new Lazy<List<string>>(() => LoadValuesFromResources("tld.txt").ToList());
        }

        public static IList<string> FemaleFirstNames
        {
            get { return _femaleFirstNames.Value; }
        }

        public static IList<string> MaleFirstNames
        {
            get { return _maleFirstNames.Value; }
        }

        public static IReadOnlyList<string> FirstNames
        {
            get { return new ConcatList<string>(_maleFirstNames.Value, _femaleFirstNames.Value); }
        }

        public static IList<string> LastNames
        {
            get { return _lastNames.Value; }
        }

        public static IList<string> UserNames
        {
            get { return _userNames.Value; }
        }

        public static IList<string> EmailDomainsWithoutLld
        {
            get { return _emailDomainsWithoutLld.Value; }
        }

        public static IList<string> TopLevelDomains
        {
            get { return _topLevelDomains.Value; }
        }

        private static IEnumerable<string> LoadValuesFromResources(string fileName)
        {
            List<string> result = new List<string>();
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                string resourceName = assembly.GetName().Name + ".Resources." + fileName;
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            result.Add(line);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log(LogType.Error, value: ex);
            }

            return result;
        }

        public static void LoadAll()
        {
            Logger.Log(LogType.Verbose, value: "ReferentialData loading...");
            object o;
            o = FemaleFirstNames;
            o = MaleFirstNames;
            o = LastNames;
            o = UserNames;
            o = EmailDomainsWithoutLld;
            o = TopLevelDomains;
            Logger.Log(LogType.Verbose, value: "ReferentialData loaded");
        }
    }
}
