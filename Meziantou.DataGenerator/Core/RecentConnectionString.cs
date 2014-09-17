using CodeFluent.Runtime.Database.Design;
using CodeFluent.Runtime.Database.Management;

namespace Meziantou.DataGenerator.Core
{
    public class RecentConnectionString
    {
        public RecentConnectionString()
        {
            
        }

        public RecentConnectionString(DatabaseSystem system, Database database)
        {
            DatabaseSystem = system;
            ConnectionString = database.ConnectionString;
            Name = database.Name;
        }

        protected bool Equals(RecentConnectionString other)
        {
            return DatabaseSystem == other.DatabaseSystem && string.Equals(ConnectionString, other.ConnectionString);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((RecentConnectionString)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int)DatabaseSystem * 397) ^ (ConnectionString != null ? ConnectionString.GetHashCode() : 0);
            }
        }

        public DatabaseSystem DatabaseSystem { get; set; }
        public string ConnectionString { get; set; }
        public string Name { get; set; }

        public override string ToString()
        {
            return string.Format("{0} - {1} ({2})", DatabaseSystem, Name, ConnectionString);
        }

        public ConnectionStringObject ToConnectionStringObject()
        {
            return ConnectionStringObject.CreateObject(DatabaseSystem, ConnectionString);
        }
    }
}