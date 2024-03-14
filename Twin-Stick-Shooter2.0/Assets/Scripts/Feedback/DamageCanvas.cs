using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageCanvas : MonoBehaviour {

    public float deinflationTime = 0.6f;
    public float increasedSize = 0.9f;
    public float fadeOutTime = 0.3f;
    public float standByTime = 0.3f;
    public TMP_Text damageText;

    private Vector3 enemyPosition;
    private Vector3 _defaultScale;
    private bool _isCrit;

    public void Initialize(Enemy enemy, float damage, bool crit) {
        damageText.text = damage.ToString("F0");
        enemyPosition = enemy.transform.position;
        _defaultScale = transform.localScale;
        _isCrit = crit;
        Destroy(gameObject, deinflationTime + standByTime + fadeOutTime + 0.2f);
        StartCoroutine(DamageAnimation());
    }
    public void Initialize(Vector3 position, float damage, bool crit) {
        damageText.text = damage.ToString("F0");
        enemyPosition = position;
        _defaultScale = transform.localScale;
        _isCrit = crit;
        Destroy(gameObject, deinflationTime + standByTime + fadeOutTime + 0.2f);
        StartCoroutine(DamageAnimation());
    }

    private IEnumerator DamageAnimation() {

        //Random position around the player
        transform.position = new Vector3(enemyPosition.x + Random.Range(-0.7f, 0.7f), 0, enemyPosition.z + Random.Range(0, 1f));
        //Increase the size based in crit
        transform.localScale = _isCrit ? _defaultScale * (1 + increasedSize * 2.5f) : _defaultScale * (1 + increasedSize);

        float timer = deinflationTime;
        while (timer >= deinflationTime / 2f) {
            transform.localScale = Vector3.Lerp(transform.localScale, _defaultScale, 1 - (timer / deinflationTime));

            timer -= Time.deltaTime;
            yield return null;
        }
        //TODO: Se mantiene el tamaño un segundo casi
        timer = standByTime;
        while (timer >= 0) {

            timer -= Time.deltaTime;
            yield return null;
        }
        //TODO: Desaparece
        timer = fadeOutTime;

        while (timer >= 0) {
            Color newColor = damageText.color;
            newColor.a = Mathf.Lerp(newColor.a, 0, 1 - (timer / fadeOutTime));
            damageText.color = newColor;

            timer -= Time.deltaTime;
            yield return null;
        }
    }
}
