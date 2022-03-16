using UnityEngine;

public class VFXManeger : MonoBehaviour
{
    public static VFXManeger Instance;

    [SerializeField] ParticleSystem _ballToGuardVFX;
    ParticleSystem.MainModule _ballToGuardVfxMainModule;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        _ballToGuardVfxMainModule = _ballToGuardVFX.main;
        _ballToGuardVFX.Stop();
    }

    /// <summary>
    /// Scatter the particles at the point where the ball hits the wrong guard
    /// </summary>
    /// <param name="position"></param>
    /// <param name="ballColor"></param>
    /// <param name="guardColor"></param>
    public void PlayBallToGuardVFX(Vector2 position, Color ballColor, Color guardColor)
    {
        _ballToGuardVFX.transform.position = position;
        Gradient gradient = new Gradient();
        gradient.mode = GradientMode.Fixed;

        GradientAlphaKey[] alphaKey = new GradientAlphaKey[1];
        GradientColorKey[] colorKeys = new GradientColorKey[2];

        alphaKey[0].alpha = 1f;
        alphaKey[0].time = 1f;

        colorKeys[0].time = 0.2f;
        colorKeys[0].color = ballColor;

        colorKeys[1].time = 1f;
        colorKeys[1].color = guardColor;

        gradient.colorKeys = colorKeys;
        gradient.alphaKeys = alphaKey;

        ParticleSystem.MinMaxGradient minMaxGradient = new ParticleSystem.MinMaxGradient(gradient);
        minMaxGradient.mode = ParticleSystemGradientMode.RandomColor;
        _ballToGuardVfxMainModule.startColor = minMaxGradient;

        _ballToGuardVFX.Play();
    }
}
