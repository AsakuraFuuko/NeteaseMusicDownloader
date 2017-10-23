using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseMusicDownloader.Utils
{
    public class RSA
    {
        public static String encrypt(String text, String pubKey, String modulus)
        {

            String reverseText = new string(text.ToCharArray().Reverse().ToArray());
            BigInteger src = BigInteger.Parse(stringToHex(reverseText), System.Globalization.NumberStyles.HexNumber);
            BigInteger pub = BigInteger.Parse(pubKey, System.Globalization.NumberStyles.HexNumber);
            BigInteger modu = BigInteger.Parse(modulus, System.Globalization.NumberStyles.HexNumber);

            BigInteger res = BigInteger.ModPow(src, pub, modu);
            return ZFill(String.Format("{0:x}", res), 256);
        }

        private static String stringToHex(String strPart)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strPart.Length; i++)
            {
                int ch = (int)strPart[i];
                String strHex = ch.ToString("x");
                sb.Append(strHex);
            }
            return sb.ToString();
        }

        private static string ZFill(string str, int size)
        {
            while (str.Length < size) str = "0" + str;
            return str.Substring(str.Length - size);
        }
    }
}
