using System.Collections.Generic;
using System.Linq;
using CodeFluent.Runtime.Database.Management;
using Meziantou.DataGenerator.Utilities;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public abstract class LookupGenerator : DataGenerator
    {
        private IList<object> _values = null;

        protected IList<object> Values
        {
            get { return _values; }
        }

        protected abstract IEnumerable<object> LoadValues();

        protected override object GenerateCore(Project project, Column column)
        {
            if (_values == null)
            {
                _values = LoadValues().ToList();
            }

            if (_values == null || _values.Count == 0)
                return null;
            
            return Random.NextFromList(_values);
        }
    }
}