using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MeteoSwissApi.Resources.Strings;

namespace MeteoSwissApi.Models
{
    [Serializable]
    public struct WeatherConditionCode : IComparable, IComparable<WeatherConditionCode>, IComparable<int>, IEquatable<WeatherConditionCode>, IFormattable
    {
        public static readonly WeatherConditionCode Unknown = new WeatherConditionCode();

        public static readonly WeatherConditionCode Code1 = new WeatherConditionCode(1);
        public static readonly WeatherConditionCode Code2 = new WeatherConditionCode(2);
        public static readonly WeatherConditionCode Code3 = new WeatherConditionCode(3);
        public static readonly WeatherConditionCode Code4 = new WeatherConditionCode(4);
        public static readonly WeatherConditionCode Code5 = new WeatherConditionCode(5);
        public static readonly WeatherConditionCode Code6 = new WeatherConditionCode(6);
        public static readonly WeatherConditionCode Code7 = new WeatherConditionCode(7);
        public static readonly WeatherConditionCode Code8 = new WeatherConditionCode(8);
        public static readonly WeatherConditionCode Code9 = new WeatherConditionCode(9);
        public static readonly WeatherConditionCode Code10 = new WeatherConditionCode(10);
        public static readonly WeatherConditionCode Code11 = new WeatherConditionCode(11);
        public static readonly WeatherConditionCode Code12 = new WeatherConditionCode(12);
        public static readonly WeatherConditionCode Code13 = new WeatherConditionCode(13);
        public static readonly WeatherConditionCode Code14 = new WeatherConditionCode(14);
        public static readonly WeatherConditionCode Code15 = new WeatherConditionCode(15);
        public static readonly WeatherConditionCode Code16 = new WeatherConditionCode(16);
        public static readonly WeatherConditionCode Code17 = new WeatherConditionCode(17);
        public static readonly WeatherConditionCode Code18 = new WeatherConditionCode(18);
        public static readonly WeatherConditionCode Code19 = new WeatherConditionCode(19);
        public static readonly WeatherConditionCode Code20 = new WeatherConditionCode(20);
        public static readonly WeatherConditionCode Code21 = new WeatherConditionCode(21);
        public static readonly WeatherConditionCode Code22 = new WeatherConditionCode(22);
        public static readonly WeatherConditionCode Code23 = new WeatherConditionCode(23);
        public static readonly WeatherConditionCode Code24 = new WeatherConditionCode(24);
        public static readonly WeatherConditionCode Code25 = new WeatherConditionCode(25);
        public static readonly WeatherConditionCode Code26 = new WeatherConditionCode(26);
        public static readonly WeatherConditionCode Code27 = new WeatherConditionCode(27);
        public static readonly WeatherConditionCode Code28 = new WeatherConditionCode(28);
        public static readonly WeatherConditionCode Code29 = new WeatherConditionCode(29);
        public static readonly WeatherConditionCode Code30 = new WeatherConditionCode(30);
        public static readonly WeatherConditionCode Code31 = new WeatherConditionCode(31);
        public static readonly WeatherConditionCode Code32 = new WeatherConditionCode(32);
        public static readonly WeatherConditionCode Code33 = new WeatherConditionCode(33);
        public static readonly WeatherConditionCode Code34 = new WeatherConditionCode(34);
        public static readonly WeatherConditionCode Code35 = new WeatherConditionCode(35);
        public static readonly WeatherConditionCode Code36 = new WeatherConditionCode(36);
        public static readonly WeatherConditionCode Code37 = new WeatherConditionCode(37);
        public static readonly WeatherConditionCode Code38 = new WeatherConditionCode(38);
        public static readonly WeatherConditionCode Code39 = new WeatherConditionCode(39);
        public static readonly WeatherConditionCode Code40 = new WeatherConditionCode(40);
        public static readonly WeatherConditionCode Code41 = new WeatherConditionCode(41);
        public static readonly WeatherConditionCode Code42 = new WeatherConditionCode(42);

