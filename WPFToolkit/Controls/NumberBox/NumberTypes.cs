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

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Defines which kind of numbers the <see cref="DW.WPFToolkit.Controls.NumberBox" /> is accepting.
    /// </summary>
    /// <remarks>
    /// Number type references:<br />
    /// <a href="https://msdn.microsoft.com/en-us/library/exx3b86w.aspx">Integral Types Table</a><br />
    /// <a href="https://msdn.microsoft.com/en-us/library/9ahet949.aspx">Floating-Point Types Table</a><br />
    /// <a href="https://msdn.microsoft.com/en-us/library/364x0z75.aspx">Decimal</a><br />
    /// <a href="https://msdn.microsoft.com/de-de/library/vstudio/system.numerics.biginteger(v=vs.100)">System.Numerics.BigInteger Structure</a>
    /// </remarks>
    public enum NumberType
    {
        /// <summary>
        /// Represents sbyte or sbyte?.
        /// </summary>
        SByte,

        /// <summary>
        /// Represents byte or byte?.
        /// </summary>
        Byte,

        /// <summary>
        /// Represents short or short?.
        /// </summary>
        Short,

        /// <summary>
        /// Represents ushort or ushort?.
        /// </summary>
        UShort,

        /// <summary>
        /// Represents int or int?.
        /// </summary>
        Int,

        /// <summary>
        /// Represents uint or uint?.
        /// </summary>
        UInt,

        /// <summary>
        /// Represents long or long?.
        /// </summary>
        Long,

        /// <summary>
        /// Represents ulong or ulong?.
        /// </summary>
        ULong,

        /// <summary>
        /// Represents BigInteger or BigInteger?.
        /// </summary>
        BigInteger,

        /// <summary>
        /// Represents float or float?.
        /// </summary>
        Float,

        /// <summary>
        /// Represents double or double?.
        /// </summary>
        Double,

        /// <summary>
        /// Represents decimal or decimal?.
        /// </summary>
        Decimal,
    }
}
