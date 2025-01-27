using NexusForever.Game.Static.Entity;
using NexusForever.GameTable.Static;

namespace NexusForever.GameTable.Model
{
    public class Item2Entry
    {
        public uint Id;
        public uint ItemBudgetId;
        public uint ItemStatId;
        public uint ItemRuneInstanceId;
        public uint ItemQualityId;
        public uint ItemSpecialId00;
        public uint ItemImbuementId;
        public uint Item2FamilyId;
        public uint Item2CategoryId;
        public uint Item2TypeId;
        public uint ItemDisplayId;
        public uint ItemSourceId;
        public uint ClassRequired;
        public uint RaceRequired;
        public uint Faction2IdRequired;
        public uint PowerLevel;
        public uint RequiredLevel;
        public uint RequiredItemLevel;
        public uint PrerequisiteId;
        public uint EquippedSlotFlags;
        public uint MaxStackCount;
        public uint MaxCharges;
        public uint ExpirationTimeMinutes;
        public uint Quest2IdActivation;
        public uint Quest2IdActivationRequired;
        public uint QuestObjectiveActivationRequired;
        public uint TradeskillAdditiveId;
        public uint TradeskillCatalystId;
        public uint HousingDecorInfoId;
        public uint HousingWarplotBossTokenId;
        public uint GenericUnlockSetId;
        public ItemFlags Flags;
        public uint BindFlags;
        public uint BuyFromVendorStackCount;
        [GameTableFieldArray(2)]
        public CurrencyType[] CurrencyTypeId;
        [GameTableFieldArray(2)]
        public uint[] CurrencyAmount;
        [GameTableFieldArray(2)]
        public CurrencyType[] CurrencyTypeIdSellToVendor;
        [GameTableFieldArray(2)]
        public uint[] CurrencyAmountSellToVendor;
        public uint ItemColorSetId;
        public float SupportPowerPercentage;
        public uint LocalizedTextIdName;
        public uint LocalizedTextIdTooltip;
        public string ButtonTemplate;
        public string ButtonIcon;
        public uint SoundEventIdEquip;
    }
}
