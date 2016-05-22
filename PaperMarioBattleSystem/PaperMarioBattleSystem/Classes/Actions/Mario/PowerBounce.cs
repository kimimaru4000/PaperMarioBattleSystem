using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    public sealed class PowerBounce : Jump
    {
        private int DamageValue = 0;
        private int Bounces = 0;

        protected override int DamageDealt => DamageValue;

        public PowerBounce()
        {
            Name = "Power Bounce";
            Description = "Bounce multiple times on an enemy";
            BaseDamage = 5;
        }

        protected override void OnStart()
        {
            base.OnStart();

            DamageValue = BaseDamage;
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    DealDamage(DamageDealt);
                    DamageValue = UtilityGlobals.Clamp(DamageValue - 1, 1, BattleGlobals.MaxDamage);

                    Bounces++;

                    if (Bounces < BattleGlobals.MaxPowerBounces)
                    {
                        ChangeSequenceBranch(SequenceBranch.Command);
                    }
                    else
                    {
                        ChangeSequenceBranch(SequenceBranch.End);
                    }
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
