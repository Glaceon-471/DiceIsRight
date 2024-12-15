namespace DiceIsRight;

public static class Data
{
    public const UnitBufType DamageFactorType = (UnitBufType)101981;
    public const float DamageFactorBufValue = 1.2f;
    public const float DamageFactorDebufValue = 0.8f;
}

public class DamageFactorBuf : UnitBuf
{
    private float DamageFactor;
    private float Time;

    public DamageFactorBuf(float factor) : this(factor, 15) { }

    public DamageFactorBuf(float factor, float time)
    {
        type = Data.DamageFactorType;
        duplicateType = BufDuplicateType.ONLY_ONE;
        DamageFactor = factor;
        Time = time;
    }

    public override void Init(UnitModel model)
    {
        base.Init(model);
        remainTime = Time;
    }

    public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo) =>
        DamageFactor;
}