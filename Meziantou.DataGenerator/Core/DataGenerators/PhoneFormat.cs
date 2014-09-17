namespace Meziantou.DataGenerator.Core.DataGenerators
{
    public class PhoneFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public PhoneFormat(string name, string format)
        {
            Name = name;
            Format = format;
        }

        public string Name { get; private set; }
        public string Format { get; private set; }
    }
}