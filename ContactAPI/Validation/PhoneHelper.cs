using PhoneNumbers;
namespace ContactAPI.Validation
{
    public static class PhoneHelper
    {
        private static readonly PhoneNumberUtil Util = PhoneNumberUtil.GetInstance();
        public static bool TryNormalise(string input, out string e164)
        {
            e164 = string.Empty;
            try
            {
                var number = Util.Parse(input, "GB");
                if (!Util.IsValidNumber(number)) return false;
                
                e164 = Util.Format(number, PhoneNumberFormat.E164);
                return true;
            }
            catch (NumberParseException)
            {
                return false;
            }
        }
    }
}
