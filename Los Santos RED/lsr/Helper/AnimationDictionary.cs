﻿using Rage;
using Rage.Native;

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

public static class AnimationDictionary
{
    public static void RequestAnimationDictionay(string sDict)
    {
        if (sDict != "")
        {
            NativeFunction.CallByName<bool>("REQUEST_ANIM_DICT", sDict);
            uint GameTimeStartedRequesting = Game.GameTime;
            while (!NativeFunction.CallByName<bool>("HAS_ANIM_DICT_LOADED", sDict) && Game.GameTime - GameTimeStartedRequesting <= 100)
            {
                GameFiber.Yield();
            }
        }
    }

}