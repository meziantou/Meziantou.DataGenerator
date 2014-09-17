using System;
using System.Collections.Generic;
using CodeFluent.Runtime.Database.Design;
using CodeFluent.Runtime.Database.Management;
using CodeFluent.Runtime.Utilities;

namespace Meziantou.DataGenerator.Core
{
    public class Settings : Serializable<Settings>
    {
        public Settings()
        {
            this.RecentConnectionStrings = new List<RecentConnectionString>();
        }

        private static readonly Lazy<Settings> _current = new Lazy<Settings>(DeserializeFromConfiguration);
        public static Settings Current
        {
            get { return _current.Value; }
        }

        public void AddRecentConnectionString(DatabaseSystem system, Database database)
        {
            RecentConnectionString connectionString = new RecentConnectionString(system, database);

            if (!RecentConnectionStrings.Contains(connectionString))
            {
                RecentConnectionStrings.Insert(0, connectionString);

                const int maxCount = 20;
                if (RecentConnectionStrings.Count > maxCount)
                {
                    RecentConnectionStrings.RemoveRange(maxCount, RecentConnectionStrings.Count - maxCount);
                }
            }
        }

        public List<RecentConnectionString> RecentConnectionStrings { get; set; }
    }
}
