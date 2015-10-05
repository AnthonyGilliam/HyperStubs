using System;
using HyperStubs.Enums;
using HyperStubs.Random;

namespace HyperStubs.Core
{
    internal static class DataTypeEvaluator
    {
        public static string EvaluateString(string propertyFieldName)
        {
            propertyFieldName = propertyFieldName.ToUpper();

            if (propertyFieldName.Contains("WEBADDRESS"))
                return Randomizer.RandomWebAddress();

            if (propertyFieldName == "EMAILADDRESS" || propertyFieldName.Contains("EMAIL"))
                return Randomizer.RandomEmailAddress();
            
            if (propertyFieldName == "ADDRESS2" || propertyFieldName == "STREET2")
                return Randomizer.RandomAddress2();

            if (propertyFieldName == "ADDRESS" || propertyFieldName == "ADDRESS1" || propertyFieldName == "STREET1" || propertyFieldName.Contains("ADDRESS"))
                return Randomizer.RandomAddress1();

            if (propertyFieldName.Contains("CITY") || propertyFieldName.Contains("CITIES"))
                return Randomizer.RandomCity();

            if (propertyFieldName.Contains("STATE"))
                return Randomizer.RandomUnitedState();

            if (propertyFieldName.Contains("COUNTY") || propertyFieldName.Contains("COUNTIES"))
                return Randomizer.RandomCounty();

            if (propertyFieldName.Contains("COUNTRY") || propertyFieldName.Contains("COUNTRIES"))
                return Randomizer.RandomCountry();

            if (propertyFieldName == "ZIP" || propertyFieldName.Contains("ZIPCODE"))
                return Randomizer.RandomZipCode();

            if ((propertyFieldName.Contains("FIRST") || propertyFieldName.Contains("MIDDLE")) && propertyFieldName.Contains("NAME"))
                return Randomizer.RandomFirstName();

            if (propertyFieldName.Contains("LAST") && propertyFieldName.Contains("NAME"))
                return Randomizer.RandomLastName();

            if (propertyFieldName == "NAME" || propertyFieldName == "FULLNAME")
                return Randomizer.RandomFullName();

            if (propertyFieldName.Contains("PHONE") || propertyFieldName.Contains("FAX"))
                return Randomizer.RandomPhoneNumber(PhoneNumberFormat.Parenthesis);

            if ((propertyFieldName.Contains("FIRM") || propertyFieldName.Contains("COMPANY")) && propertyFieldName.Contains("NAME"))
                return Randomizer.RandomCompanyName();

            return Randomizer.RandomWord();
        }

        public static byte EvaluateByte(string propertyFieldName, bool random = true)
        {
            propertyFieldName = propertyFieldName.ToUpper();

            if (propertyFieldName == "AGE")
                return Randomizer.RandomAge();

            return random ? Randomizer.RandomByte() : byte.MinValue;
        }

        public static int EvaluateInt(string propertyFieldName, bool random = true)
        {
            propertyFieldName = propertyFieldName.ToUpper();

            var value = EvaluateByte(propertyFieldName, false);
            if (value > 0)
                return value;

            if (propertyFieldName.EndsWith("ID") || propertyFieldName.Contains("IDENTIFICATION"))
                return Randomizer.RandomInt(10000000, 99999999);

            return random ? Randomizer.RandomInt() : 0;
        }

        public static decimal EvaluateDecimal(string propertyFieldName)
        {
            propertyFieldName = propertyFieldName.ToUpper();

            if (propertyFieldName.Contains("PRICE")
                || propertyFieldName.Contains("COST")
                || propertyFieldName.Contains("MONEY"))
                return Randomizer.RandomPrice();

            return Randomizer.RandomDecimal();
        }

        public static object GenerateStubObject(Type type, StubDataType stubDataType)
        {
            object stubObject = null;

            switch (stubDataType)
            {
                case StubDataType.Address1:
                    stubObject = Randomizer.RandomAddress1();
                    break;

                case StubDataType.Address2:
                    stubObject = Randomizer.RandomAddress2();
                    break;

                case StubDataType.Age:
                    stubObject = Randomizer.RandomAge();
                    break;

                case StubDataType.BirthDate:
                    stubObject = Randomizer.RandomBirthDate();
                    break;

                case StubDataType.Bool:
                    stubObject = Randomizer.RandomBool();
                    break;

                case StubDataType.City:
                    stubObject = Randomizer.RandomCity();
                    break;

                case StubDataType.CompanyName:
                    stubObject = Randomizer.RandomCompanyName();
                    break;

                case StubDataType.County:
                    stubObject = Randomizer.RandomCounty();
                    break;

                case StubDataType.Country:
                    stubObject = Randomizer.RandomCounty();
                    break;

                case StubDataType.Character:
                    stubObject = Randomizer.RandomChar(true);
                    break;

                case StubDataType.Date:
                    stubObject = Randomizer.RandomDate();
                    break;

                case StubDataType.FirstName:
                    stubObject = Randomizer.RandomFirstName();
                    break;

                case StubDataType.LastName:
                    stubObject = Randomizer.RandomLastName();
                    break;

                case StubDataType.Number:
                    stubObject = Randomizer.RandomInt();
                    break;

                //case StubDataType.Paragraph:
                //    stubObject = Randomizer.RandomParagraph();
                //    break;

                case StubDataType.Price:
                    stubObject = Randomizer.RandomPrice();
                    break;

                case StubDataType.State:
                    stubObject = Randomizer.RandomUnitedState();
                    break;

                case StubDataType.Word:
                    stubObject = Randomizer.RandomWord();
                    break;

                case StubDataType.ZipCode:
                    stubObject = Randomizer.RandomZipCode();
                    break;

                default:
                    stubObject = Randomizer.RandomWord();
                    break;
            }

            return stubObject;
        }
    }
}
