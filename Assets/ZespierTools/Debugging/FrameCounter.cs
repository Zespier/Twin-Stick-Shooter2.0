using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FrameCounter : MonoBehaviour
{

    public TMP_Text frames;

    private List<float> _lastFrames = new List<float>() { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };
    private int _currentIndex;
    private float _media;
    private float _lastMedia;
    private int _frameListCountBasedOnMedia = 100;

    private void Update()
    {
        float framesThisFrame = (1f / Time.deltaTime);

        ClampFrameListCount(framesThisFrame);

        AddFramesToList(framesThisFrame);

        CalculateMedia();

        PrintMedia();
    }

    private void AddFramesToList(float framesThisFrame)
    {
        _currentIndex = (_currentIndex + 1) % _lastFrames.Count;

        if (_lastFrames.Count < _frameListCountBasedOnMedia)
        {
            for (int i = 0; i < _frameListCountBasedOnMedia - _lastFrames.Count; i++)
            {
                _lastFrames.Add(framesThisFrame);
            }
        }
        else
        {
            _lastFrames[_currentIndex] = framesThisFrame;
        }
    }

    private void CalculateMedia()
    {
        _media = 0;
        for (int i = 0; i < _lastFrames.Count; i++)
        {
            _media += _lastFrames[i];
        }

        _media /= _lastFrames.Count;
    }

    private void PrintMedia()
    {
        _lastMedia = Mathf.Lerp(_lastMedia, _media, Time.deltaTime / .9f);
        frames.text = _lastMedia.ToString("F0");
    }

    private void ClampFrameListCount(float framesThisFrame)
    {
        _frameListCountBasedOnMedia = (int)Mathf.Pow(framesThisFrame, 1.1f);

        if (_lastFrames.Count > _frameListCountBasedOnMedia)
        {
            int newCount = (int)Mathf.Lerp(_lastFrames.Count, _frameListCountBasedOnMedia, Time.deltaTime / 0.5f);
            _lastFrames = _lastFrames.GetRange(0, newCount);

            if (_currentIndex >= _lastFrames.Count)
            {
                _currentIndex = -1;
            }
        }
    }
}
