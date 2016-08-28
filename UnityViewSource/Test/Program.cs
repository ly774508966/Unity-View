using System;
using UnityView.Collections;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            float[] array = new[] {0, 1f, 2, 3, 4f, 5, 6, 7, 8, 9f};

            int index = Algorithm.BinarySearchFloat(array, 5.5f);
            Console.WriteLine(index);
            Console.ReadLine();
        }
    }
}
