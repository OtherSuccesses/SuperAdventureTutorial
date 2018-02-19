using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace Engine
{
    //There is no way that I actually understand this random number generator.
    //It's my understanding that it works better than the native Random() function.
    public static class RandomNumberGenerator
    {
        private static readonly RNGCryptoServiceProvider _generator = new RNGCryptoServiceProvider();

        public static int NumberBetween(int minimumValue, int maximumValue)
        {
            byte[] randomNumber = new byte[1];

            _generator.GetBytes(randomNumber);

            double asciiValueOfRandomCharacter = Convert.ToDouble(randomNumber[0]);

            //We are using Math.Max and subtracting 0.00000000001, to ensure "multiplier"
            //will always be between 0.0 and 0.99999999999 or it could be 1 and cause problems in rounding
            double multiplier = Math.Max(0, (asciiValueOfRandomCharacter / 255d) - 0.00000000001d);

            //1 needs to be added to the range to account for the rounding in Math.Floor
            int range = maximumValue - minimumValue + 1;

            double randomValueInRange = Math.Floor(multiplier * range);

            return (int)(minimumValue + randomValueInRange);
        }
    }
}
