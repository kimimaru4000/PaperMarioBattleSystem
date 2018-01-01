using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static PaperMarioBattleSystem.Enumerations;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// The Shell in Koops' Shell Shield move.
    /// </summary>
    public sealed class Shell : BattleEntity
    {
        /// <summary>
        /// The BattleEntity the Shell is defending.
        /// </summary>
        private BattleEntity EntityDefending = null;

        private bool SentDeathBattleEvent = false;

        public Shell(int hp, int maxHP) : base(new Stats(0, maxHP, 0, 0, 0))
        {
            Name = "Shell";

            EntityType = EntityTypes.Neutral;

            //Set the HP to the amount of HP the Shell should have
            int hpDiff = BattleStats.MaxHP - hp;
            if (hpDiff > 0)
            {
                LoseHP(hpDiff);
            }
            
            //The Shell doesn't take turns
            BaseTurns = -99;

            //The Shell isn't targetable by normal means. It makes all attacks target it instead of the normal BattleEntity when active
            this.AddIntAdditionalProperty(AdditionalProperty.Untargetable, 1);

            AddStatusImmunities();

            Texture2D spriteSheet = AssetManager.Instance.LoadRawTexture2D($"{ContentGlobals.SpriteRoot}/Neutral/ShellShieldShell.png");
            AnimManager.SetSpriteSheet(spriteSheet);

            AnimManager.AddAnimation(AnimationGlobals.IdleName, new Animation(spriteSheet,
                new Animation.Frame(new Rectangle(7, 3, 186, 130), 1000d)));

            Scale = new Vector2(.5f, .5f);

            //Subscribe to the removed event so we can remove the protection and clear the entity reference if it's removed
            BattleManager.Instance.EntityRemovedEvent -= EntityRemoved;
            BattleManager.Instance.EntityRemovedEvent += EntityRemoved;

            this.AddShowHPProperty();
        }

        public override void CleanUp()
        {
            BattleManager.Instance.EntityRemovedEvent -= EntityRemoved;

            RemoveEntityDefending();

            base.CleanUp();
        }

        /// <summary>
        /// Clears the BattleEntity the Shell is defending.
        /// </summary>
        public void RemoveEntityDefending()
        {
            if (EntityDefending != null)
            {
                Debug.Log($"The Shell stopped defending {EntityDefending.Name}!");

                EntityDefending.EntityProperties.RemoveAdditionalProperty(AdditionalProperty.DefendedByEntity);
                EntityDefending = null;
            }
        }

        /// <summary>
        /// Sets the BattleEntity for the Shell to defend.
        /// </summary>
        /// <param name="battleEntity">The BattleEntity for the Shell to defend.</param>
        public void SetEntityToDefend(BattleEntity battleEntity)
        {
            RemoveEntityDefending();

            if (battleEntity == null) return;

            EntityDefending = battleEntity;
            EntityDefending.EntityProperties.AddAdditionalProperty(AdditionalProperty.DefendedByEntity, this);

            Debug.Log($"The Shell started defending {EntityDefending.Name}!");
        }

        public override void OnTurnStart()
        {
            //In the event the Shell does start its turn, simply do nothing
            base.OnTurnStart();

            StartAction(new NoAction(), false, null);
        }

        protected override void OnDeath()
        {
            base.OnDeath();

            //Add a Battle Event to end the protection and play the animation of the Shell breaking at the end of the turn
            if (SentDeathBattleEvent == false)
            {
                BattleEventManager.Instance.QueueBattleEvent((int)BattleGlobals.StartEventPriorities.Status - 1,
                    new BattleManager.BattleState[] { BattleManager.BattleState.TurnEnd },
                    new ShellBreakBattleEvent(this));

                SentDeathBattleEvent = true;
            }
        }

        public override int GetEquippedBadgeCount(BadgeGlobals.BadgeTypes badgeType)
        {
            return 0;
        }

        public override Item GetItemOfType(Item.ItemTypes itemTypes)
        {
            return null;
        }

        private void AddStatusImmunities()
        {
            //The Shell is immune to all Status Effects
            StatusTypes[] statuses = UtilityGlobals.GetEnumValues<StatusTypes>();
            for (int i = 0; i < statuses.Length; i++)
            {
                this.AddRemoveStatusImmunity(statuses[i], true);
            }
        }

        public override void Draw()
        {
            base.Draw();

            //Show the Shell's HP, which can only be viewed with the Peekaboo Badge since it can't be tattled (in the actual games, at least)
            //NOTE: Have a way to add the ShowHP property on anything with Peekaboo, not only enemies (maybe through an interface?)
            if (BattleManager.Instance.ShouldShowPlayerTurnUI == true)
            {
                int showHP = EntityProperties.GetAdditionalProperty<int>(Enumerations.AdditionalProperty.ShowHP);

                if (showHP > 0)
                {
                    //Show HP
                    SpriteRenderer.Instance.DrawText(AssetManager.Instance.TTYDFont, $"{CurHP}/{BattleStats.MaxHP}", Position + new Vector2(0, 40), Color.White, .2f, false);
                }
            }
        }

        private void EntityRemoved(BattleEntity entityRemoved)
        {
            //Remove the BattleEntity the Shell is defending and kill the Shell
            if (EntityDefending != null && entityRemoved == EntityDefending)
            {
                RemoveEntityDefending();

                //Unsubscribe from this event
                BattleManager.Instance.EntityRemovedEvent -= EntityRemoved;

                //Kill the Shell if it's not already dead
                if (IsDead == false)
                    Die();
            }
        }
    }
}
