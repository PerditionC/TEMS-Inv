// This is an independent project of an individual developer. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com

using System.Text.RegularExpressions;

using NLog;

namespace TEMS.InventoryModel.util
{
    public class ItemNumberParser
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// A regex pattern do do validation and extract components
        /// optional letter, #, optional dash, optional #, optional dash, optional letters
        ///   equip code    item type id  -     item #         -           site code
        /// </summary>
        private const string RegexPattern = "^(?<EquipCode>[a-z,A-Z])?(?<ItemTypeId>[0-9]+)(?:-?(?<ItemId>[0-9]+))?(?:-?(?<SiteCode>[a-z,A-Z]*))?$";

        /// <summary>
        /// We maintain a single Regex object for the life of this instance, same pattern is always used, our Item Number pattern.
        /// </summary>
        private Regex regex = new Regex(RegexPattern);

        /// <summary>
        /// the provided input text to validate and parse
        /// </summary>
        private string inputText
        {
            get { return _inputText; }
            set { _inputText = value?.Trim(); isItemNumber = null; }
        }

        private string _inputText;

        /// <summary>
        /// cached results
        /// </summary>
        private bool? isItemNumber = null;

        /// <summary>
        /// After parsed, the value of components within the item number provided, only itemTypeId guaranteed to not be string.Empty ("") if IsItemNumber returns true.
        /// </summary>
        public string equipCode { get; private set; } = string.Empty;

        public string itemTypeId { get; private set; } = string.Empty;
        public string itemId { get; private set; } = string.Empty;
        public string siteCode { get; private set; } = string.Empty;

        /// <summary>
        /// Constructor, inputText must be passed to IsItemNumber for valid results.
        /// </summary>
        public ItemNumberParser() { }

        /// <summary>
        /// Constructor, must provide input text used to validate and parse item#
        /// </summary>
        /// <param name="inputText">input text to be validated and parsed, leading & trailing whitespace is ignored</param>
        public ItemNumberParser(string inputText) : this()
        {
            this.inputText = inputText;
        }

        /// <summary>
        /// Updates the internal input text and returns results based on new input text, see IsItemNumber();
        /// </summary>
        /// <param name="inputText">input text to be validated and parsed, leading & trailing whitespace is ignored</param>
        /// <returns>true if inputText is parsable as an item number, false otherwise</returns>
        public bool IsItemNumber(string inputText)
        {
            this.inputText = inputText;
            return IsItemNumber();
        }

        /// <summary>
        /// Should be called to validate and parse provided inputText to determine if is an item number and obtain component values.
        /// May be called multiple times, only 1st time will parse, same results returned there after.
        /// All values are string.Empty ("") if not provided in inputText; if inputText is not an item number all will be string.Empty otherwise provided components will match corresponding value.
        /// updates: searchText - possibly item # to validate; item is in form of D###-####-AAA where D, second ####, AAA, and dashes are optional (1st dash is required if item id provided)</param>
        /// updates: equipCode - optional, 1st letter of item#, the equipment item is within; redundant if itemId is provided</param>
        /// updates: itemTypeId - the 1st #, required to be considered a item #, the id of the item's type (ItemType table)</param>
        /// updates: itemId - optional, the 2nd number after a - (single dash), the id of the item (Item table)</param>
        /// updates: siteCode - optional, 1st letter of item#, the site code, when combined with item type & item id represents an Item Instance id</param>
        /// </summary>
        /// <returns>true if inputText is parsable as an item number, false otherwise</returns>
        public bool IsItemNumber()
        {
            if (isItemNumber == null)
            {
                // if no number specified yet return false but don't cache result yet
                if (inputText == null)
                {
                    return false;
                }
                else // determine if matches to an item number
                {
                    Match match = regex.Match(inputText);
                    if (match.Success)
                    {
                        equipCode = match.Groups["EquipCode"].Value;
                        itemTypeId = match.Groups["ItemTypeId"].Value;
                        itemId = match.Groups["ItemId"].Value;
                        siteCode = match.Groups["SiteCode"].Value;
                        return true;
                    }
                    else
                    {
                        equipCode = string.Empty;
                        itemTypeId = string.Empty;
                        itemId = string.Empty;
                        siteCode = string.Empty;
                        return false;
                    }
                }
            }
            else
            {
                return (bool)isItemNumber;
            }
        }
    }
}