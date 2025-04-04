using LosSantosRED.lsr;
using Rage;
using System;

public class PluginTask
{
    public string DebugName;
    public uint GameTimeLastRan = 0;
    public uint Interval = 500;
    public bool RanThisTick = false;
    public Action<ModController> TickToRun;
    public ModController ModController;
    public PluginTask(uint _Interval, string _DebugName, Action<ModController> _TickToRun, ModController _ModController)
    {
        GameTimeLastRan = 0;
        Interval = _Interval;
        DebugName = _DebugName;
        TickToRun = _TickToRun;
        ModController = _ModController;
    }
    //public bool MissedInterval => Interval != 0 && Game.GameTime - GameTimeLastRan >= IntervalMissLength;
    //public bool RunningBehind => Interval != 0 && Game.GameTime - GameTimeLastRan >= (IntervalMissLength * 2);
    public bool ShouldRun => GameTimeLastRan == 0 || Game.GameTime - GameTimeLastRan > Interval; //public bool ShouldRun => GameTimeLastRan == 0 || Environment.TickCount - GameTimeLastRan > Interval;//CHANING IT TO TICKCOUNT WILL MURDER PERFORMANCE. IS THE TICK COUNT TOO HIGH?
    public void Run()
    {
        TickToRun(ModController);
        GameTimeLastRan = Game.GameTime;
        RanThisTick = true;
    }
}