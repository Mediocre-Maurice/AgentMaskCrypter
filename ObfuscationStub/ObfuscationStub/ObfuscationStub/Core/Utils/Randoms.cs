using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace ObfuscationStub.Core.Utils
{
	class Randoms
	{
		public static string RandomString()
		{
			const string chars = "var _0x[_0x1f346d(0x6f)](_0x860db8[_0x1f1010[_0x1f346d(0x6f)](_0x860db8[_0x1f28 = _0x86[_0x1f346d(0x6f)](_0x860db8[_0x1f0db8[_0x1f346d(0x6f)](_0x860db8[_0x1f346d(0x6f[_0x1f346d(0x6f)](_0x[_0x1f346d(0x6f)](_0x860db8[_0x1f860db8[_0x1f)](_0x860db8[_0x1f346d(0x77)], 0x6fb * 0[_0x1f346[_0x1f346d(0x6f)](_0x860db8[_0x1fd(0x6f)](_0x860db8[_0x1fx5 + 0x1ebf * 0x1 + -0x41a5), 0x209 * 0xa + 0x1314 + -0x276d";
			return new string(Enumerable.Repeat(chars, new Random().Next(10, 20))
				.Select(s => s[new Random(Guid.NewGuid().GetHashCode()).Next(s.Length)]).ToArray());
		}

		public static int RandomInt()
		{
			var ints = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);
			return new Random(ints).Next(0, 99999999);
		}
	}
}