using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        protected Vector2 StartPosition = Vector2.Zero;
        protected Vector2 StarThrowVelocity = new Vector2(6f, 6f);

        protected double CircleRadius = 60d;
        protected double CursorAngle = 90d;

        protected double CursorMoveSpeed = 1d;
        protected double CursorRotSpeed = .25d;

        protected const double MaxCursorAngle = -5d;
        protected const double MinCursorAngle = -75d;

        protected const double ThrowCooldown = 200d;
        protected const float StarMaxX = 800f;

        protected Keys ButtonToThrow = Keys.Left;

        protected double PrevThrow = 0d;

        protected double ElapsedTime = 0d;

        protected UIFourPiecedTex Cursor = null;

        protected UICroppedTexture2D MarioHPIcon = null;
        protected UICroppedTexture2D PartnerHPIcon = null;
        protected UICroppedTexture2D FPIcon = null;
        protected UIText MarioHPText = null;
        protected UIText PartnerHPText = null;
        protected UIText FPText = null;

        /// <summary>
        /// The list of stars thrown during the action command.
        /// </summary>
        protected List<SweetTreatThrownStar> StarsThrown = new List<SweetTreatThrownStar>();

        /// <summary>
        /// The response sent to the Sequence.
        /// This is added to as the player hits more healing icons.
        /// </summary>
        protected SweetTreatResponse HealingResponse = default(SweetTreatResponse);

        /// <summary>
        /// Tells if you can throw a star or not.
        /// </summary>
        protected bool CanThrowStar => (Time.ActiveMilliseconds >= PrevThrow);

        public SweetTreatCommand(IActionCommandHandler commandHandler) : base(commandHandler)
        {
            
        }

        public override void StartInput(params object[] values)
        {
            base.StartInput(values);

            ElapsedTime = 0d;
            PrevThrow = 0d;

            StartPosition = Camera.Instance.SpriteToUIPos((Vector2)values[0]);

            CursorAngle = MinCursorAngle;

            //Define the UI to display
            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            CroppedTexture2D croppedTex2D = new CroppedTexture2D(battleGFX, new Rectangle(14, 273, 46, 46));
            Cursor = new UIFourPiecedTex(croppedTex2D, croppedTex2D.WidthHeightToVector2(), .5f, Color.White);

            MarioHPIcon = new UICroppedTexture2D(new CroppedTexture2D(battleGFX, new Rectangle(324, 407, 61, 58)));
            PartnerHPIcon = new UICroppedTexture2D(new CroppedTexture2D(battleGFX, new Rectangle(324, 407, 61, 58)));
            FPIcon = new UICroppedTexture2D(new CroppedTexture2D(battleGFX, new Rectangle(179, 416, 40, 39)));

            MarioHPText = new UIText("0", Color.Black);
            PartnerHPText = new UIText("0", Color.Black);
            FPText = new UIText("0", Color.Black);

            //Set UI properties
            MarioHPIcon.Position = MarioHPText.Position = StartPosition + new Vector2(-10, -45);
            PartnerHPIcon.Position = PartnerHPText.Position = StartPosition + new Vector2(-80, -45);
            FPIcon.Position = FPText.Position = StartPosition + new Vector2(-45, -95);

            MarioHPText.Position += new Vector2(0f, 10f);
            PartnerHPText.Position += new Vector2(0f, 10f);
            FPText.Position += new Vector2(0f, 10f);

            MarioHPIcon.Depth = PartnerHPIcon.Depth = FPIcon.Depth = .6f;
            MarioHPText.Depth = PartnerHPText.Depth = FPText.Depth = .61f;

            MarioHPIcon.Origin = PartnerHPIcon.Origin = FPIcon.Origin = MarioHPText.Origin = PartnerHPText.Origin = FPText.Origin = new Vector2(.5f, .5f);


            //Set cursor position
            Cursor.Position = UtilityGlobals.GetPointAroundCircle(new Circle(StartPosition, CircleRadius), CursorAngle, true);

            //Define the spawner
            Vector2 startPos = new Vector2(500, 15);
            Vector2 endPos = new Vector2(startPos.X, BattleManager.Instance.PartnerPos.Y + 350f);
            IconSpawner = new SweetTreatElementSpawner(4, 40f, 5000d, 750d, startPos, endPos, new RestoreTypes[]
            {
                RestoreTypes.MarioHP, RestoreTypes.PartnerHP, RestoreTypes.FP, RestoreTypes.PoisonMushroom
            }, new int[] { 7, 7, 6, 2 });

            //Add the cursor and other UI elements
            BattleUIManager.Instance.AddUIElement(Cursor);
            BattleUIManager.Instance.AddUIElement(MarioHPIcon);
            BattleUIManager.Instance.AddUIElement(PartnerHPIcon);
            BattleUIManager.Instance.AddUIElement(FPIcon);
            BattleUIManager.Instance.AddUIElement(MarioHPText);
            BattleUIManager.Instance.AddUIElement(PartnerHPText);
            BattleUIManager.Instance.AddUIElement(FPText);

            HealingResponse = default(SweetTreatResponse);
        }

        public override void EndInput()
        {
            base.EndInput();

            ElapsedTime = 0d;

            //Clean up any remaining stars
            for (int i = 0; i < StarsThrown.Count; i++)
            {
                BattleUIManager.Instance.RemoveUIElement(StarsThrown[i]);
                StarsThrown.RemoveAt(i);
                i--;
            }

            //Remove any UI we added
            BattleUIManager.Instance.RemoveUIElement(Cursor);
            BattleUIManager.Instance.RemoveUIElement(MarioHPIcon);
            BattleUIManager.Instance.RemoveUIElement(PartnerHPIcon);
            BattleUIManager.Instance.RemoveUIElement(FPIcon);
            BattleUIManager.Instance.RemoveUIElement(MarioHPText);
            BattleUIManager.Instance.RemoveUIElement(PartnerHPText);
            BattleUIManager.Instance.RemoveUIElement(FPText);

            Cursor = null;
            MarioHPIcon = null;
            PartnerHPIcon = null;
            FPIcon = null;
            MarioHPText = null;
            PartnerHPText = null;
            FPText = null;

            IconSpawner.CleanUp();
            IconSpawner = null;

            HealingResponse = default(SweetTreatResponse);
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

            SweetTreatThrownStar star = new SweetTreatThrownStar(StartPosition, curVelocity);

            StarsThrown.Add(star);
            BattleUIManager.Instance.AddUIElement(star);
        }

        protected void UpdateCursor()
        {
            CursorAngle = UtilityGlobals.Clamp(CursorAngle + CursorMoveSpeed, MinCursorAngle, MaxCursorAngle);

            //If it reached its limits, reverse the angle
            if (CursorAngle >= MaxCursorAngle || CursorAngle <= MinCursorAngle)
            {
                CursorMoveSpeed = -CursorMoveSpeed;
            }

            Cursor.Position = UtilityGlobals.GetPointAroundCircle(new Circle(StartPosition, CircleRadius), CursorAngle, true);
            Cursor.Rotation = (float)(-ElapsedTime * UtilityGlobals.ToRadians(CursorRotSpeed));
        }

        protected void UpdateThrownStars()
        {
            //Check if the stars collided with anything
            for (int i = 0; i < StarsThrown.Count; i++)
            {
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

                    BattleUIManager.Instance.RemoveUIElement(StarsThrown[i]);
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

            MarioHPText.Text = HealingResponse.MarioHPRestored.ToString();
            PartnerHPText.Text = HealingResponse.PartnerHPRestored.ToString();
            FPText.Text = HealingResponse.FPRestored.ToString();
        }
    }
}
