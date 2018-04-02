using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// An object that manages rendering HP bars for BattleEntities with a <see cref="Enumerations.AdditionalProperty.ShowHP"/> property.
    /// </summary>
    public sealed class HPBarManagerObj : EntityListenerObj
    {
        /// <summary>
        /// The list of BattleEntities that have their HP shown.
        /// </summary>
        private List<BattleEntity> HPShownEntities = new List<BattleEntity>();

        /// <summary>
        /// The list of BattleEntities that don't have their HP shown.
        /// </summary>
        private List<BattleEntity> NoHPEntities = new List<BattleEntity>();

        private CroppedTexture2D HPBar = null;
        private CroppedTexture2D HPBarFill = null;

        private Vector2 HPBarOffset = new Vector2(-5, 40);
        private Vector2 FillOffset = new Vector2(2, 2);
        private float HPBarWidth = 33f;
        private Vector2 HPTextOffset = Vector2.Zero;

        public HPBarManagerObj()
        {
            Initialize();

            //Subscribe to the events
            ListenToEntityEvents();

            BattleManager.Instance.BattleTurnEndedEvent -= OnBattleTurnEnded;
            BattleManager.Instance.BattleTurnEndedEvent += OnBattleTurnEnded;
        }

        public override void CleanUp()
        {
            HPShownEntities.Clear();
            NoHPEntities.Clear();

            HPBar = null;
            HPBarFill = null;

            base.CleanUp();

            if (BattleManager.HasInstance == true)
            {
                BattleManager.Instance.BattleTurnEndedEvent -= OnBattleTurnEnded;
            }
        }

        private void Initialize()
        {
            //Add all existing entities
            BattleManager.Instance.GetAllBattleEntities(NoHPEntities, null);

            CheckShowEntityHP();

            Texture2D battleGFX = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.BattleGFX}.png");

            HPBar = new CroppedTexture2D(battleGFX, new Rectangle(513, 275, 38, 13));
            HPBarFill = new CroppedTexture2D(battleGFX, new Rectangle(516, 292, 1, 8));

            HPTextOffset = HPBar.WidthHeightToVector2();
        }

        /// <summary>
        /// Updates the list of BattleEntities that have their HP shown and now should have it hidden.
        /// It looks through the list of BattleEntities who have their HP shown and adds them to the hidden list if they should not show their HP anymore.
        /// </summary>
        private void CheckHideEntityHP()
        {
            for (int i = 0; i < HPShownEntities.Count; i++)
            {
                //Check if the entities should no longer have their HP shown
                if (HPShownEntities[i].EntityProperties.HasAdditionalProperty(Enumerations.AdditionalProperty.ShowHP) == false)
                {
                    //Add them to the not shown list
                    NoHPEntities.Add(HPShownEntities[i]);

                    //Remove them from the shown list
                    HPShownEntities.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Updates the list of BattleEntities that should have their HP shown.
        /// It looks through the list of BattleEntities who don't have their HP shown and adds them to the HP shown list if they should now show their HP.
        /// </summary>
        private void CheckShowEntityHP()
        {
            for (int i = 0; i < NoHPEntities.Count; i++)
            {
                //Check if the entities should now have their HP shown
                if (NoHPEntities[i].EntityProperties.HasAdditionalProperty(Enumerations.AdditionalProperty.ShowHP) == true)
                {
                    //Add them to the HP shown list
                    HPShownEntities.Add(NoHPEntities[i]);

                    //Remove them from the not shown list
                    NoHPEntities.RemoveAt(i);
                    i--;
                }
            }
        }

        public override void Draw()
        {
            //Don't render the HP bars if the UI shouldn't show up
            if (BattleManager.Instance.ShouldShowPlayerTurnUI == false) return;

            //Render the HP bars
            for (int i = 0; i < HPShownEntities.Count; i++)
            {
                BattleEntity entity = HPShownEntities[i];
                float hpRatio = entity.CurHP / (float)entity.BattleStats.MaxHP;
                Vector2 fillScale = new Vector2((int)(HPBarWidth * hpRatio), 1f);

                Vector2 hpBarPos = entity.Position + HPBarOffset;

                SpriteRenderer.Instance.Draw(HPBar.Tex, hpBarPos, HPBar.SourceRect, Color.White, false, false, .2f);
                SpriteRenderer.Instance.Draw(HPBarFill.Tex, hpBarPos + FillOffset, HPBarFill.SourceRect, Color.White, 0f, Vector2.Zero, fillScale, false, false, .21f);

                SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, entity.CurHP.ToString(), hpBarPos + HPTextOffset, Color.White, 0f, new Vector2(1f, 0f), 1f, .2f);
            }
        }

        protected override void EntityAdded(BattleEntity battleEntity)
        {
            //Check if the entity has the ShowHP property and add it to the HP shown list
            if (battleEntity.EntityProperties.HasAdditionalProperty(Enumerations.AdditionalProperty.ShowHP) == true)
            {
                HPShownEntities.Add(battleEntity);
            }
            //Otherwise add it to the no HP shown list
            else
            {
                NoHPEntities.Add(battleEntity);
            }
        }

        protected override void EntityRemoved(BattleEntity battleEntity)
        {
            //Remove the entity
            NoHPEntities.Remove(battleEntity);
            HPShownEntities.Remove(battleEntity);
        }

        private void OnBattleTurnEnded(BattleEntity battleEntity)
        {
            //Update the HP shown list on turn end
            CheckHideEntityHP();
            CheckShowEntityHP();
        }
    }
}
