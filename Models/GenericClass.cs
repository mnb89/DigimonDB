using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigimonDB.Models
{
    public class CardBox
    {
        public string? Id { get; set; } 
        public bool ShouldSerializeId() { return !string.IsNullOrWhiteSpace(Id); }

        public string? Name { get; set; }
        public bool ShouldSerializeName() { return !string.IsNullOrWhiteSpace(Name); }

        public string? Tag { get; set; }
        public bool ShouldSerializeTag() { return !string.IsNullOrWhiteSpace(Tag); }

        public string? ImageUrl { get; set; }
        public bool ShouldSerializeImageUrl() { return !string.IsNullOrWhiteSpace(ImageUrl); }

        public string? ImagePath { get; set; }
        public bool ShouldSerializeImagePath() { return !string.IsNullOrWhiteSpace(ImagePath); }

        public string? TypeString { get; set; }
        public bool ShouldSerializeTypeString() { return !string.IsNullOrWhiteSpace(TypeString); }
        public bool ShouldSerializeType() { return false; }
        public CLTYPE Type { 

            get 
            {
                if (string.Compare(TypeString, "booster") == 0 || string.Compare(TypeString, "BT") == 0)
                    return CLTYPE.BOOSTER;
                else if (string.Compare(TypeString, "theme booster") == 0 || string.Compare(TypeString, "EX") == 0)
                    return CLTYPE.THEME_BOOSTER;
                else if (string.Compare(TypeString, "reboot booster") == 0 || string.Compare(TypeString, "RB") == 0)
                    return CLTYPE.REBOOT_BOOSTER;
                else if (string.Compare(TypeString, "starter deck") == 0 || string.Compare(TypeString, "ST") == 0)
                    return CLTYPE.STARTER_DECK;
                else if (string.Compare(TypeString, "advance deck") == 0)
                    return CLTYPE.ADVANCE_DECK;
                else
                    return CLTYPE.NONE;
            } 

            set 
            {
                if (value.Equals(CLTYPE.BOOSTER))
                    TypeString = "booster";
                else if (value.Equals(CLTYPE.THEME_BOOSTER))
                    TypeString = "theme booster";
                else if (value.Equals(CLTYPE.REBOOT_BOOSTER))
                    TypeString = "reboot booster";
                else if (value.Equals(CLTYPE.STARTER_DECK))
                    TypeString = "starter deck";
                else if (value.Equals(CLTYPE.ADVANCE_DECK))
                    TypeString = "advance deck";
                else
                    TypeString = null;
            } 
        }

        public List<Card> Cards { get; set;} 
        public bool ShouldSerializeCards() { return Cards.Count > 0; }

        internal void GenerateType()
        {
            if (Name != null && Tag != null)
            {
                if (Name.Contains("BOOSTER") && Tag.Contains("BT"))
                    Type = CLTYPE.BOOSTER;
                else if (Name.Contains("THEME BOOSTER") && Tag.Contains("EX"))
                    Type = CLTYPE.THEME_BOOSTER;
                else if (Name.Contains("REBOOT BOOSTER") && Tag.Contains("RB"))
                    Type = CLTYPE.REBOOT_BOOSTER;
                else if (Name.Contains("STARTER DECK") && Tag.Contains("ST"))
                    Type = CLTYPE.STARTER_DECK;
                else if (Name.Contains("ADVANCE DECK") && Tag.Contains("ST"))
                    Type = CLTYPE.ADVANCE_DECK;
                else
                    Type = CLTYPE.NONE;
            }
        }

        public Enum Status { get; set; }
        public bool ShouldSerializeStatus() { return false; }

        public CardBox()
        {
            Status = STATUS.NONE;
            Cards = new List<Card>();
        }
    }

    public class Card
    {

        public string? Code { get; set; }
        public bool ShouldSerializeCode() { return !string.IsNullOrWhiteSpace(Code); }
        public string? Name { get; set; }
        public bool ShouldSerializeName() { return !string.IsNullOrWhiteSpace(Name); }

        public string? Ctype { get; set; }
        public bool ShouldSerializeCtype() { return !string.IsNullOrWhiteSpace(Ctype); }

        public string? Color1 { get; set; }
        public bool ShouldSerializeColor1() { return !string.IsNullOrWhiteSpace(Color1); }

        public string? Color2 { get; set; }
        public bool ShouldSerializeColor2() { return !string.IsNullOrWhiteSpace(Color2); }

        public string? Rarity { get; set; }
        public bool ShouldSerializeRarity() { return !string.IsNullOrWhiteSpace(Rarity); }

        public string? Level { get; set; }
        public bool ShouldSerializeLevel() { return !string.IsNullOrWhiteSpace(Level); }

        public bool IsParallel { get; set; }

        public string BoxCode { get; set; }
        public bool ShouldSerializeBoxCode() { return !string.IsNullOrWhiteSpace(BoxCode); }

        public string BoxTag { get; set; }
        public bool ShouldSerializeBoxTag() { return !string.IsNullOrWhiteSpace(BoxTag); }

        public string? Form { get; set; }
        public bool ShouldSerializeForm() { return !string.IsNullOrWhiteSpace(Form); }

        public string? Attribute { get; set; }
        public bool ShouldSerializeAttribute() { return !string.IsNullOrWhiteSpace(Attribute); }

        public List<string> DigimonTypes { get; set; }
        public bool ShouldSerializeDigimonType() { return DigimonTypes.Count > 0; }
        
        public string? DP { get; set; }
        public bool ShouldSerializeDP() { return !string.IsNullOrWhiteSpace(DP); }

        public string? PlayCost { get; set; }
        public bool ShouldSerializePlayCost() { return !string.IsNullOrWhiteSpace(PlayCost); }

        public string? DigivolveCost1 { get; set; }
        public bool ShouldSerializeDigivolveCost1() { return !string.IsNullOrWhiteSpace(DigivolveCost1); }

        public string? DigivolveCost2 { get; set; }
        public bool ShouldSerializeDigivolveCost2() { return !string.IsNullOrWhiteSpace(DigivolveCost2); }
        public string? Effect { get; set; }
        public bool ShouldSerializeEffect() { return !string.IsNullOrWhiteSpace(Effect); }
        public string? DigivolveEffect { get; set; }
        public bool ShouldSerializeDigivolveEffect() { return !string.IsNullOrWhiteSpace(DigivolveEffect); }

        public string? SecurityEffect { get; set; }
        public bool ShouldSerializeSecurityEffect() { return !string.IsNullOrWhiteSpace(SecurityEffect); }

        public List<GenericImage> Imgs { get; set; }

        public bool ShouldSerializeImgs() { return Imgs.Count > 0; }


        public Card()
        {
            IsParallel = false;
            DigimonTypes = new List<string>();
            Imgs = new List<GenericImage>();
        }

    }

    public class GenericValue
    {
        public long Id { get; set; }
        public bool ShouldSerializeId() { return Id > 0 ; }

        public string? Name { get; set; }
        public bool ShouldSerializeName() { return !string.IsNullOrWhiteSpace(Name); }

        public string? Value { get; set; }
        public bool ShouldSerializeValue() { return !string.IsNullOrWhiteSpace(Value); }
    }

    public class GenericImage
    {
        public long Id { get; set; }
        public bool ShouldSerializeId() { return Id > 0; }

        public string? Code { get; set; }
        public bool ShouldSerializeCode() { return !string.IsNullOrWhiteSpace(Code); }

        public string? IsParallel { get; set; }
        public bool ShouldSerializeIsParallel() { return !string.IsNullOrWhiteSpace(IsParallel); }

        public string? Rarity { get; set; }
        public bool ShouldSerializeRarity() { return !string.IsNullOrWhiteSpace(Rarity); }

        public long BoxId { get; set; }
        public bool ShouldSerializeBoxId() { return BoxId > 0; ; }

        public string? ImgUrl { get; set; }
        public bool ShouldSerializeImgUrl() { return !string.IsNullOrWhiteSpace(ImgUrl); }

        public string? ImgPath { get; set; }
        public bool ShouldSerializeImgPath() { return !string.IsNullOrWhiteSpace(ImgPath); }
    }

}
