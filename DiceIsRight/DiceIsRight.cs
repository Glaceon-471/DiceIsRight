using Spine;
using Spine.Unity;
using System.Collections.Generic;

namespace DiceIsRight;

public class DiceIsRightScript : CreatureBase
{
    public DiceIsRightAnim Anim;
    public UseSkill LastUseSkill { get; private set; }

    public override void OnViewInit(CreatureUnit unit)
    {
        base.OnViewInit(unit);
        Anim = (DiceIsRightAnim)unit.animTarget;
        Anim.SetScript(this);
    }

    public override void OnEnterRoom(UseSkill skill)
    {
        base.OnEnterRoom(skill);
        if (EquipmentTypeInfo.GetLcId(skill.agent.Equipment.weapon.metaInfo) == 201981)
            return;
        LastUseSkill = skill;
        Anim.RollAnim();
    }

    public void RollEffect(UseSkill skill, int value)
    {
        RwbpType type = skill.skillTypeInfo.rwbpType;
        AgentModel worker = skill.agent;
        int success = SuccessValue(worker, type);
        List<AgentModel> target_agent = [];
        List<CreatureModel> target_creature = [];
        foreach (AgentModel agent in AgentManager.instance.GetAgentList())
        {
            if (agent.IsAttackTargetable() && !agent.IsDead() && !agent.IsPanic() && !agent.IsCrazy() && agent.unconAction == null)
                target_agent.Add(agent);
        }
        foreach (CreatureModel creature in CreatureManager.instance.GetCreatureList())
        {
            if (creature.IsAttackTargetable() && creature.IsEscapedOnlyEscape())
                target_creature.Add(creature);
        }
        if (value == 100)
        {
            switch (type)
            {
                case RwbpType.R:
                    foreach (AgentModel agent in target_agent)
                        agent.TakeDamage(new DamageInfo(RwbpType.R, (int)(agent.maxHp / 4f), (int)(agent.maxHp / 2f)));
                    break;
                case RwbpType.W:
                    foreach (AgentModel agent in target_agent)
                        agent.TakeDamage(new DamageInfo(RwbpType.W, (int)(agent.maxMental / 4f), (int)(agent.maxMental / 2f)));
                    break;
                case RwbpType.B:
                    foreach (AgentModel agent in target_agent)
                        agent.AddUnitBuf(new DamageFactorBuf(Data.DamageFactorBufValue));
                    foreach (CreatureModel creature in target_creature)
                        creature.AddUnitBuf(new DamageFactorBuf(Data.DamageFactorDebufValue));
                    break;
                case RwbpType.P:
                    foreach (AgentModel agent in target_agent)
                        worker.Equipment.weapon.OnAttack(worker, agent);
                    worker.tempAnim.PlayAttackAnimation(worker.OnEndAttackCycle);
                    break;
            }
        }
        else if (value >= 96)
        {
            switch (type)
            {
                case RwbpType.R:
                    foreach (AgentModel agent in target_agent)
                        agent.TakeDamage(new DamageInfo(RwbpType.R, (int)(agent.maxHp / 10f), (int)(agent.maxHp * 2 / 10f)));
                    break;
                case RwbpType.W:
                    foreach (AgentModel agent in target_agent)
                        agent.TakeDamage(new DamageInfo(RwbpType.W, (int)(agent.maxMental / 10f), (int)(agent.maxMental * 2 / 10f)));
                    break;
                case RwbpType.B:
                    foreach (CreatureModel creature in target_creature)
                        creature.AddUnitBuf(new DamageFactorBuf(Data.DamageFactorDebufValue));
                    break;
                case RwbpType.P:
                    worker.Equipment.weapon.OnAttack(worker, worker);
                    worker.tempAnim.PlayAttackAnimation(worker.OnEndAttackCycle);
                    break;
            }
        }
        else if (value <= success)
        {
            if (value == 1)
            {
                switch (type)
                {
                    case RwbpType.R:
                        foreach (AgentModel agent in target_agent)
                            agent.RecoverHP(agent.maxHp);
                        break;
                    case RwbpType.W:
                        foreach (AgentModel agent in target_agent)
                            agent.RecoverMental(agent.maxMental);
                        break;
                    case RwbpType.B:
                        foreach (CreatureModel creature in target_creature)
                            creature.AddUnitBuf(new DamageFactorBuf(Data.DamageFactorBufValue * 10));
                        break;
                    case RwbpType.P:
                        foreach (CreatureModel creature in target_creature)
                        {
                            creature.AddUnitBuf(new DamageFactorBuf(Data.DamageFactorBufValue * 5, 1));
                            worker.Equipment.weapon.OnAttack(worker, creature);
                        }
                        worker.tempAnim.PlayAttackAnimation(worker.OnEndAttackCycle);
                        break;
                }
            }
            else if (value <= 5)
            {
                switch (type)
                {
                    case RwbpType.R:
                        foreach (AgentModel agent in target_agent)
                            agent.RecoverHP(agent.maxHp / 5f);
                        break;
                    case RwbpType.W:
                        foreach (AgentModel agent in target_agent)
                            agent.RecoverMental(agent.maxMental / 5f);
                        break;
                    case RwbpType.B:
                        foreach (CreatureModel creature in target_creature)
                            creature.AddUnitBuf(new DamageFactorBuf(Data.DamageFactorBufValue * 5));
                        break;
                    case RwbpType.P:
                        foreach (CreatureModel creature in target_creature)
                        {
                            creature.AddUnitBuf(new DamageFactorBuf(Data.DamageFactorBufValue, 1));
                            worker.Equipment.weapon.OnAttack(worker, creature);
                        }
                        worker.tempAnim.PlayAttackAnimation(worker.OnEndAttackCycle);
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case RwbpType.R:
                        foreach (AgentModel agent in target_agent)
                            agent.RecoverHP(agent.maxHp / 10f);
                        break;
                    case RwbpType.W:
                        foreach (AgentModel agent in target_agent)
                            agent.RecoverMental(agent.maxMental / 10f);
                        break;
                    case RwbpType.B:
                        foreach (CreatureModel creature in target_creature)
                            creature.AddUnitBuf(new DamageFactorBuf(Data.DamageFactorBufValue));
                        break;
                    case RwbpType.P:
                        foreach (CreatureModel creature in target_creature)
                            worker.Equipment.weapon.OnAttack(worker, creature);
                        worker.tempAnim.PlayAttackAnimation(worker.OnEndAttackCycle);
                        break;
                }
            }
        }
    }

