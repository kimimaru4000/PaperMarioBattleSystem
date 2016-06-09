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
            Bounces = 0;
        }

        protected override void OnEnd()
        {
            base.OnEnd();

            Bounces = 0;
        }

        public override void OnCommandSuccess()
        {
            base.OnCommandSuccess();

            Bounces++;
        }

        protected override void SequenceSuccessBranch()
        {
            switch (SequenceStep)
            {
                case 0:
                    if (Bounces < BattleGlobals.MaxPowerBounces)
                    {
                        ChangeSequenceBranch(SequenceBranch.Command);
                    }
                    else
                    {
                        Debug.Log($"Reached Power Bounce limit with {Bounces} and real max is {BattleGlobals.MaxPowerBounces}!");
                        ChangeSequenceBranch(SequenceBranch.End);
                    }

                    AttemptDamage(DamageDealt, EntitiesAffected);
                    DamageValue = UtilityGlobals.Clamp(DamageValue - 1, 1, BattleGlobals.MaxDamage);
                    break;
                default:
                    PrintInvalidSequence();
                    break;
            }
        }
    }
}
