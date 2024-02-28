using System;

namespace CC.Common.Effects
{
    [Flags]
    public enum CargoEffectPools
    {
        None = 0,
        Oils = 1,
        Liquids = 2,
        FlammableLiquids = 4 | Liquids,
        CorrosiveLiquids = 8 | Liquids,
        Gases = 16,
        FlammableGases = 32 | Gases,
        ExtiguishingGases = 64 | Gases,
        FlammableSolids = 128,
        RadioactiveCargo = 256,
        ExplosiveCargo = 512,
        Oxidizers = 1024,
    }
}