    private int SuccessValue(AgentModel agent, RwbpType type)
    {
        switch (type)
        {
            case RwbpType.R:
                return (int)(agent.fortitudeStat * 3/4f);
            case RwbpType.W:
                return (int)(agent.prudenceStat * 3/4f);
            case RwbpType.B:
                return (int)(agent.temperanceStat / 4f);
            case RwbpType.P:
                return (int)(agent.justiceLevel / 4f);
        }
        return 0;
    }
}

public class DiceIsRightAnim : CreatureAnimScript
{
    public SkeletonAnimation Animator;
    public DiceIsRightScript Script;

    public void SetScript(DiceIsRightScript script)
    {
        Script = script;
        Animator = GetComponent<SkeletonAnimation>();
        DefaultAnim();
    }

    public void DefaultAnim()
    {
        AnimationState state = Animator.AnimationState;
        state.SetAnimation(0, "default", true);
        state.SetAnimation(1, "rotate", true);
    }

    public void RollAnim()
    {
        AnimationState state = Animator.AnimationState;
        state.GetCurrent(0).Complete += delegate (TrackEntry entry1)
        {
            state.SetAnimation(0, "default", false).Complete += delegate (TrackEntry entry2)
            {
                DefaultAnim();
            };
            state.SetAnimation(1, "roll", false).Event += delegate (TrackEntry entry2, Event e)
            {
                switch (e.Data.Name)
                {
                    case "dice_roll":
                        DiceIsRightHelper.D100(out int tens1, out int ones1);
                        SetDiceValue(tens1, ones1);
                        break;
                    case "run_effect":
                        int value = DiceIsRightHelper.D100(out int tens2, out int ones2);
                        SetDiceValue(tens2, ones2);
                        Script.RollEffect(Script.LastUseSkill, value);
                        break;
                }
            };
        };
    }

    public void SetDiceValue(int tens_place, int ones_place)
    {
        AnimationState state = Animator.AnimationState;
        state.SetAnimation(2, $"d100_{tens_place}0", true);
        state.SetAnimation(3, $"d100_{ones_place}", true);
    }
}

public class DiceIsRightWeapon : EquipmentScriptBase
{
    public override WeaponDamageInfo OnAttackStart(UnitModel actor, UnitModel target)
    {
        base.OnAttackStart(actor, target);
        List<DamageInfo> list = [];
        int value = DiceIsRightHelper.RandomInt(1, 10);
        for (int i = 0; i < value; i++) list.Add(base.model.metaInfo.damageInfos[i].Copy());
        return new(model.metaInfo.animationNames[value - 1], [.. list]);
    }
}