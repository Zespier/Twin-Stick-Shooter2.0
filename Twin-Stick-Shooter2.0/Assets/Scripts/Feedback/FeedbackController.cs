using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackController : MonoBehaviour {

    public GameObject inventoryFullPrefab;
    public GameObject healingFeedBackPrefab;
    public GameObject damageCanvas;
    public Color damageColor = Color.white;

    public static FeedbackController instance;
    private void Awake() {
        if (!instance) {
            instance = this;
        }
    }

    public void DamageFeedBack(Enemy enemy, float damage, bool crit, string damageType) {
        DamageCanvas damageCanvas = Instantiate(this.damageCanvas, enemy.transform.position, Quaternion.identity).GetComponent<DamageCanvas>();
        damageCanvas.Initialize(enemy, damage, crit);

        switch (damageType) {
            case "energy":
            case "Energy":
            default:
                damageCanvas.damageText.color = damageColor;
                break;
        }
    }
    public void DamageFeedBack(Vector3 position, float damage, bool crit, string damageType) {
        DamageCanvas damageCanvas = Instantiate(this.damageCanvas, position, Quaternion.identity).GetComponent<DamageCanvas>();
        damageCanvas.Initialize(position, damage, crit);

        switch (damageType) {
            case "energy":
            case "Energy":
            default:
                damageCanvas.damageText.color = damageColor;
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
