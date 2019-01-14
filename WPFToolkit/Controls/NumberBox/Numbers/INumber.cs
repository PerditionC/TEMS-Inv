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

using System.Globalization;

namespace DW.WPFToolkit.Controls.Numbers
{
    internal interface INumber
    {
        void Initialize(object number, object minimum, object maximum, object step, object defaultNumber, CultureInfo parsingCulture, CultureInfo predefinedCulture);
        void TakeCulture(object culture);
        bool TakeNumber(object newNumber);
        void TakeMinimum(object newMinimum);
        void TakeMaximum(object newMaximum);
        void TakeStep(object newStep);
        void TakeDefaultNumber(object newDefaultValue);
        void Increase();
        void Decrease();
        void Reset();
        void ToMaximum();
        void ToMinimum();

        object CurrentNumber { get; }
        bool CanIncrease { get; }
        bool CanDecrease { get; }
        bool AcceptNegative { get; }
        bool NumberIsBelowMinimum { get; }
    }
}