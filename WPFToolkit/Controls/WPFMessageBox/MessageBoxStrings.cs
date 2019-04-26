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

namespace DW.WPFToolkit.Controls
{
    /// <summary>
    /// Provides messagebox element strings in the current system language.
    /// </summary>
    public class MessageBoxStrings
    {
        private string _ok;
        private string _cancel;
        private string _abort;
        private string _retry;
        private string _ignore;
        private string _yes;
        private string _no;
        private string _help;
        private string _tryAgain;
        private string _continue;
        private string _yesToAll;
        private string _noToAll;
        private string _doNotShowAgain;
        private string _openDetails;
        private string _closeDetails;

        /// <summary>
        /// Gets or sets the translation for 'OK'.
        /// </summary>
        public string OK
        {
            get { return Load(SystemTexts.OK_CAPTION, _ok); }
            set { _ok = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Cancel'.
        /// </summary>
        public string Cancel
        {
            get { return Load(SystemTexts.CANCEL_CAPTION, _cancel); }
            set { _cancel = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Abort'.
        /// </summary>
        public string Abort
        {
            get { return Load(SystemTexts.ABORT_CAPTION, _abort); }
            set { _abort = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Retry'.
        /// </summary>
        public string Retry
        {
            get { return Load(SystemTexts.RETRY_CAPTION, _retry); }
            set { _retry = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Ignore'.
        /// </summary>
        public string Ignore
        {
            get { return Load(SystemTexts.IGNORE_CAPTION, _ignore); }
            set { _ignore = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Yes'.
        /// </summary>
        public string Yes
        {
            get { return Load(SystemTexts.YES_CAPTION, _yes); }
            set { _yes = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'No'.
        /// </summary>
        public string No
        {
            get { return Load(SystemTexts.NO_CAPTION, _no); }
            set { _no = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Help'.
        /// </summary>
        public string Help
        {
            get { return Load(SystemTexts.HELP_CAPTION, _help); }
            set { _help = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Try Again'.
        /// </summary>
        public string TryAgain
        {
            get { return Load(SystemTexts.TRYAGAIN_CAPTION, _tryAgain); }
            set { _tryAgain = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Continue'.
        /// </summary>
        public string Continue
        {
            get { return Load(SystemTexts.CONTINUE_CAPTION, _continue); }
            set { _continue = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Yes To All'.
        /// </summary>
        public string YesToAll
        {
            get { return LoadCustom(YesToAllId, _yesToAll); }
            set { _yesToAll = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'No To All'.
        /// </summary>
        public string NoToAll
        {
            get { return LoadCustom(NoToAllId, _noToAll); }
            set { _noToAll = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Do Not Show Again'.
        /// </summary>
        public string DoNotShowAgain
        {
            get { return LoadCustom(DoNotShowAgainId, _doNotShowAgain); }
            set { _doNotShowAgain = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Open Details'.
        /// </summary>
        public string OpenDetails
        {
            get { return LoadCustom(OpenDetailsId, _openDetails); }
            set { _openDetails = value; }
        }

        /// <summary>
        /// Gets or sets the translation for 'Close Details'.
        /// </summary>
        public string CloseDetails
        {
            get { return LoadCustom(CloseDetailsId, _closeDetails); }
            set { _closeDetails = value; }
        }

        private string Load(uint id, string alternate)
        {
            if (!string.IsNullOrWhiteSpace(alternate))
                return alternate;
            var systemString = SystemTexts.GetString(id);
            return systemString.Replace('&', '_');
        }

        private string LoadCustom(int id, string alternate)
        {
            if (!string.IsNullOrWhiteSpace(alternate))
                return alternate;

            switch (id)
            {
                case YesToAllId:
                    return "Y_es to All";
                case NoToAllId:
                    return "N_o to All";
                case DoNotShowAgainId:
                    return "_Don't show this message again";
                case OpenDetailsId:
                    return "_Show Details";
                case CloseDetailsId:
                    return "_Hide Details";
            }
            return string.Empty;
        }

        private const int YesToAllId = -800;
        private const int NoToAllId = -801;
        private const int DoNotShowAgainId = -802;
        private const int OpenDetailsId = -803;
        private const int CloseDetailsId = -804;
    }
}