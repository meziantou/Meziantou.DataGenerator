using System;

namespace Meziantou.DataGenerator.Core.DataGenerators
{
    [Flags]
    public enum CharacterSet
    {
        Unknown = 0x0,
        Custom = 0x1,
        Unicode = 0x2,
        Digits = 0x4,
        LowerAlpha = 0x8,
        UpperAlpha = 0x10,
        Specials = 0x20,
    }
}