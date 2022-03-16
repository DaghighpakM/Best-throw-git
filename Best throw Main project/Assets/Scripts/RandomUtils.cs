using UnityEngine;
using System.Collections.Generic;

public class RandomUtils : MonoBehaviour
{
    /// <summary>
    /// Creates an array of random numbers without repetition
    /// </summary>
    /// <param name="maxNumberForRandom">The random numbers created are between 0 and the maxNumberForRandom.</param>
    /// <param name="sizeOfReturn">The size of the array where we want to randomly insert random numbers into.</param>
    /// <returns> Returns an array of random numbers without repetition with a specified size </returns>
    public static int[] RandomUnRepeat(int maxNumberForRandom, int sizeOfReturn)
    {
        List<int> numbers = new List<int>(maxNumberForRandom);
        int[] randomUnRepeat = new int[sizeOfReturn];

        for (int i = 0; i < maxNumberForRandom; i++)
        {
            numbers.Add(i);
        }

        int random;
        for (int i = 0; i < sizeOfReturn; i++)
        {
            random = Random.Range(0, numbers.Count);
            randomUnRepeat[i] = numbers[random];

            numbers.Remove(numbers[random]);
        }
        return randomUnRepeat;
    }

    /// <summary>
    /// Generate a random number with a random sign (x > 0 or x < 0)
    /// </summary>
    /// <param name="min">Should min >= 0 </param>
    /// <param name="max">Should max > min</param>
    /// <returns>A random float number with a positive or negative sign</returns>
    public static float RandomWithSignRandom(float min, float max)
    {
        float a = Random.Range(min, max);
        int s = Random.Range(0, 2);
        if (s == 1)
            a *= -1;
        return a;
    }

    /// <summary>
    /// By creating a probability value we create a boolean.
    /// </summary>
    /// <param name="probability">Probability value</param>
    /// <returns>Returns true or false depending on the probability</returns>
    public static bool RandomProbabilityTrue(Probability probability)
    {
        bool p = false;
        if (probability == Probability.always)
        {
            p = true;
        }
        else if (probability == Probability.never)
        {
            p = false;
        }
        else
        {
            bool[] bools = null;
            switch (probability)
            {
                case Probability.few:
                    bools = new bool[] { true, false, false, false };
                    break;
                case Probability.medium:
                    bools = new bool[] { true, false };
                    break;
                case Probability.mostly:
                    bools = new bool[] { true, true, true, false };
                    break;
            }
            p = bools[Random.Range(0, bools.Length)];
        }
        return p;
    }
}

/// <summary>
/// Possible Probability sizes
/// </summary>
public enum Probability
{
    never = 0,
    few = 25,
    medium = 50,
    mostly = 75,
    always = 100
}