        public static readonly WeatherConditionCode Code51 = new WeatherConditionCode(51);
        public static readonly WeatherConditionCode Code52 = new WeatherConditionCode(52);
        public static readonly WeatherConditionCode Code53 = new WeatherConditionCode(53);
        public static readonly WeatherConditionCode Code54 = new WeatherConditionCode(54);
        public static readonly WeatherConditionCode Code55 = new WeatherConditionCode(55);
        public static readonly WeatherConditionCode Code56 = new WeatherConditionCode(56);
        public static readonly WeatherConditionCode Code57 = new WeatherConditionCode(57);
        public static readonly WeatherConditionCode Code58 = new WeatherConditionCode(58);
        public static readonly WeatherConditionCode Code59 = new WeatherConditionCode(59);
        public static readonly WeatherConditionCode Code60 = new WeatherConditionCode(60);
        public static readonly WeatherConditionCode Code61 = new WeatherConditionCode(61);
        public static readonly WeatherConditionCode Code62 = new WeatherConditionCode(62);
        public static readonly WeatherConditionCode Code63 = new WeatherConditionCode(63);
        public static readonly WeatherConditionCode Code64 = new WeatherConditionCode(64);
        public static readonly WeatherConditionCode Code65 = new WeatherConditionCode(65);
        public static readonly WeatherConditionCode Code66 = new WeatherConditionCode(66);
        public static readonly WeatherConditionCode Code67 = new WeatherConditionCode(67);
        public static readonly WeatherConditionCode Code68 = new WeatherConditionCode(68);
        public static readonly WeatherConditionCode Code69 = new WeatherConditionCode(69);
        public static readonly WeatherConditionCode Code70 = new WeatherConditionCode(70);
        public static readonly WeatherConditionCode Code71 = new WeatherConditionCode(71);
        public static readonly WeatherConditionCode Code72 = new WeatherConditionCode(72);
        public static readonly WeatherConditionCode Code73 = new WeatherConditionCode(73);
        public static readonly WeatherConditionCode Code74 = new WeatherConditionCode(74);
        public static readonly WeatherConditionCode Code75 = new WeatherConditionCode(75);
        public static readonly WeatherConditionCode Code76 = new WeatherConditionCode(76);
        public static readonly WeatherConditionCode Code77 = new WeatherConditionCode(77);
        public static readonly WeatherConditionCode Code78 = new WeatherConditionCode(78);

        public static readonly WeatherConditionCode Code101 = new WeatherConditionCode(101);
        public static readonly WeatherConditionCode Code102 = new WeatherConditionCode(102);
        public static readonly WeatherConditionCode Code103 = new WeatherConditionCode(103);
        public static readonly WeatherConditionCode Code104 = new WeatherConditionCode(104);
        public static readonly WeatherConditionCode Code105 = new WeatherConditionCode(105);
        public static readonly WeatherConditionCode Code106 = new WeatherConditionCode(106);
        public static readonly WeatherConditionCode Code107 = new WeatherConditionCode(107);
        public static readonly WeatherConditionCode Code108 = new WeatherConditionCode(108);
        public static readonly WeatherConditionCode Code109 = new WeatherConditionCode(109);
        public static readonly WeatherConditionCode Code110 = new WeatherConditionCode(110);
        public static readonly WeatherConditionCode Code111 = new WeatherConditionCode(111);
        public static readonly WeatherConditionCode Code112 = new WeatherConditionCode(112);
        public static readonly WeatherConditionCode Code113 = new WeatherConditionCode(113);
        public static readonly WeatherConditionCode Code114 = new WeatherConditionCode(114);
        public static readonly WeatherConditionCode Code115 = new WeatherConditionCode(115);
        public static readonly WeatherConditionCode Code116 = new WeatherConditionCode(116);
        public static readonly WeatherConditionCode Code117 = new WeatherConditionCode(117);
        public static readonly WeatherConditionCode Code118 = new WeatherConditionCode(118);
        public static readonly WeatherConditionCode Code119 = new WeatherConditionCode(119);
        public static readonly WeatherConditionCode Code120 = new WeatherConditionCode(120);
        public static readonly WeatherConditionCode Code121 = new WeatherConditionCode(121);
        public static readonly WeatherConditionCode Code122 = new WeatherConditionCode(122);
        public static readonly WeatherConditionCode Code123 = new WeatherConditionCode(123);
        public static readonly WeatherConditionCode Code124 = new WeatherConditionCode(124);
        public static readonly WeatherConditionCode Code125 = new WeatherConditionCode(125);
        public static readonly WeatherConditionCode Code126 = new WeatherConditionCode(126);
        public static readonly WeatherConditionCode Code127 = new WeatherConditionCode(127);
        public static readonly WeatherConditionCode Code128 = new WeatherConditionCode(128);
        public static readonly WeatherConditionCode Code129 = new WeatherConditionCode(129);
        public static readonly WeatherConditionCode Code130 = new WeatherConditionCode(130);
        public static readonly WeatherConditionCode Code131 = new WeatherConditionCode(131);
        public static readonly WeatherConditionCode Code132 = new WeatherConditionCode(132);
        public static readonly WeatherConditionCode Code133 = new WeatherConditionCode(133);
        public static readonly WeatherConditionCode Code134 = new WeatherConditionCode(134);
        public static readonly WeatherConditionCode Code135 = new WeatherConditionCode(135);
        public static readonly WeatherConditionCode Code136 = new WeatherConditionCode(136);
        public static readonly WeatherConditionCode Code137 = new WeatherConditionCode(137);
        public static readonly WeatherConditionCode Code138 = new WeatherConditionCode(138);
        public static readonly WeatherConditionCode Code139 = new WeatherConditionCode(139);
        public static readonly WeatherConditionCode Code140 = new WeatherConditionCode(140);
        public static readonly WeatherConditionCode Code141 = new WeatherConditionCode(141);
        public static readonly WeatherConditionCode Code142 = new WeatherConditionCode(142);

