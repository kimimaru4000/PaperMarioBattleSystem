using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaperMarioBattleSystem
{
    /// <summary>
    /// Mario's partners in battle
    /// </summary>
    public abstract class BattlePartner : BattlePlayer
    {
        public Enumerations.PartnerTypes PartnerType { get; protected set; } = Enumerations.PartnerTypes.None;

        public BattlePartner(Stats stats) : base(stats)
        {
            Name = "Partner";

            EntityType = Enumerations.EntityTypes.Player;
        }

        //public void EquipAllBadges()
        //{
        //    List<Badge> badges = Inventory.Instance.GetActivePartnerBadges();
        //    for (int i = 0; i < badges.Count; i++)
        //    {
        //        badges[i].Equip(this);
        //    }
        //}

        //public void UnEquipAllBadges()
        //{
        //    List<Badge> badges = Inventory.Instance.GetActivePartnerBadges();
        //    for (int i = 0; i < badges.Count; i++)
        //    {
        //        badges[i].UnEquip();
        //    }
        //}
    }
}
