using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Class for global values dealing with Animations
    /// </summary>
    public static class AnimationGlobals
    {
        /// <summary>
        /// A value corresponding to an animation that loops infinitely
        /// </summary>
        public const int InfiniteLoop = -1;
        public const float DefaultAnimSpeed = 1f;

        //Shared animations
        public const string IdleName = "Idle";
        public const string JumpStartName = "JumpStart";
        public const string JumpRisingName = "JumpRising";
        public const string JumpFallingName = "JumpFalling";
        public const string JumpMissName = "JumpMiss";
        public const string JumpLandName = "JumpLand";
        public const string RunningName = "Run";
        public const string HurtName = "Hurt";
        public const string DeathName = "Death";
        public const string VictoryName = "Victory";

        public const string GetItemName = "GetItem";

        public const string SpikedTipHurtName = "SpikedTipHurt";

        public const string TalkName = "Talk";

        /// <summary>
        /// Battle animations specific to playable characters
        /// </summary>
        public static class PlayerBattleAnimations
        {
            public const string ChoosingActionName = "ChoosingAction";
            public const string DangerChoosingActionName = "DangerChoosingAction";
            public const string GuardName = "Guard";
            public const string SuperguardName = "Superguard";
            public const string DangerName = "Danger";
            public const string StarSpecialName = "StarSpecial";
            public const string StarWishName = "StarWish";
            public const string RunAwayFailName = "RunAwayFail";
        }

        /// <summary>
        /// Mario-specific battle animations
        /// </summary>
        public static class MarioBattleAnimations
        {
            public const string HammerPickupName = "HammerPickup";
            public const string HammerWindupName = "HammerWindup";
            public const string HammerSlamName = "HammerSlam";

            public const string StoneCapPutOnName = "StoneCapPutOn";

            public const string TornadoJumpFailName = "TornadoJumpFail";

            public const string MapLiftName = "MapLift";
            public const string SweetTreatReadyThrowName = "SweetTreatReadyThrow";
            public const string SweetTreatThrowName = "SweetTreatThrow";

            public const string ListenName = "Listen";
        }

        /// <summary>
        /// Goombario-specific battle animations.
        /// </summary>
        public static class GoombarioBattleAnimations
        {
            public const string TattleName = "Tattle";
        }

        /// <summary>
        /// Goombella-specific battle animations.
        /// </summary>
        public static class GoombellaBattleAnimations
        {
            public const string TattleStartName = "TattleStart";
            public const string TattleFailName = "TattleFail";
            public const string WinkName = "Wink";
        }

        /// <summary>
        /// Koops-specific battle animations.
        /// </summary>
        public static class KoopsBattleAnimations
        {
            public const string ShellSummonName = "ShellSummon";
        }

        /// <summary>
        /// Parakarry-specific battle animations.
        /// </summary>
        public static class ParakarryBattleAnimations
        {
            public const string AirLiftName = "AirLift";
        }

        /// <summary>
        /// Yoshi-specific battle animations
        /// </summary>
        public static class YoshiBattleAnimations
        {
            public const string GulpEatName = "GulpEat";
            public const string EggLayName = "EggLay";
            public const string EggThrowName = "EggThrow";
        }

        /// <summary>
        /// Watt-specific battle animations.
        /// </summary>
        public static class WattBattleAnimations
        {
            public const string WattElectricChargeName = "WattElectricCharge";
        }

        public static class ParagoombaBattleAnimations
        {
            public const string DiveKickName = "DiveKick";
        }

        public static class ParatroopaBattleAnimations
        {
            public const string ShellShotName = "ShellShot";
        }

        public static class HuffNPuffBattleAnimations
        {
            public const string InhaleName = "Inhale";
            public const string ExhaleName = "Exhale";
        }

        public static class GulpitBattleAnimations
        {
            public const string LickName = "Lick";
            public const string SpitRockName = "SpitRock";
        }

        public static class KoopatrolBattleAnimations
        {
            public const string SummonKoopatrolName = "SummonKoopatrol";
        }

        public static class DuplighostBattleAnimations
        {
            public const string HeadbuttStartName = "HeadbuttStart";
            public const string HeadbuttName = "Headbutt";
            public const string DisguiseStartName = "DisguiseStart";
            public const string DisguiseName = "Disguise";
        }

        /// <summary>
        /// Animations for Shelled BattleEntities.
        /// </summary>
        public static class ShelledBattleAnimations
        {
            public const string EnterShellName = "EnterShell";
            public const string ExitShellName = "ExitShell";
            public const string ShellSpinName = "ShellSpin";

            public const string FlippedName = "Flipped";
        }

        /// <summary>
        /// Animations for Winged BattleEntities.
        /// </summary>
        public static class WingedBattleAnimations
        {
            public const string WingedIdleName = "WingedIdle";
            public const string FlyingName = "Flying";
        }

        /// <summary>
        /// Animations for the Shell Shield Shell.
        /// </summary>
        public static class ShellBattleAnimations
        {
            public const string FullHealthStateName = "FullHealthState";
            public const string MildlyDamagedStateName = "MildlyDamagedState";
            public const string SeverelyDamagedStateName = "SeverelyDamagedState";
        }

        /// <summary>
        /// Status Effect-related animations in battle
        /// </summary>
        public static class StatusBattleAnimations
        {
            public const string StoneName = "StoneName";
            public const string DizzyName = "Dizzy";
            public const string ConfusedName = "Confused";
            public const string InjuredName = "Injured";
            public const string SleepName = "Sleep";
        }
    }
}
