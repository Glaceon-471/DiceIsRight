using UnityEngine;

namespace DiceIsRight;

public static class Helper
{
    public static int RandomInt(int min, int max) => Random.Range(min, max + 1);
    
    public static int D100(out int tens_place, out int ones_place)
    {
        tens_place = RandomInt(0, 9);
        ones_place = RandomInt(0, 9);
        int value = tens_place * 10 + ones_place;
        return value != 0 ? value : 100;
    }
}
