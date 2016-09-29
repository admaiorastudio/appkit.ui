namespace AdMaiora.AppKit.UI
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Text.RegularExpressions;

    public class WidgetValidator
    {
        #region Inner Classes

        class Validator
        {
            public Func<bool> RuleValidator
            {
                get;
                set;
            }

            public string ErrorMessage
            {
                get;
                set;
            }
        }

        #endregion

        #region Validation Rules

        public static bool IsNullOrEmpty(string value)
        {
            return String.IsNullOrWhiteSpace(value);
        }

        public static bool IsNotNullOrEmpty(string value)
        {
            return !String.IsNullOrWhiteSpace(value);
        }

        public static bool IsEmail(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            return Regex.IsMatch(value,
                @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
                RegexOptions.IgnoreCase);
        }

        public static bool IsPasswordMin8(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            return value.Length >= 8;
        }

        public static bool IsPasswordMin8AlphaNumeric(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            return Regex.IsMatch(value,
                @"(?!^[0-9]*$)(?!^[a-zA-Z]*$)^([a-zA-Z0-9]{8})$");
        }

        public static bool IsDate(string value)
        {
            if (String.IsNullOrWhiteSpace(value))
                return false;

            DateTime date = DateTime.MinValue;
            return DateTime.TryParse(value, out date);
        }

        #endregion

        #region Constants and Fields

        private List<Validator> _validators;

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public WidgetValidator()
        {
            _validators = new List<Validator>();
        }

        #endregion

        #region Public Methods

        public WidgetValidator AddValidator<TValue>(Func<TValue> getter, Func<TValue, bool> rule, string errorMessage)
        {
            _validators.Add(new Validator
            {
                RuleValidator = () => rule(getter()),
                ErrorMessage = errorMessage
            });
            return this;
        }

        public bool Validate(out string errorMessage)
        {
            errorMessage = null;

            if (_validators.Count == 0)
                return true;

            foreach (var v in _validators)
            {
                if (!v.RuleValidator())
                {
                    errorMessage = v.ErrorMessage;
                    return false;
                }
            }

            return true;
        }

        #endregion
    }

}
