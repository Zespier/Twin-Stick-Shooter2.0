using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FeedbackController : MonoBehaviour {

    public GameObject inventoryFullPrefab;
    public GameObject healingFeedBackPrefab;
    public DamageCanvas damageCanvas;
    public Color damageColor = Color.white;
    public Color playerDamagedColor = Color.HSVToRGB(0, 69, 86);
    public Color healingColor = Color.green;
    public Transform mainCamera;

    public List<ParticleFeedback> particleSystemPrefabs = new List<ParticleFeedback>();
    public ParticleType visualReferenceToAllTypes;

    private List<ParticleFeedback> _particlePool = new();
    private List<DamageCanvas> _damageCanvasPool = new List<DamageCanvas>(capacity: 64);

    public static FeedbackController instance;
    private void Awake() {
        if (!instance) {
            instance = this;
        }

        InitializePools();
    }

    private void InitializePools() {
        for (int i = 0; i < particleSystemPrefabs.Count; i++) {
            ParticleFeedback newParticle = Instantiate(particleSystemPrefabs[i], transform);
            GetIntoThePool(newParticle);
        }

        DamageCanvas newDamageCanvas = Instantiate(damageCanvas, transform);
        GetIntoThePool(newDamageCanvas);
    }

    public ParticleFeedback GetFromPool(ParticleType particleType) {

        for (int i = 0; i < _particlePool.Count; i++) {
            if (_particlePool[i].type == particleType && !_particlePool[i].particleSystem.isPlaying) {
                _particlePool[i].gameObject.SetActive(true);
                return _particlePool[i];
            }
        }

        ParticleFeedback newParticle = Instantiate(particleSystemPrefabs[particleSystemPrefabs.FindIndex(psp => psp.type == particleType)], transform);
        GetIntoThePool(newParticle);
        newParticle.gameObject.SetActive(true);
        return newParticle;
    }

    public DamageCanvas GetDamageCanvasFromPool() {

        for (int i = 0; i < _damageCanvasPool.Count; i++) {
            if (!_damageCanvasPool[i].gameObject.activeSelf) {
                return _damageCanvasPool[i];
            }
        }

        DamageCanvas newDamageCanvas = Instantiate(damageCanvas, transform);
        GetIntoThePool(newDamageCanvas);
        return newDamageCanvas;
    }

    public void GetIntoThePool(ParticleFeedback particleFeedback) {
        if (!_particlePool.Contains(particleFeedback)) {
            _particlePool.Add(particleFeedback);
        }
    }

    public void GetIntoThePool(DamageCanvas damageCanvas) {
        damageCanvas.ResetCanvas();
        damageCanvas.gameObject.SetActive(false);
        _damageCanvasPool.Add(damageCanvas);
    }

    public ParticleFeedback Particles(ParticleType particleType, Vector3 position, Vector3 direction, Transform newParent = default) {
        if (newParent == default) { newParent = transform; }

        ParticleFeedback particleFeedback = GetFromPool(particleType);
        particleFeedback.transform.position = position;
        particleFeedback.transform.forward = direction;
        particleFeedback.transform.parent = newParent;
        particleFeedback.particleSystem.Play();
        return particleFeedback;
    }

    /// <summary>
    /// Creates a damage canvas with the parameters on the position 
    /// </summary>
    /// <param name="position"></param>
    /// <param name="damage"></param>
    /// <param name="crit"></param>
    /// <param name="damageType"></param>
    public void DamageFeedBack(Vector3 position, float damage, bool crit, DamageType damageType) {
        DamageCanvas damageCanvas = GetDamageCanvasFromPool();
        damageCanvas.Initialize(position, damage, crit);

        switch (damageType) {
            case DamageType.PlayerDamaged:
                damageCanvas.damageText.color = playerDamagedColor;
                break;
            case DamageType.DefaultWhite:
            default:
                damageCanvas.damageText.color = damageColor;
                break;
            case DamageType.Healing:
                damageCanvas.damageText.color = healingColor;
                break;
        }
    }

    #region Commented Future Methods


    //public void InventoryIsFull() {
    //    Destroy(Instantiate(inventoryFullPrefab, canvasPrefabs.sceneCanvas), 2f);
    //}

    //public void HealingFeedBack(int healing) {
    //    GameObject feedBack = Instantiate(healingFeedBackPrefab, canvasPrefabs.sceneCanvas);
    //    Text text = feedBack.GetComponentInChildren<Text>();
    //    text.text = healing.ToString("F0");
    //    Destroy(feedBack, 1.5f);
    //}

    //private IEnumerator ChangeConfigurationAnimation() {
    //    _changeConfiguration.SetActive(true);
    //    _changeConfiguration.transform.localPosition = _changeDefaultPosition;

    //    Vector3 newPosition = _changeDefaultPosition + Vector3.up * changeMovement;
    //    float totalTime = 0.3f;
    //    float timer = totalTime;
    //    while (timer >= 0) {
    //        _changeConfiguration.transform.localPosition = Vector3.Lerp(_changeConfiguration.transform.localPosition, newPosition, 1 - (timer / totalTime));
    //        timer -= Time.deltaTime;
    //        yield return null;
    //    }

    //    _changeConfiguration.SetActive(false);
    //}

    //public void EffectFeedBack(Enemy enemy, string effect) {
    //    //Instantiate a new Feedback Canvas
    //    GameObject newEffectFeedBack = Instantiate(effectFeedbackPrefab, enemy.transform);
    //    Vector3 defaultScale = newEffectFeedBack.transform.localScale; //Get the default scale to deinflate it

    //    Image image = newEffectFeedBack.GetComponentInChildren<Image>();
    //    image.sprite = Resources.Load<Sprite>("MarkEffects/" + effect); //Load the image of the effect

    //    Destroy(newEffectFeedBack, deinflationTime + standByTime + fadeOutTime + 0.3f); //Destroy it when the animation stops
    //    StartCoroutine(EffectAppliedAnimation(newEffectFeedBack.transform, enemy.transform.position, defaultScale, image)); //The animation
    //    //StartCoroutine(PoisonAnimation(newEffectFeedBack));
    //}

    //private IEnumerator EffectAppliedAnimation(Transform instance, Vector3 enemyPosition, Vector3 defaultScale, Image image) {

    //    //Random position around the player
    //    instance.position = new Vector3(enemyPosition.x + Random.Range(-0.3f, 0.3f), enemyPosition.y + Random.Range(0.2f, 0.4f), 0);
    //    //Increase the size based in crit
    //    instance.localScale = defaultScale * (1 + increasedSize);

    //    float timer = deinflationTime;
    //    while (timer >= 0) {
    //        instance.localScale = Vector3.Lerp(instance.localScale, defaultScale, 1 - (timer / deinflationTime));

    //        timer -= Time.deltaTime;
    //        yield return null;
    //    }
    //    //TODO: Se mantiene el tamaño un segundo casi
    //    timer = standByTime;
    //    while (timer >= 0) {

    //        timer -= Time.deltaTime;
    //        yield return null;
    //    }
    //    //TODO: Desaparece
    //    timer = fadeOutTime;

    //    while (timer >= 0) {
    //        Color newColor = image.color;
    //        newColor.a = Mathf.Lerp(newColor.a, 0, 1 - (timer / fadeOutTime));
    //        image.color = newColor;

    //        timer -= Time.deltaTime;
    //        yield return null;
    //    }
    //}

    //public void BuffArrows() {
    //    if (_buffFeedBack != null) {
    //        StopCoroutine(_buffFeedBack);
    //    }
    //    _buffFeedBack = StartCoroutine(BuffFeedBack());
    //}

    //public void StartGenericCoroutine() {
    //    if (_debuffFeedBack != null) {
    //        StopCoroutine(_debuffFeedBack);
    //    }
    //    _debuffFeedBack = StartCoroutine(DebuffFeedBack());
    //}

    #endregion
}
public enum DamageType : byte {
    DefaultWhite,
    PlayerDamaged,
    Healing,
}