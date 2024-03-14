public interface IBullet {
    public float Damage { get; set; }
    public float BaseDamagePercentage { get; set; }
    public void Deactivate();
}