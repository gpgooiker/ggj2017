﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerWave
{
	public float MaxLifeTimeSeconds { get; set; }
	public float CurrentLifeTimeSeconds { get; set; }
	public float ScaleUnitsPerSecond { get; set; }
	public float CurrentScale { get; set; }
	public GameObject Obj { get; set; }
}

public class PlayerWaves : MonoBehaviour
{
	public AudioClip WaveSound;
	private AudioSource source;
	
	public float LifeTimeSeconds = 3.0f;
	//public float FinalScale = 5.0f;

	private float LifeTimeSecondsMultiplier = 1.0f;
	public float StrengthUnitsSecondsMultiplier = 1.0f;
	private float MinimumLifeTimeSeconds = 1.0f;
	public float StrengthSpeedSecondsMultiplier = 1.0f;
	
	public List<PlayerWave> Waves { get; private set; }

	private GameObject basePlayerWave;
	private Sprite waveSprite;

	public void AddNewWave(Vector3 worldPos, Vector3 worldNormal, float timePressedSecond)
	{
		PlayerWave wave = new PlayerWave();
		wave.MaxLifeTimeSeconds = LifeTimeSeconds;// Mathf.Max(timePressedSecond * LifeTimeSecondsMultiplier, MinimumLifeTimeSeconds);
		wave.CurrentLifeTimeSeconds = wave.MaxLifeTimeSeconds;
		wave.ScaleUnitsPerSecond = StrengthUnitsSecondsMultiplier;

		// rotate up towards normal of surface
		Vector3 worldPosNoZ = new Vector3(worldPos.x, worldPos.y, -0.1f);
		Quaternion rotateUpTowardsNormal = Quaternion.FromToRotation(Vector3.up, worldNormal);
		wave.Obj = Object.Instantiate(basePlayerWave, worldPosNoZ, rotateUpTowardsNormal);
		wave.Obj.SetActive(true);
		wave.Obj.tag = "Wave";
		wave.Obj.transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);

		CircleCollider2D sphereColider = wave.Obj.AddComponent<CircleCollider2D>();
		sphereColider.radius = 0.5f;
		
		Waves.Add(wave);
		
        //source.PlayOneShot(WaveSound, 0.3f);
	}

	void Start()
	{
		Waves = new List<PlayerWave>();

		// cache sprite and base object
		waveSprite = Resources.Load<Sprite>("WaveArt");
		basePlayerWave = new GameObject();
		basePlayerWave.SetActive(false);
		basePlayerWave.layer = 8;
		SpriteRenderer renderer = basePlayerWave.AddComponent<SpriteRenderer>();
		renderer.sprite = waveSprite;
		renderer.sortingLayerName = "Gameplay";

		source = gameObject.AddComponent<AudioSource>();
    }

	void Update()
	{
		// update game objects correctly
		List<PlayerWave> wavesToDelete = new List<PlayerWave>();
		foreach (PlayerWave wave in Waves)
		{
			// scale game object
			float currentScale = (wave.MaxLifeTimeSeconds - wave.CurrentLifeTimeSeconds) * wave.ScaleUnitsPerSecond;
			wave.Obj.transform.localScale = new Vector3(currentScale, currentScale, 1.0f);
			wave.CurrentScale = currentScale;

			// render game object fade
			float uniformLifeTime = wave.CurrentLifeTimeSeconds / wave.MaxLifeTimeSeconds;

			SpriteRenderer renderer = wave.Obj.GetComponent<SpriteRenderer>();
			renderer.color *= new Color( 1.0f, 1.0f, 1.0f, 0.0f);
			renderer.color += new Color(0f,0f,0f, uniformLifeTime);

			// update lifetime and kill waves that died
			if (wave.CurrentLifeTimeSeconds <= 0.0f) {
				wavesToDelete.Add (wave);
			}

			wave.CurrentLifeTimeSeconds -= Time.deltaTime;
		}

		// delete died game objects
		foreach (PlayerWave wave in wavesToDelete)
		{
			Waves.Remove(wave);
			Object.Destroy(wave.Obj);
		}
	}
}
