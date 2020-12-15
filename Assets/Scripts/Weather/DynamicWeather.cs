using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class DynamicWeather : MonoBehaviour
{
    private Transform _player;
    private Transform _weather;
    private float _weatherHeight = 15f;
    
    public ParticleSystem _rainParticleSystem;
    public ParticleSystem _cloudParticleSystem;
    public GameObject _sunnyParticleSystem;
    
    private float _switchWeatherTimer = 0f;
    public float _resetWeatherTimer;

    public float _skyboxBlendValue;
    public float _skyboxBlendTime = 0.25f;
    private bool _sunnyState;
    private bool _cloudState;
    private bool _rainState;
    
     private WeatherStates _weatherState;                        //Defines the naming convention of our weather states
    private int _switchWeather;

    public float _audioFadeTime = 0.25f;
    private AudioClip _rainAudio;

    public Light _sunLight;
    public float _lightDimTime = 0.001f;
    public float _minimumIntensity = 0f;
    public float _maximumIntensity = 1f;
    public float _cloudIntensity = .5f;
    public float _rainIntensity = .25f;
    public float _sunnyIntensity = 1f;

    public Color _sunFog;
    public Color _cloudFog;
    public Color _rainFog;
    public float _fogChangeSpeed = 0.1f;

    enum WeatherStates                                  // Defines all weather states.
    {
        PickWeather,
        SunnyWeather,
        RainWeather,
        CloudWeather
    }
    // Start is called before the first frame update
    void Start()
    {
        GameObject _playerGameObject = GameObject.FindGameObjectWithTag("Player");
        _player = _playerGameObject.transform;
        
        GameObject _weatherGameObject = GameObject.FindGameObjectWithTag("Weather");
        _weather = _weatherGameObject.transform;
        
        RenderSettings.fog = true;
        RenderSettings.fogMode = FogMode.ExponentialSquared;
        RenderSettings.fogDensity = 0.01f;
        
        _cloudParticleSystem.Stop();
        _rainParticleSystem.Stop();
        _sunnyParticleSystem.SetActive(true);
        
        //RenderSettings.skybox.SetFloat("_Blend", 0);
        
        StartCoroutine(WeatherFSM());
    }

    // Update is called once per frame
    void Update()
    {
        SwitchWeatherTimer();
        
        _weather.transform.position = new Vector3(
                                        _player.position.x,
                                        _player.position.y + _weatherHeight,
                                        _player.position.z);
    }

    void SwitchWeatherTimer()
    {
        //print("SwitchWeatherTimer");
        _switchWeatherTimer -= Time.deltaTime;

        if (_switchWeatherTimer < 0)
            _switchWeatherTimer = 0;

        if (_switchWeatherTimer > 0)
            return;

        if (_switchWeatherTimer == 0)
            _weatherState = WeatherStates.PickWeather;

        _resetWeatherTimer = UnityEngine.Random.Range(5f, 10f);

        _switchWeatherTimer = _resetWeatherTimer;
    }

    IEnumerator WeatherFSM()
    {
        while (true)
        {
            switch(_weatherState)
            {
                case WeatherStates.PickWeather:
                    PickWeather();
                    break;
                case WeatherStates.SunnyWeather:
                    SunnyWeather();
                    break;
                case WeatherStates.RainWeather:
                    RainWeather();
                    break;
                case WeatherStates.CloudWeather:
                    CloudWeather();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return null;
        }
    }

    void PickWeather()
    {
        _switchWeather = UnityEngine.Random.Range(0, 2);
        
        _sunnyState = false;
        _cloudState = false;
        _rainState = false;

        switch (_switchWeather)
        {
            case 0:
                _weatherState = WeatherStates.SunnyWeather;
                break;
            case 1:
                _weatherState = WeatherStates.RainWeather;
                break;
            case 2:
                _weatherState = WeatherStates.CloudWeather;
                break;
        }
    }

    void SunnyWeather()
    {
        //TODO: implement audio control.
        //print("Sunny Weather");
        _cloudParticleSystem.Stop();
        _rainParticleSystem.Stop();
        _sunnyState = true;
        
        
        //AudioSource _audioComp = GetComponent<AudioSource>();
        LightManager(_sunnyIntensity);
        if (!_sunnyParticleSystem.activeSelf)
        {
            _sunnyParticleSystem.SetActive(true);
        }
        FogManager(_sunFog);
    }

    void RainWeather()
    {
        //TODO: implement audio control.
        //print("Rain Weather");
        _sunnyParticleSystem.SetActive(false);
        _rainState = true;
        //AudioSource _audioComp = GetComponent<AudioSource>();
        LightManager(_rainIntensity);
        if (!_rainParticleSystem.isPlaying)
        {
            _rainParticleSystem.Play();
        }

        if (!_cloudParticleSystem.isPlaying)
        {
            _cloudParticleSystem.Play();
        }
        FogManager(_rainFog);
    }
    //TODO: Rainshower weather
    void CloudWeather()
    {
        //TODO: implement audio control.
        //print("Cloud Weather");
        _cloudState = true;
        _rainParticleSystem.Stop();
        _sunnyParticleSystem.SetActive(false);
        //AudioSource _audioComp = GetComponent<AudioSource>();
        LightManager(_cloudIntensity);
        if (!_cloudParticleSystem.isPlaying)
        {
            _cloudParticleSystem.Play();
        }
        FogManager(_cloudFog);
    }
    
    void LightManager(float weatherIntensity)
    {
        if (_sunLight.intensity == weatherIntensity)
            return;
        
        if (_sunLight.intensity > weatherIntensity)
            _sunLight.intensity -= Time.deltaTime * _lightDimTime;
        
        if (_sunLight.intensity > weatherIntensity)
            _sunLight.intensity += Time.deltaTime * _lightDimTime;
    }

    void FogManager(Color _weatherFog)
    {
        Color currentFogColor = RenderSettings.fogColor;
        RenderSettings.fogColor = Color.Lerp(currentFogColor, _weatherFog, _fogChangeSpeed * Time.deltaTime);
    }

    void SkyBoxBlendManager()
    {
        //TODO: get skyboxes to implement blender
        if (_sunnyState)
        {
            if (_skyboxBlendValue == 0)
                return;
            
            _skyboxBlendValue -= _skyboxBlendTime * Time.deltaTime;
            if (_skyboxBlendValue < 0)
                _skyboxBlendValue = 0;
            
            RenderSettings.skybox.SetFloat("_Blend", _skyboxBlendValue);
        } 
        else if (_cloudState)
        {
            if (_skyboxBlendValue == 0.5f)
                return;
            
            if (_skyboxBlendValue < 0.5f)
            {
                _skyboxBlendValue += _skyboxBlendTime * Time.deltaTime;
                if (_skyboxBlendValue > 0.5f)
                    _skyboxBlendValue = 0.25f;
            }
            else if (_skyboxBlendValue > 0.5f)
            {
                _skyboxBlendValue -= _skyboxBlendTime * Time.deltaTime;
                if (_skyboxBlendValue < 0.5f)
                    _skyboxBlendValue = 0.5f;
            }
            
            RenderSettings.skybox.SetFloat("_Blend", _skyboxBlendValue);
        } 
        else if (_rainState)
        {
            if (_skyboxBlendValue == 1)
                return;
            
            _skyboxBlendValue += _skyboxBlendTime * Time.deltaTime;
            if (_skyboxBlendValue > 1)
                _skyboxBlendValue = 1;
            
            RenderSettings.skybox.SetFloat("_Blend", _skyboxBlendValue);
        }
    }
}
