using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreTracker : MonoBehaviour {

	private int score;
	public static ScoreTracker Instance;
	public Text ScoreText;

	public int Score
	{
		get
		{
			return score;
		}

		set
		{
			score = value;
			ScoreText.text = score.ToString();


		}
	}

	void Awake()
	{

		//PlayerPrefs.DeleteAll ();
		Instance = this;
		ScoreText.text = "0";

	}

}
