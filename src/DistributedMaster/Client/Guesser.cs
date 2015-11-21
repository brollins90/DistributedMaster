namespace DistributedMaster.Client
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Text;

    public class Guesser
    {
        HashAlgorithm algo = new SHA256CryptoServiceProvider();
        private static IList<string> _words4;
        private static IList<string> _words5;
        private int _endIndex;
        private int _startIndex;

        public Guesser(IList<string> words4, IList<string> words5, int startIndex, int endIndex)
        {
            _words4 = words4;
            _words5 = words5;
            _startIndex = startIndex;
            _endIndex = endIndex;
        }

        public bool Go(string lookingFor, out string result)
        {
            result = "";
            for (int i1 = _startIndex; i1 <= _endIndex; i1++)
            {
                Console.WriteLine($"{i1} {_words4[i1]}");
                for (int i2 = 0; i2 < _words5.Count; i2++)
                {
                    Console.WriteLine($"{i1} {_words4[i1]} - {i2} {_words5[i2]}");
                    for (int i3 = 0; i3 < _words4.Count; i3++)
                    {
                        Console.WriteLine($"{i1} {_words4[i1]} - {i2} {_words5[i2]} - {i3} {_words4[i3]}");
                        for (int i4 = 0; i4 < _words5.Count; i4++)
                        {
                            for (int i5 = 0; i5 < _words4.Count; i5++)
                            {
                                string s = $"{_words4[i1]} {_words5[i2]} {_words4[i3]} {_words5[i4]} {_words4[i5]}";
                                if (Check(s, lookingFor))
                                {
                                    result = s;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
            return false;
        }

        private bool Check(string s, string lookingFor)
        {
            var algorithm = new SHA256Managed();
            Byte[] inputBytes = Encoding.UTF8.GetBytes(s);
            Byte[] hashedBytes = algorithm.ComputeHash(inputBytes);
            string sha = BitConverter.ToString(hashedBytes);

            if (sha.Equals(lookingFor))
            {
                string shaNoHyphons = sha.Replace("-", "");
                Console.WriteLine(s);
                Console.WriteLine(sha);
                Console.WriteLine(shaNoHyphons);
                Console.WriteLine();
                return true;
            }
            return false;
        }
    }
}