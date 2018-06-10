using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PaperMarioBattleSystem.Utilities;
using static PaperMarioBattleSystem.ActionCommandGlobals;
using static PaperMarioBattleSystem.SweetTreatGlobals;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Hold Left to ready a star and release to throw it. Hit the icons to heal HP and FP.
    /// Hitting a Poison Mushroom makes you unable to throw stars for a short time.
    /// </summary>
    public class SweetTreatCommand : ActionCommand
    {
        /// <summary>
        /// Handles spawning the icons.
        /// </summary>
        protected SweetTreatElementSpawner IconSpawner = null;

        protected bool StarReady = false;

        public Vector2 StartPosition { get; protected set; } = Vector2.Zero;
        protected Vector2 StarThrowVelocity = new Vector2(6f, 6f);

        public double CircleRadius { get; protected set; } = 60d;
        public double CursorAngle { get; protected set; } = 90d;

        protected double CursorMoveSpeed = 1d;
        public double CursorRotSpeed { get; protected set; } = .25d;

        protected const double MaxCursorAngle = -5d;
        protected const double MinCursorAngle = -75d;

        protected const double ThrowCooldown = 200d;
        protected const float StarMaxX = 800f;

        protected Keys ButtonToThrow = Keys.Left;

        protected double PrevThrow = 0d;

        public double ElapsedTime { get; protected set; } = 0d;

        /// <summary>
        /// The list of stars thrown during the action command.
        /// </summary>
        public readonly List<SweetTreatThrownStar> StarsThrown = new List<SweetTreatThrownStar>();

        /// <summary>
        /// The response sent to the Sequence.
        /// This is added to as the player hits more healing icons.
        /// </summary>
        public SweetTreatResponse HealingResponse = default(SweetTreatResponse);

        private BattleUIManager BUIManager = null;

        /// <summary>
        /// Tells if you can throw a star or not.
        /// </summary>
        protected bool CanThrowStar => (Time.ActiveMilliseconds >= PrevThrow);

        public SweetTreatCommand(IActionCommandHandler commandHandler, BattleUIManager bUIManager) : base(commandHandler)
        {
            BUIManager = bUIManager;
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            ElapsedTime = 0d;
            PrevThrow = 0d;

            StartPosition = Camera.Instance.SpriteToUIPos((Vector2)values[0]);

            //Count how many BattleEntities are affected
            //If there's only one, then don't add any of the Partner information
            int entitiesAffected = (int)values[1];

            CursorAngle = MinCursorAngle;

            //Define the spawner
            Vector2 startPos = new Vector2(500, 15);
            Vector2 endPos = new Vector2(startPos.X, BattleGlobals.PartnerPos.Y + 350f);

            RestoreTypes[] restoreTypes = null;
            int[] restoreTypeCounts = null;
            if (entitiesAffected <= 1)
            {
                restoreTypes = new RestoreTypes[] { RestoreTypes.MarioHP, RestoreTypes.FP, RestoreTypes.PoisonMushroom };
                restoreTypeCounts = new int[] { 14, 6, 2 };
            }
            else
            {
                restoreTypes = new RestoreTypes[] { RestoreTypes.MarioHP, RestoreTypes.PartnerHP, RestoreTypes.FP, RestoreTypes.PoisonMushroom };
                restoreTypeCounts = new int[] { 7, 7, 6, 2 };
            }

            IconSpawner = new SweetTreatElementSpawner(BUIManager, 4, 40f, 5000d, 750d, startPos, endPos, restoreTypes, restoreTypeCounts);

            HealingResponse = default(SweetTreatResponse);
        }

        public override void EndInput()
        {
            base.EndInput();

            ElapsedTime = 0d;

            //Clean up any remaining stars
            for (int i = 0; i < StarsThrown.Count; i++)
            {
                StarsThrown.RemoveAt(i);
                i--;
            }

            IconSpawner.CleanUp();
            IconSpawner = null;

            HealingResponse = default(SweetTreatResponse);

            BUIManager = null;
        }

        protected override void ReadInput()
        {
            ElapsedTime += Time.ElapsedMilliseconds;

            HandleStarThrow();

            UpdateCursor();
            UpdateThrownStars();
            IconSpawner.Update();

            //If the spawner is completely done, end the Action Command
            if (IconSpawner.CompletelyDone == true)
            {
                //Send the response with the current data set
                SendResponse(HealingResponse);

                OnComplete(CommandResults.Success);
            }
        }

        protected virtual void HandleStarThrow()
        {
            //Don't throw unless you can throw
            if (CanThrowStar == true)
            {
                //If a star isn't ready, check if the player held the button required to ready one
                if (StarReady == false)
                {
                    //Ready the star
                    if (Input.GetKey(ButtonToThrow) == true)
                    {
                        StarReady = true;
                    }
                }
                else
                {
                    //Check if the player released the button while a star was ready, then throw it
                    if (Input.GetKey(ButtonToThrow) == false)
                    {
                        StarReady = false;

                        CreateStar();
                    }
                }
            }
        }

        protected void CreateStar()
        {
            PrevThrow = Time.ActiveMilliseconds + ThrowCooldown;

            double angleRadians = UtilityGlobals.ToRadians(CursorAngle);

            //Throw the star and send it off with the current velocity
            Vector2 curVelocity = new Vector2(StarThrowVelocity.X * (float)Math.Cos(angleRadians), StarThrowVelocity.Y * (float)Math.Sin(angleRadians));

            SweetTreatThrownStar star = new SweetTreatThrownStar(BUIManager, StartPosition, curVelocity);

            StarsThrown.Add(star);
        }

        protected void UpdateCursor()
        {
            CursorAngle = UtilityGlobals.Clamp(CursorAngle + CursorMoveSpeed, MinCursorAngle, MaxCursorAngle);

            //If it reached its limits, reverse the angle
            if (CursorAngle >= MaxCursorAngle || CursorAngle <= MinCursorAngle)
            {
                CursorMoveSpeed = -CursorMoveSpeed;
            }
        }

        protected void UpdateThrownStars()
        {
            //Check if the stars collided with anything
            for (int i = 0; i < StarsThrown.Count; i++)
            {
                StarsThrown[i].Update();

                //Make sure the star doesn't go past the designated X value
                if (StarsThrown[i].Position.X >= StarMaxX)
                {
                    StarsThrown.RemoveAt(i);
                    i--;

                    continue;
                }

                CollisionResponseHolder? collisionResponse = UtilityGlobals.GetCollisionForSet(StarsThrown[i], IconSpawner.RestorationElements);

                //We have a collision, so handle it
                if (collisionResponse != null)
                {
                    //Remove the star and icon
                    IconSpawner.RemoveElement((SweetTreatRestorationElement)collisionResponse.Value.ResponseObj);

                    StarsThrown.RemoveAt(i);
                    i--;

                    //Handle the restore data based on its behavior
                    RestoreBehaviorData restoreData = (RestoreBehaviorData)collisionResponse.Value.CollisionData;
                    HandleRestoreData(restoreData);
                }
            }
        }

        protected void HandleRestoreData(RestoreBehaviorData restoreData)
        {
            int dataValue = restoreData.Value;
            
            //Handle the different behaviors
            switch (restoreData.Behavior)
            {
                case RestoreBehavior.HealMarioHP:
                    HealingResponse.MarioHPRestored += dataValue;
                    break;
                case RestoreBehavior.HealPartnerHP:
                    HealingResponse.PartnerHPRestored += dataValue;
                    break;
                case RestoreBehavior.HealFP:
                    HealingResponse.FPRestored += dataValue;
                    break;
                case RestoreBehavior.PreventInput:
                    PrevThrow = Time.ActiveMilliseconds + dataValue;
                    break;
                default:
                    break;
            }
        }
    }
}
