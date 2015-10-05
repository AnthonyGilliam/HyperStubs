using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using HyperStubs.Enums;
using HyperStubs.Model;

namespace HyperStubs.Core
{
    public static class Randomizer
    {
        private static int _max;
        private static int _min;
        private static Random _random;

        static Randomizer()
        {
            _random = new Random();
        }

        #region Address

        public static string RandomAddress1()
        {
            _max = StubDataLibrary.Address1s.Count();

            return StubDataLibrary.Address1s[RandomInt(0, _max)];
        }

        public static string RandomAddress2()
        {
            _max = StubDataLibrary.Address2s.Count();

            return StubDataLibrary.Address2s[RandomInt(0, _max)];
        }

        public static string RandomCity()
        {
            _max = StubDataLibrary.Cities.Count();

            return StubDataLibrary.Cities[RandomInt(0, _max)];
        }

        public static string RandomUnitedState()
        {
            _max = StubDataLibrary.UnitedStates.Count();

            return StubDataLibrary.UnitedStates[RandomInt(0, _max - 1)];
        }

        public static string RandomZipCode()
        {
            return RandomInt(10001, 99999).ToString();
        }

        public static string RandomCounty()
        {
            _max = StubDataLibrary.Counties.Count();

            return StubDataLibrary.Counties[RandomInt(0, _max)];
        }

        public static string RandomCountry()
        {
            _max = StubDataLibrary.Countries.Count();

            return StubDataLibrary.Countries[RandomInt(0, _max)];
        }
        #endregion Address

        #region Bool
        
        public static bool RandomBool()
        {
            bool[] choice = new bool[] { false, true };
            int randomInt = RandomInt(2, 1000);
            int choiceIndex = randomInt % 2;
            return choice[choiceIndex];
        }

        #endregion Bool

        #region Char

        public static char RandomChar(bool upperCase)
        {
            int charIndex = RandomInt(96, 122);
            char rndmChar = (char)charIndex;

            if (upperCase)
                return char.ToUpper(rndmChar);

            return rndmChar;
        }

        #endregion Char

        #region DateTime

        public static DateTime RandomDate()
        {
            DateTime rndmDate = RandomDate(1990, 2020);

            return rndmDate;
        }

        public static DateTime RandomDate(int minYear, int maxYear)
        {
            int rndmYear = RandomInt(minYear, maxYear);
            int rndmMonth = RandomInt(1, 12);
            int rndmDay = RandomInt(1, DateTime.DaysInMonth(rndmYear, rndmMonth));

            DateTime rndmDate = DateTime.Parse(rndmMonth.ToString() + "/"
                + rndmDay.ToString() + "/"
                + rndmYear.ToString());

            return rndmDate;
        }

        public static DateTime RandomBirthDate()
        {
            DateTime rndmBirthDate = RandomDate(1930, 1990);

            return rndmBirthDate;
        }

        #endregion DateTime

        #region Double

        public static double RandomDouble()
        {
            return RandomDouble(int.MinValue, int.MaxValue);
        }

        public static double RandomDouble(int max)
        {
            return RandomDouble(0, max);
        }

        public static double RandomDouble(int min, int max)
        {
            return _random.Next(min, max) + _random.NextDouble();
        }

        #endregion Double

        #region Int

        public static int RandomInt()
        {
            return RandomInt(int.MinValue, int.MaxValue);
        }

        public static int RandomInt(int max)
        {
            return RandomInt(0, max);
        }

        public static int RandomInt(int min, int max)
        {
            return _random.Next(min, max);
        }

        #endregion Int

        #region Name

        public static string RandomFirstName()
        {
            _max = StubDataLibrary.FirstNames.Count();

            return StubDataLibrary.FirstNames[RandomInt(_max)];
        }

        public static string RandomLastName()
        {
            _max = StubDataLibrary.LastNames.Count();

            return StubDataLibrary.LastNames[RandomInt(_max)];
        }

        public static string RandomFullName()
        {
            string fullName = RandomFirstName() + " " + RandomLastName();

            return fullName;
        }

        public static string RandomCompanyName()
        {
            _max = StubDataLibrary.CompanyNames.Count();

            return StubDataLibrary.CompanyNames[RandomInt(_max)];
        }

        #endregion Name

        #region Number

        public static string RandomPhoneNumber(PhoneNumberFormat format)
        {
            string phoneNumber = null;

            switch(format)
            {
                case PhoneNumberFormat.Decimal :
                    phoneNumber = string.Format("{0}.{1}.{2}"
                      , RandomInt(200, 999)
                      , RandomInt(100, 999)
                      , RandomInt(1000, 9999));
                    break;
                case PhoneNumberFormat.Dash:
                    phoneNumber = string.Format("{0}-{1}-{2}"
                      , RandomInt(200, 999)
                      , RandomInt(100, 999)
                      , RandomInt(1000, 9999));
                    break;
                case PhoneNumberFormat.Parenthesis:
                    phoneNumber = string.Format("({0}) {1}-{2}"
                      , RandomInt(200, 999)
                      , RandomInt(100, 999)
                      , RandomInt(1000, 9999));
                    break;
            }

            return phoneNumber;
        }

        #endregion Number

        #region Word

        public static string RandomWord()
        {
            int size = RandomInt(3, 12);

            return RandomWord(size, false);
        }

        public static string RandomWord(bool upperCase)
        {
            int size = RandomInt(3, 12);

            return RandomWord(size, upperCase);
        }

        public static string RandomWord(int size, bool upperCase)
        {
            StringBuilder wordBuilder = new StringBuilder();
            char nextChar;
            string word = null;

            for (int i = 0; i < size; i++)
            {
                nextChar = RandomChar(false);
                wordBuilder.Append(nextChar);
            }

            word = wordBuilder.ToString();

            if (upperCase)
            {
                return word.ToUpper();
            }
            else
            {
                char firstChar = char.ToUpper(word[0]);
                word = word.Remove(0, 1).Insert(0, firstChar.ToString());
                return word;
            }
        }

        #endregion Word

        #region URL

		public static string RandomWebAddress()
        {
            return "www."
                    + Regex.Replace(RandomCompanyName(), @"[\x20\x27\x2C\.]", "").ToLower()
                    + ".com";
        }

        public static string RandomEmailAddress()
        {
            return Randomizer.RandomChar(false).ToString()
                    + Randomizer.RandomLastName().ToLower()
                    + "@" + Regex.Replace(Randomizer.RandomCompanyName(), @"[\x20\x27\x2C\.]", "").ToLower()
                    + ".com";
        }

        #endregion URL 
    }
}
