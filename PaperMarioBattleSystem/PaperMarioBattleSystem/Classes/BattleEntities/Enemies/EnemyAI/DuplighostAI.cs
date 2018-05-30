using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Duplighost enemy AI.
    /// </summary>
    public sealed class DuplighostAI : EnemyAIBehavior
    {
        private Duplighost duplighost = null;

        public DuplighostAI(Duplighost dupliGhost) : base(dupliGhost)
        {
            duplighost = dupliGhost;
        }

        public override void PerformAction()
        {
            //If it's flipped, don't do anything
            if (duplighost.FlippableBehavior != null && duplighost.FlippableBehavior.Flipped == true)
            {
                duplighost.StartAction(new NoAction(duplighost), true, null);
                return;
            }

            if (duplighost.PartnerTypeDisguise == PartnerTypes.None)
            {
                //For testing, say that it's a 50% chance of disguising and headbutting
                int randVal = GeneralGlobals.Randomizer.Next(0, 2);

                //Ensure there's a Partner to copy
                bool partnerExists = (BattleManager.Instance.Partner != null);

                if (randVal == 0 && partnerExists == true)
                {
                    duplighost.StartAction(new DisguiseAction(duplighost), false, BattleManager.Instance.Partner.GetTrueTarget());
                }
                else
                {
                    duplighost.StartAction(new HeadbuttAction(duplighost), false, BattleManager.Instance.FrontPlayer.GetTrueTarget());
                }
            }
            else
            {
                if (duplighost.PartnerTypeDisguise == PartnerTypes.Goombario)
                {
                    int rand = GeneralGlobals.Randomizer.Next(0, 2);

                    if (rand == 0)
                        duplighost.StartAction(new BonkAction(duplighost), false, BattleManager.Instance.FrontPlayer.GetTrueTarget());
                    else
                        duplighost.StartAction(new TattleAction(duplighost, false), false, BattleManager.Instance.Mario);
                }
                else if (duplighost.PartnerTypeDisguise == PartnerTypes.Kooper)
                {
                    duplighost.StartAction(new ShellTossAction(duplighost), false, BattleManager.Instance.FrontPlayer.GetTrueTarget());
                }
                else if (duplighost.PartnerTypeDisguise == PartnerTypes.Watt)
                {
                    duplighost.StartAction(new ElectroDashAction(duplighost), false, BattleManager.Instance.FrontPlayer.GetTrueTarget());
                }
                else
                {
                    duplighost.StartAction(new HeadbuttAction(duplighost), false, BattleManager.Instance.FrontPlayer.GetTrueTarget());
                }
            }

            //Duplighosts Auto-complete Action Commands
            if (duplighost.PreviousAction != null && duplighost.PreviousAction.HasActionCommand == true)
            {
                duplighost.PreviousAction.DrawActionCommandInfo = false;
                duplighost.PreviousAction.EnableActionCommand = true;
                duplighost.PreviousAction.actionCommand.AutoComplete = true;
            }
        }
    }
}
