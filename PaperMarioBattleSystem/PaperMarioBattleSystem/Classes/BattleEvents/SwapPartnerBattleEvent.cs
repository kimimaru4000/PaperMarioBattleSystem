using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using PaperMarioBattleSystem.Utilities;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// A BattleEvent that swaps out Mario's current Partner with a different one.
    /// </summary>
    public sealed class SwapPartnerBattleEvent : BattleEvent
    {
        private enum SwapStates
        {
            Entering, Wait, Exiting
        }

        private BattlePartner OldPartner = null;
        private BattlePartner NewPartner = null;

        private SwapStates State = SwapStates.Entering;

        Vector2 StartPos = Vector2.Zero;
        Vector2 EndPos = Vector2.Zero;

        private double MoveTime = 300d;
        private double WaitTime = 300d;
        private double ElapsedTime = 0d;

        public SwapPartnerBattleEvent(BattlePartner oldPartner, BattlePartner newPartner, double moveTime, double waitTime)
        {
            OldPartner = oldPartner;
            NewPartner = newPartner;
            MoveTime = moveTime;
            WaitTime = waitTime;
        }

        protected override void OnStart()
        {
            StartPos = OldPartner.BattlePosition;
            EndPos = OldPartner.BManager.Mario.BattlePosition;
        }

        protected override void OnEnd()
        {
            ElapsedTime = 0d;

            OldPartner = null;
            NewPartner = null;
            StartPos = EndPos = Vector2.Zero;
        }

        protected override void OnUpdate()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            if (State == SwapStates.Entering)
            {
                OldPartner.Position = Interpolation.Interpolate(StartPos, EndPos, ElapsedTime / MoveTime, Interpolation.InterpolationTypes.Linear);

                if (ElapsedTime >= MoveTime)
                {
                    ElapsedTime = 0d;
                    State = SwapStates.Wait;

                    SwapPartner();
                }
            }
            else if (State == SwapStates.Wait)
            {
                if (ElapsedTime >= WaitTime)
                {
                    ElapsedTime = 0d;

                    State = SwapStates.Exiting;

                    StartPos = EndPos;
                    EndPos = NewPartner.BattlePosition;
                }
            }
            else
            {
                NewPartner.Position = Interpolation.Interpolate(StartPos, EndPos, ElapsedTime / MoveTime, Interpolation.InterpolationTypes.Linear);

                if (ElapsedTime >= MoveTime)
                {
                    End();
                }
            }
        }

        /// <summary>
        /// Handles swapping Partners.
        /// </summary>
        private void SwapPartner()
        {
            //Remove the old partner from battle, set the new Partner reference, then add the new Partner to battle
            //This order ensures that the Partner reference is correct for the EntityRemovedEvent and the EntityAddedEvent
            OldPartner.BManager.RemoveEntity(OldPartner, false);

            OldPartner.BManager.SetPartner(NewPartner);

            OldPartner.BManager.AddEntity(NewPartner, null, false);

            //Position offset
            Vector2 offset = Vector2.Zero;

            //If the old Partner was airborne and the new one isn't, move the new one down
            if (OldPartner.HeightState == HeightStates.Airborne && NewPartner.HeightState != HeightStates.Airborne)
            {
                offset.Y += BattleGlobals.AirborneY;
            }
            //Otherwise, if the old Partner wasn't airborne and the new one is, move the new one up
            else if (OldPartner.HeightState != HeightStates.Airborne && NewPartner.HeightState == HeightStates.Airborne)
            {
                offset.Y -= BattleGlobals.AirborneY;
            }

            //Set positions to the old ones
            NewPartner.Position = OldPartner.Position;
            NewPartner.SetBattleIndex(OldPartner.BattleIndex);
            NewPartner.SetBattlePosition(OldPartner.BattlePosition + offset);

            //State the old Partner is out of battle
            OldPartner.SetBattleIndex(BattleGlobals.InvalidBattleIndex);

            //Set flip state
            NewPartner.SpriteFlip = OldPartner.SpriteFlip;

            //Set the new Partner to use the same max number of turns all Partners have this phase cycle
            //The only exceptions are if the new partner doesn't move at all (Ex. Goompa) or is immobile
            //In this case, set its max turn count to 0
            if (NewPartner.BaseTurns > 0 && NewPartner.IsImmobile() == false)
            {
                NewPartner.SetMaxTurns(BattlePartner.PartnerMaxTurns);
            }
            else
            {
                NewPartner.SetMaxTurns(0);
            }

            //If the BattleEntity swapping out partners is the old one, increment the turn count for the new Partner,
            //as the old one's turn count will be incremented after the action is finished
            if (OldPartner.IsTurn == true)
            {
                NewPartner.SetTurnsUsed(OldPartner.TurnsUsed + 1);
            }
            //Otherwise, the entity swapping out partners must be Mario, so set the new Partner's turn count to the old one's
            //(or an enemy via an attack, but none of those attacks exist in the PM games...I'm hinting at a new attack idea :P)
            else
            {
                NewPartner.SetTurnsUsed(OldPartner.TurnsUsed);
            }

            //Swap Partner badges with the new Partner
            BattlePartner.SwapPartnerBadges(OldPartner, NewPartner);
        }
    }
}
