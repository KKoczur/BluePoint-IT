using System;
using System.Dynamic;

namespace NegativeChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Set number");
            string userInput = Console.ReadLine();
            int? negaiveNumberValue;
            while (!TryParseToNegativeInt(userInput, out negaiveNumberValue))
            {
                Console.WriteLine("Set number");
                userInput = Console.ReadLine();

            }

            Console.WriteLine($"Number {negaiveNumberValue} is negtive.");

        }

        static bool TryParseToNegativeInt(string userInput, out int? negativeInt)
        {
            negativeInt = null;

            if (int.TryParse(userInput, out int number))
            {
                if (number < 0)
                {
                    negativeInt = number;
                    return true;
                }
                else
                {

                    return false;
                }
            }
            else
            {
                Console.WriteLine($"Can't parse user input {userInput} to number");
                return false;
            }

        }
    }
}
