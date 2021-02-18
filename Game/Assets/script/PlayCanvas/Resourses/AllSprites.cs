using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllSprites : MonoBehaviour
{
    public Sprite cardback_Anemo;//风卡背
    public Sprite cardback_Geo;//岩卡背

    public Sprite normalCard_Attack;//攻击
    public Sprite normalCard_Dendro;//草
    public Sprite normalCard_Defence;//防御
    public Sprite normalCard_Anemo;//风
    public Sprite normalCard_Geo;//岩
    public Sprite normalCard_Heal;//治疗
    public Sprite normalCard_Move;//移动
    public Sprite normalCard_Burst;//爆发
    public Sprite normalCard_Fly;//飞行
    public Sprite normalCard_Pyro;//火
    public Sprite normalCard_Hydro;//水
    public Sprite normalCard_Electro;//电
    public Sprite normalCard_Cryo;//冰

    public Sprite monsterCard_Hilichurl;//丘丘人
    public Sprite monsterCard_Mitachurl;//丘丘暴徒
    public Sprite monsterCard_Slime;//史莱姆
    public Sprite monsterCard_Mage;//法师
    public Sprite monsterCard_Scout;//斥候

    public Sprite rewardCard_ChurlMask;//丘丘人面具
    public Sprite rewardCard_SlimeSecrations;//史莱姆清
    public Sprite rewardCard_Scroll;//卷轴
    public Sprite rewardCard_Insignia;//鸦印
    public Sprite rewardCard_Arrowhead;//箭头

    public Sprite itemCard_Clock;//怀表
    public Sprite itemCard_Tea;//茶
    public Sprite itemCard_Book;//笔记
    public Sprite itemCard_Trap;//捕兽夹
    public Sprite itemCard_Polearm;//长柄武器
    public Sprite itemCard_Sword;//单手武器
    public Sprite itemCard_Sunsettia;//日落果
    public Sprite itemCard_CrystalCore;//晶核
    public Sprite itemCard_Advice;//残破的笔记
    public Sprite itemCard_Teaport;//茶壶
    public Sprite itemCard_Bow;//弓
    public Sprite itemCard_Sigil;//百无禁忌录
    public Sprite itemCard_Ore;//矿石
    public Sprite itemCard_Chili;//辣椒
    public Sprite itemCard_Claymore;//大剑

    public Sprite player_He;//男主
    public Sprite player_She;//女主

    public Sprite characterCard_Diluc;
    public Sprite characterCard_Bennete;
    public Sprite characterCard_Tartaglia;//达达利亚
    public Sprite characterCard_Babara;
    public Sprite characterCard_Lisa;
    public Sprite characterCard_Fischl;//菲谢尔
    public Sprite characterCard_Diona;
    public Sprite characterCard_Noelle;//诺艾尔
    public Sprite characterCard_Qiqi;
    public Sprite characterCard_Keqing;
    public Sprite characterCard_Jean;//琴
    public Sprite characterCard_Mona;
    public Sprite characterCard_Zhongli;
    public Sprite characterCard_Keaya;//凯亚
    public Sprite characterCard_Klee;
    public Sprite characterCard_Venti;
    public Sprite characterCard_Ningguang;
    public Sprite characterCard_Amber;
    public Sprite characterCard_Sucroce;//砂糖
    public Sprite characterCard_Razor;
    public Sprite characterCard_Xingqiu;
    public Sprite characterCard_Chongyun;
    public Sprite characterCard_Beidou;
    public Sprite characterCard_Xiangling;

    public Sprite characterAvatar_Qiqi;
    public Sprite characterAvatar_Lisa;
    public Sprite characterAvatar_Ningguang;
    public Sprite characterAvatar_Keaya;
    public Sprite characterAvatar_Keqing;
    public Sprite characterAvatar_Beidou;
    public Sprite characterAvatar_Klee;
    public Sprite characterAvatar_Amber;
    public Sprite characterAvatar_Venti;
    public Sprite characterAvatar_Bennete;
    public Sprite characterAvatar_Jean;
    public Sprite characterAvatar_Sucroce;
    public Sprite characterAvatar_Babara;
    public Sprite characterAvatar_Mona;
    public Sprite characterAvatar_Fischl;
    public Sprite characterAvatar_Xingqiu;
    public Sprite characterAvatar_Noelle;
    public Sprite characterAvatar_Tartaglia;
    public Sprite characterAvatar_Diluc;
    public Sprite characterAvatar_Diona;
    public Sprite characterAvatar_Chongyun;
    public Sprite characterAvatar_Zhongli;
    public Sprite characterAvatar_Razor;
    public Sprite characterAvatar_Xiangling;

    public Sprite characterInfo_Noelle;
    public Sprite characterInfo_Diluc;
    public Sprite characterInfo_Lisa;
    public Sprite characterInfo_Ningguang;
    public Sprite characterInfo_Keaya;
    public Sprite characterInfo_Keqing;
    public Sprite characterInfo_Amber;
    public Sprite characterInfo_Jean;

    public Sprite Creator_Screen;//璇玑屏
    public Sprite Creator_Trap;
    public Sprite Creator_Sigil;
    public Sprite Creator_Rose;
    public Sprite Creator_Rabbit;

    public Sprite Bar_Red;
    public Sprite Bar_Blue;
    public Sprite Bar_Yellow;
    public Sprite Bar_Blue_Light;
    public Sprite Bar_Green;

    public Sprite Num_Red_Minus_1;
    public Sprite Num_Green_Add_1;
    public Sprite Num_Yellow_Add_1;

    public Sprite Attack_Ningguang_Normal;
    public Sprite Attack_Ningguang_Normal_Actived;

    public Sprite Elemental_Anemo;
    public Sprite Elemental_Geo;
    public Sprite Elemental_Dendro;
    public Sprite Elemental_Pyro;
    public Sprite Elemental_Cryo;
    public Sprite Elemental_Electro;
    public Sprite Elemental_Hydro;

    public Sprite Buff_Elemental_Anemo;
    public Sprite Buff_Elemental_Geo;
    public Sprite Buff_Elemental_Dendro;
    public Sprite Buff_Elemental_Pyro;
    public Sprite Buff_Elemental_Cryo;
    public Sprite Buff_Elemental_Electro;
    public Sprite Buff_Elemental_Hydro;

    public Sprite[] Statu = new Sprite[5];

    public Sprite Buff_Attack_Add;
    public Sprite Buff_Time_Reduce;
    public Sprite Buff_Cert_Hunt;
    public Sprite Buff_Cert_Rate;
    public Sprite Buff_Element_Absord;
    public Sprite Buff_Element_Hurt;
    public Sprite Buff_Electro;
    public Sprite Buff_Pyro;
    public Sprite Buff_Dendro;
    public Sprite Buff_Cryo;
    public Sprite Buff_Geo;
    public Sprite Buff_Hydro;
    public Sprite Buff_Anemo;
    public Sprite Buff_Defence_Add;
    public Sprite Buff_Element_Defence;
    public Sprite Buff_Electro_Defence;
    public Sprite Buff_Pyro_Defence;
    public Sprite Buff_Dendro_Defence;
    public Sprite Buff_Cryo_Defence;
    public Sprite Buff_Geo_Defence;
    public Sprite Buff_Hydro_Defence;
    public Sprite Buff_Anemo_Defence;
    public Sprite Buff_Move;
    public Sprite Buff_Sp_Add;
    public Sprite Buff_Sp_Reduce_Consume;
    public Sprite Buff_Hp_Add;
    public Sprite Buff_Hp_Add_All;
    public Sprite Buff_Revive;

}