        public static readonly IEnumerable<WeatherConditionCode> All = new List<WeatherConditionCode>
        {
            Code1,
            Code2,
            Code3,
            Code4,
            Code5,
            Code6,
            Code7,
            Code8,
            Code9,
            Code10,
            Code11,
            Code12,
            Code13,
            Code14,
            Code15,
            Code16,
            Code17,
            Code18,
            Code19,
            Code20,
            Code21,
            Code22,
            Code23,
            Code24,
            Code25,
            Code26,
            Code27,
            Code28,
            Code29,
            Code30,
            Code31,
            Code32,
            Code33,
            Code34,
            Code35,
            Code36,
            Code37,
            Code38,
            Code39,
            Code40,
            Code41,
            Code42,
            Code51,
            Code52,
            Code53,
            Code54,
            Code55,
            Code56,
            Code57,
            Code58,
            Code59,
            Code60,
            Code61,
            Code62,
            Code63,
            Code64,
            Code65,
            Code66,
            Code67,
            Code68,
            Code69,
            Code70,
            Code71,
            Code72,
            Code73,
            Code74,
            Code75,
            Code76,
            Code77,
            Code78,
            Code101,
            Code102,
            Code103,
            Code104,
            Code105,
            Code106,
            Code107,
            Code108,
            Code109,
            Code110,
            Code111,
            Code112,
            Code113,
            Code114,
            Code115,
            Code116,
            Code117,
            Code118,
            Code119,
            Code120,
            Code121,
            Code122,
            Code123,
            Code124,
            Code125,
            Code126,
            Code127,
            Code128,
            Code129,
            Code130,
            Code131,
            Code132,
            Code133,
            Code134,
            Code135,
            Code136,
            Code137,
            Code138,
            Code139,
            Code140,
            Code141,
            Code142,
        };

        private WeatherConditionCode(int value)
        {
            this.Value = value;
        }

        public int Value { get; private set; }

        public static bool TryGetFromValue(int value, out WeatherConditionCode outpar)
        {
            var weatherConditionCode = All.Cast<WeatherConditionCode?>().SingleOrDefault(x => x.Value == value);
            if (weatherConditionCode == null)
            {
                outpar = Unknown;
                return false;
            }

            outpar = weatherConditionCode.Value;
            return true;
        }

        public static WeatherConditionCode FromValue(int value)
        {
            if (!TryGetFromValue(value, out var weatherConditionCode))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(value),
                    $"Value {value} is out of range. " +
                    $"Valid values must be between {All.Min(x => x.Value)} and {All.Max(x => x.Value)}");
            }

            return weatherConditionCode.Value;
        }

        public int CompareTo(object obj)
        {
            if (obj is WeatherConditionCode)
            {
                return this.Value.CompareTo((int)obj);
            }

            return this.Value.CompareTo(obj);
        }

        public int CompareTo(WeatherConditionCode other)
        {
            return this.Value.CompareTo(other.Value);
        }

        public int CompareTo(int other)
        {
            return this.Value.CompareTo(other);
        }

        public bool Equals(WeatherConditionCode other)
        {
            return this.Value.Equals(other.Value);
        }

        public static implicit operator WeatherConditionCode(int v) => FromValue(v);

        public static implicit operator WeatherConditionCode(long v) => FromValue((int)v);

        public static implicit operator int(WeatherConditionCode h) => h.Value;

        public static implicit operator long(WeatherConditionCode h) => h.Value;

        public override string ToString()
        {
            return this.ToString(null, null);
        }

        public string ToString(string format)
        {
            return this.ToString(format, null);
        }

        public string ToString(string format, IFormatProvider provider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "G";
            }

            provider ??= CultureInfo.CurrentCulture;
            var valueString = $"{this.Value}";

            switch (format)
            {
                case "G":
                    var translation = WeatherConditionCodes.ResourceManager.GetString(valueString, (CultureInfo)provider);
                    return translation;
                default:
                    return valueString;
            }
        }
    }
}