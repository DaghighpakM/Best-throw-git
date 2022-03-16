using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Tools
{
    public static bool ChangeIntToBool(int Int)
    {
       return (Int > 0) ? true : false;
    }

    public static int changeBoolToInt(bool b)
    {
        int i = 0;

        if (b)
            i = 1;

        return i;
    }
}
