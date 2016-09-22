using System.Security.Cryptography;
using System.Text;

namespace Gemserk.Lockstep 
{
	public static class ChecksumHelper
	{
		public static string CalculateMD5(string str)
		{
			byte[] md5hash = MD5.Create ().ComputeHash (Encoding.UTF8.GetBytes (str));

			StringBuilder sBuilder = new StringBuilder();

			// Loop through each byte of the hashed data 
			// and format each one as a hexadecimal string.
			for (int i = 0; i < md5hash.Length; i++)
			{
				sBuilder.Append(md5hash[i].ToString("x2"));
			}

			// Return the hexadecimal string.
			return sBuilder.ToString();
		}
	}
}
