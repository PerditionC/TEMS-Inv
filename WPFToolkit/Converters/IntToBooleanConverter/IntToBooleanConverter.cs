// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System;
using System.Windows.Data;
using System.Windows.Markup;
using NLog;

namespace DW.WPFToolkit.Converters
{
    /// <summary>
    /// To allow using radio buttons to get/set an Enum properter
    /// Create multiple RadioButtons and bind similarly
    ///     ... IsSomethingTrue="{Binding Path=MyValueToTest, Converter={uc:IntToBooleanConverter CompareOp}, 
    ///         ConverterParameter={x:Static DataModel:MyInt.MyOtherValue}}" ...
    ///         
    /// We return a MarkupExtesion to create a converter object allow to pass an additional parameter.
    /// The 'value' is passed to the IValueConverter is the {Binding} value
    /// The 'ConverterParameter' is the value it is compared to to make the boolean evaluation
    /// As an Extension we use the CompareOp property to indicate type of comparison to use.
    /// </summary>
    sealed public class IntToBooleanConverterExtension : MarkupExtension, IValueConverter
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static IntToBooleanConverterExtension _instance;

        /// <summary>
        /// The comparison operation to perform.
        /// Operations are spelled out as words to simplify including in XAML (avoiding escaping issues).
        /// Valid values are: Equal, NotEqual, LessThan, GreaterThan, LessThanOrEqual, GreaterThanOrEqual
        /// </summary>
        public string CompareOp { get; set; } = ">";

        /// <summary>
        /// The actual conversion
        /// </summary>
        /// <param name="value1"></param>
        /// <param name="op"></param>
        /// <param name="value2"></param>
        /// <returns></returns>
        private bool DoComparison(object value1, string op, object value2)
        {
            if (value1 == null) throw new ArgumentOutOfRangeException("IntToBooleanConverter: unable to do conversion, binding value is null");
            if (value2 == null) throw new ArgumentOutOfRangeException("IntToBooleanConverter: unable to do conversion, parameter value is null");

            if (!IsNumericType(value1.GetType())) throw new ArgumentOutOfRangeException("IntToBooleanConverter: unable to do conversion, binding value is not numeric");
            if (!IsNumericType(value2.GetType())) throw new ArgumentOutOfRangeException("IntToBooleanConverter: unable to do conversion, parameter value is not numeric");

            // Note: we assume actual integers are required, allows using floating point as well        
            var val1 = System.Convert.ToDouble(value1);
            var val2 = System.Convert.ToDouble(value2);

            switch (op)
            {
                case "==":
                case "Equal":
                    return val1 == val2;
                case "!=":
                case "<>":
                case "NotEqual":
                    return val1 != val2;
                case "<":
                case "LessThan":
                    return val1 < val2;
                case ">":
                case "GreaterThan":
                    return val1 > val2;
                case "<=":
                case "LessThanOrEqual":
                    return val1 <= val2;
                case ">=":
                case "GreaterThanOrEqual":
                    return val1 >= val2;
                default:
                    throw new ArgumentOutOfRangeException($"IntToBooleanConverter: invalid comparison operation {CompareOp} provided");
            }
        }

        /// <summary>
        /// returns if given type can be converted to a numerical value
        /// </summary>
        /// <param name="type"></param>
        /// <returns>true if Type can be converted to a numerical value, false otherwise</returns>
        private bool IsNumericType(Type type)
        {
            if (type == null)
                return false;

            switch (Type.GetTypeCode(type))
            {
                // check if type of one of common numerical types
                case TypeCode.String: // Note: exception thrown if other than convertable digits
                case TypeCode.Byte:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Single:
                case TypeCode.Double:
                case TypeCode.Decimal:
                    return true;
                // recursive check, if Type is a Nullable type then assuming value is not null ok as long as base type can be converted to a numerical value
                // Note: if the Nullable value has a null value and is passed to converter then an exception will be thrown
                case TypeCode.Object:
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        return IsNumericType(Nullable.GetUnderlyingType(type));
                    }
                    return false;
            }

            // otherwise assume not a numeric type
            return false;
        }


        // Convert value to boolean, returns (param1 op param2)
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug($"IntToBooleanConverterExtension Convert=>{value?.ToString()} OP[{CompareOp}] {parameter?.ToString() ?? "null using 0"}");
            return DoComparison(value, CompareOp, parameter);
        }

        // Convert boolean to enum, returning [param] if true
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            logger.Debug("EnumToBooleanConverter ConvertBack=>value=" + value?.ToString());
            throw new NotImplementedException();
            //return (bool)value ? parameter : Binding.DoNothing;
        }

        /// <summary>
        /// This is the trick that allows this object to be used as a Converter, we return ourselves for the Converter markup.
        /// </summary>
        /// <param name="serviceProvider"></param>
        /// <returns>this (ourselves)</returns>
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return _instance ?? (_instance = new IntToBooleanConverterExtension());
        }
    }
}
