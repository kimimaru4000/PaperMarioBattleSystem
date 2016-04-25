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
        private bool Repeat = false;
        private int Bounces = 0;

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

        public override void OnCommandSuccess()
        {
            Repeat = true;
        }

        public override void OnCommandFailed()
        {
            
        }

        protected override void OnProgressSequence()
        {
            switch (SequenceStep)
            {
                case 0:
                case 1:
                case 2:
                    Repeat = false;
                    base.OnProgressSequence();
                    break;
                case 3:
                    DealDamage(DamageValue);
                    DamageValue = HelperGlobals.Clamp(DamageValue - 1, 1, BattleGlobals.MaxDamage);

                    Bounces++;

                    if (Repeat == true && Bounces < BattleGlobals.MaxPowerBounces)
                    {
                        SequenceStep = 0;
                    }
                    break;
                case 4:
                    OnProgressBase();
                    break;
                default:
                    EndSequence();
                    break;
            }
        }
    }
}
