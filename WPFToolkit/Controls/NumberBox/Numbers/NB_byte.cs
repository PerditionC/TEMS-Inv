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
    internal class NB_byte : Number<byte?>
    {
        public override bool CanIncrease
        {
            get { return (_current + _step) <= _maximum; }
        }

        public override bool CanDecrease
        {
            get { return (_current - _step) >= _minimum; }
        }

        public override bool AcceptNegative
        {
            get { return false; }
        }

        public override bool NumberIsBelowMinimum
        {
            get { return _current < _minimum; }
        }

        protected override byte? GetMinValue()
        {
            return byte.MinValue;
        }

        protected override byte? GetMaxValue()
        {
            return byte.MaxValue;
        }

        protected override byte? GetDefaultStep()
        {
            return 1;
        }

        protected override void StepUp()
        {
            _current += _step;
        }

        protected override void StepDown()
        {
            _current -= _step;
        }

        protected override bool IsInRange(byte? parsedNumber)
        {
            if (parsedNumber == null)
                return true;
            return parsedNumber <= _maximum;
        }

        protected override bool TryParse(string numberString, out byte? parsed)
        {
            if (string.IsNullOrWhiteSpace(numberString))
            {
                parsed = null;
                return true;
            }

            byte tmp;
            var result = byte.TryParse(numberString, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, _parsingCulture, out tmp);
            parsed = tmp;
            return result;
        }

        public override string ToString()
        {
            if (_current == null)
                return string.Empty;
            return _current.Value.ToString(_parsingCulture);
        }
    }
}