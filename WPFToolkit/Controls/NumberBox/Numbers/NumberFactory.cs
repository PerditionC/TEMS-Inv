// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

#region License
/*
The MIT License (MIT)

Copyright (c) 2009-2016 David Wendland

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE
*/
#endregion License

namespace DW.WPFToolkit.Controls.Numbers
{
    internal static class NumberFactory
    {
        internal static INumber Create(NumberType numberType)
        {
            switch (numberType)
            {
                case NumberType.SByte: return new NB_sbyte();
                case NumberType.Byte: return new NB_byte();
                case NumberType.Short: return new NB_short();
                case NumberType.UShort: return new NB_ushort();
                case NumberType.Int: return new NB_int();
                case NumberType.UInt: return new NB_uint();
                case NumberType.Long: return new NB_long();
                case NumberType.ULong: return new NB_ulong();
                case NumberType.BigInteger: return new NB_BigInteger();
                case NumberType.Float: return new NB_float();
                case NumberType.Double: return new NB_double();
                case NumberType.Decimal: return new NB_decimal();
            }

            return new NB_int();
        }
    }
}