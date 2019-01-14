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
    internal abstract class Number<T> : INumber
    {
        protected T _current;
        protected T _minimum;
        protected T _maximum;
        protected T _step;
        protected T _default;
        protected CultureInfo _parsingCulture;

        public object CurrentNumber { get { return _current; } }

        public void Initialize(object number, object minimum, object maximum, object step, object defaultNumber, CultureInfo parsingCulture, CultureInfo predefinedCulture)
        {
            TakeCulture(predefinedCulture);
            TakeMinimum(minimum);
            TakeMaximum(maximum);
            TakeStep(step);
            TakeDefaultNumber(defaultNumber);
            TakeNumber(number);
            TakeCulture(parsingCulture);
        }

        public void TakeCulture(object culture)
        {
            var casted = culture as CultureInfo;
            _parsingCulture = casted ?? CultureInfo.CurrentUICulture;
        }

        public bool TakeNumber(object newNumber)
        {
            T parsedNumber;
            if (TryParse(newNumber, out parsedNumber) && IsInRange(parsedNumber))
            {
                _current = parsedNumber;
                return true;
            }
            return false;
        }

        public void TakeMinimum(object newMinimum)
        {
            if (newMinimum == null)
            {
                _minimum = GetMinValue();
                return;
            }

            T parsedNumber;
            if (TryParse(newMinimum, out parsedNumber))
                _minimum = parsedNumber;
        }

        public void TakeMaximum(object newMaximum)
        {
            if (newMaximum == null)
            {
                _maximum = GetMaxValue();
                return;
            }

            T parsedNumber;
            if (TryParse(newMaximum, out parsedNumber))
                _maximum = parsedNumber;
        }

        public void TakeStep(object newStep)
        {
            T parsedStep;
            TryParse(newStep, out parsedStep);
            _step = parsedStep;
            if (_step == null)
                _step = GetDefaultStep();
        }

        public void TakeDefaultNumber(object newDefaultValue)
        {
            T parsedNumber;
            if (TryParse(newDefaultValue, out parsedNumber))
                _default = parsedNumber;
        }

        public void Increase()
        {
            if (!CanIncrease)
                return;

            StepUp();
        }

        public void Decrease()
        {
            if (!CanDecrease)
                return;

            StepDown();
        }

        public void Reset()
        {
            _current = _default;
        }

        public void ToMaximum()
        {
            _current = _maximum;
        }

        public void ToMinimum()
        {
            _current = _minimum;
        }

        private bool TryParse(object number, out T parsed)
        {
            parsed = default(T);
            if (number == null)
                return true;

            T parsedNumber;
            if (TryParse(number.ToString(), out parsedNumber))
            {
                parsed = parsedNumber;
                return true;
            }
            return false;
        }

        public abstract bool CanIncrease { get; }
        public abstract bool CanDecrease { get; }
        public abstract bool AcceptNegative { get; }
        public abstract bool NumberIsBelowMinimum { get; }
        protected abstract T GetMinValue();
        protected abstract T GetMaxValue();
        protected abstract T GetDefaultStep();
        protected abstract void StepUp();
        protected abstract void StepDown();
        protected abstract bool IsInRange(T parsedNumber);
        protected abstract bool TryParse(string numberString, out T parsed);
    }
}