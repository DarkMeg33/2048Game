using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
//using GoogleMobileAds.Api;

public enum GameState
{
	Playing,
	GameOver, 
	WaitingForMoveToEnd
}


public class GameManager : MonoBehaviour {

	// NEW AFTER ADDED DELAYS
	public GameState State;
	[Range(0, 2f)]
	public float delay;
	public int hour1, hour2, minute1, minute2, second1, second2;
	private float gameTime;
	private bool moveMade;
	private bool play = true;
	private bool[] lineMoveComplete = new bool[4]{true, true, true, true};
	// NEW AFTER ADDED DELAYS

	public GameObject YouWonText;
	public GameObject GameOverText;
	public Text GameOverScoreText;
	public Text ClockText;
	public GameObject GameOverPanel;

	private Tile[,] AllTiles = new Tile[4,4];
	private List <Tile[]> columns = new List<Tile[]> ();
	private List <Tile[]> rows = new List<Tile[]> ();
	private List<Tile> EmptyTiles = new List<Tile>();

	//Ads
	//private InterstitialAd ad;
	private const string banner = "ca-app-pub-6601553821464262/1354399020"; 
	private const string video = "ca-app-pub-6601553821464262/2283147943";


	// Use this for initialization
	void Start () 
	{
		//BannerView bannerV = new BannerView(banner,AdSize.Banner,AdPosition.Bottom);
		//AdRequest request = new AdRequest.Builder().Build();
		//bannerV.LoadAd(request);
		Tile[] AllTilesOneDim = GameObject.FindObjectsOfType<Tile> ();
		foreach (Tile t in AllTilesOneDim) 
		{
			t.Number = 0;
			AllTiles[t.indRow, t.indCol] = t;
			EmptyTiles.Add (t);
		}

		columns.Add (new Tile[]{AllTiles [0, 0], AllTiles [1, 0], AllTiles [2, 0], AllTiles [3, 0]});
		columns.Add (new Tile[]{AllTiles [0, 1], AllTiles [1, 1], AllTiles [2, 1], AllTiles [3, 1]});
		columns.Add (new Tile[]{AllTiles [0, 2], AllTiles [1, 2], AllTiles [2, 2], AllTiles [3, 2]});
		columns.Add (new Tile[]{AllTiles [0, 3], AllTiles [1, 3], AllTiles [2, 3], AllTiles [3, 3]});

		rows.Add (new Tile[]{AllTiles [0, 0], AllTiles [0, 1], AllTiles [0, 2], AllTiles [0, 3]});
		rows.Add (new Tile[]{AllTiles [1, 0], AllTiles [1, 1], AllTiles [1, 2], AllTiles [1, 3]});
		rows.Add (new Tile[]{AllTiles [2, 0], AllTiles [2, 1], AllTiles [2, 2], AllTiles [2, 3]});
		rows.Add (new Tile[]{AllTiles [3, 0], AllTiles [3, 1], AllTiles [3, 2], AllTiles [3, 3]});

		State = GameState.Playing;

		Generate ();
		Generate ();
	}

	void Update()
	{
		if (play == true)
		{
			ClockText.text = "" + hour1 + hour2 + ":" + minute1 + minute2 + ":" + second1 + second2;
			gameTime += Time.deltaTime;
			if (gameTime > 1)
			{
				second2 += 1;
				gameTime = 0;
			}
			if (second2 > 9)
			{
				second1 += 1;
				second2 = 0;
			}
			if (second1 > 5)
			{
				minute2 += 1;
				second1 = 0;
			}
			if (minute2 > 9)
			{
				minute1 += 1;
				minute2 = 0;
			}
			if (minute1 > 5)
			{
				hour2 += 1;
				minute1 = 0;
			}
			if (hour2 > 9)
			{
				hour1 += 1;
				hour2 = 0;
			}
		}
	}

	private void YouWon()
	{
		State = GameState.GameOver;
		GameOverText.SetActive(false);
		YouWonText.SetActive(true);
		GameOverScoreText.text = ScoreTracker.Instance.Score.ToString();
		GameOverPanel.SetActive(true);
		play = false;
	}

	private void GameOver()
	{
		//ad = new InterstitialAd(video);
		//AdRequest request = new AdRequest.Builder().Build();
		//ad.LoadAd(request);
		//ad.OnAdLoaded += OnAdLoaded;
		State = GameState.GameOver;
		GameOverScoreText.text = ScoreTracker.Instance.Score.ToString ();
		GameOverPanel.SetActive (true);
		play = false;
	}

	bool CanMove()
	{
		if (EmptyTiles.Count > 0)
			return true;
		else 
		{
			// check columns
			for(int i = 0; i< columns.Count; i++)
				for (int j = 0; j< rows.Count-1; j++)
					if (AllTiles[j, i].Number == AllTiles[j+1, i].Number)
						return true;

			// check rows
			for(int i = 0; i< rows.Count; i++)
				for (int j = 0; j< columns.Count-1; j++)
					if (AllTiles[i, j].Number == AllTiles[i, j+1].Number)
						return true;

		}
		return false;
	}

	//public void NewGameButtonHandler()
	//{
	//	Application.LoadLevel(Application.loadedLevel);
	//}

	bool MakeOneMoveDownIndex(Tile[] LineOfTiles)
	{
		for (int i =0; i< LineOfTiles.Length-1; i++) 
		{
			//MOVE BLOCK 
			if (LineOfTiles[i].Number == 0 && LineOfTiles[i+1].Number != 0)
			{
				LineOfTiles[i].Number = LineOfTiles[i+1].Number;
				LineOfTiles[i+1].Number = 0;
				return true;
			}
			// MERGE BLOCK
			if (LineOfTiles[i].Number!= 0 && LineOfTiles[i].Number == LineOfTiles[i+1].Number &&
			    LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i+1].mergedThisTurn == false)
			{
				LineOfTiles[i].Number *= 2;
				LineOfTiles[i+1].Number = 0;
				LineOfTiles[i].mergedThisTurn = true;
				LineOfTiles[i].PlayMergeAnimation();
				ScoreTracker.Instance.Score += LineOfTiles[i].Number;
				if (LineOfTiles[i].Number == 16384)
					YouWon();
				return true;
			}
		}
		return false;
	}

	bool MakeOneMoveUpIndex(Tile[] LineOfTiles)
	{
		for (int i =LineOfTiles.Length-1; i > 0; i--) 
		{
			//MOVE BLOCK 
			if (LineOfTiles[i].Number == 0 && LineOfTiles[i-1].Number != 0)
			{
				LineOfTiles[i].Number = LineOfTiles[i-1].Number;
				LineOfTiles[i-1].Number = 0;
				return true;
			}
			// MERGE BLOCK
			if (LineOfTiles[i].Number!= 0 && LineOfTiles[i].Number == LineOfTiles[i-1].Number &&
			    LineOfTiles[i].mergedThisTurn == false && LineOfTiles[i-1].mergedThisTurn == false)
			{
				LineOfTiles[i].Number *= 2;
				LineOfTiles[i-1].Number = 0;
				LineOfTiles[i].mergedThisTurn = true;
				LineOfTiles[i].PlayMergeAnimation();
				ScoreTracker.Instance.Score += LineOfTiles[i].Number;
				if (LineOfTiles[i].Number == 16384)
					YouWon();
				return true;
			}
		}
		return false;
	}


	
	void Generate()
	{
		if (EmptyTiles.Count > 0) 
		{
			int indexForNewNumber = Random.Range(0, EmptyTiles.Count);
			int randomNum = Random.Range(0,10);
			if (randomNum == 0)
				EmptyTiles[indexForNewNumber].Number = 4;
			else
				EmptyTiles[indexForNewNumber].Number = 2;

			EmptyTiles[indexForNewNumber].PlayAppearAnimation();

			EmptyTiles.RemoveAt(indexForNewNumber);
		}
	}
	
	// Update is called once per frame
	/*void Update () 
	{
		if (Input.GetKeyDown (KeyCode.G))
			Generate ();
	
	}*/

	private void ResetMergedFlags()
	{
		foreach (Tile t in AllTiles)
			t.mergedThisTurn = false;
	}

	private void UpdateEmptyTiles()
	{
		EmptyTiles.Clear ();
		foreach (Tile t in AllTiles) 
		{
			if(t.Number == 0)
				EmptyTiles.Add(t);
		}
	}
	
	public void Move(MoveDirection md)
	{
		Debug.Log (md.ToString () + " move.");
	    moveMade = false;
		ResetMergedFlags ();
		if (delay > 0)
			StartCoroutine (MoveCoroutine (md));
		else 
		{
			for (int i =0; i< rows.Count; i++) 
			{
				switch (md) 
				{
				case MoveDirection.Down:
					while (MakeOneMoveUpIndex(columns[i])) 
					{
						moveMade = true;
					}
					break;
				case MoveDirection.Left:
					while (MakeOneMoveDownIndex(rows[i])) 
					{
						moveMade =true;
					}
					break;
				case MoveDirection.Right:
					while (MakeOneMoveUpIndex(rows[i])) 
					{
						moveMade =true;
					}
					break;
				case MoveDirection.Up:
					while (MakeOneMoveDownIndex(columns[i])) 
					{
						moveMade =true;
					}
					break;
				}
			}

			if (moveMade) 
			{
				UpdateEmptyTiles ();
				Generate ();

				if (!CanMove())
				{
					GameOver();
				}
			
			}
		}
	}

	IEnumerator MoveOneLineUpIndexCoroutine(Tile[] line, int index)
	{
		lineMoveComplete [index] = false;
		while (MakeOneMoveUpIndex(line)) 
		{
			moveMade = true;
			yield return new WaitForSeconds(delay);
		}
		lineMoveComplete [index] = true;
	}

	IEnumerator MoveOneLineDownIndexCoroutine(Tile[] line, int index)
	{
		lineMoveComplete [index] = false;
		while (MakeOneMoveDownIndex(line)) 
		{
			moveMade = true;
			yield return new WaitForSeconds(delay);
		}
		lineMoveComplete [index] = true;
	}
	

	IEnumerator MoveCoroutine(MoveDirection md)
	{
		State = GameState.WaitingForMoveToEnd;

		// start moving each line with delays depending on MoveDirection md
		switch (md) 
		{
		case MoveDirection.Down:
			for (int i = 0; i< columns.Count; i++)
				StartCoroutine(MoveOneLineUpIndexCoroutine(columns[i], i));
			break;
		case MoveDirection.Left: 
			for (int i = 0; i< rows.Count; i++)
				StartCoroutine(MoveOneLineDownIndexCoroutine(rows[i], i));
			break;
		case MoveDirection.Right:
			for (int i = 0; i< rows.Count; i++)
				StartCoroutine(MoveOneLineUpIndexCoroutine(rows[i], i));
			break;
		case MoveDirection.Up: 
			for (int i = 0; i< columns.Count; i++)
				StartCoroutine(MoveOneLineDownIndexCoroutine(columns[i], i));
			break;

		}

		// Wait until the move is over in all lines
		while (! (lineMoveComplete[0] && lineMoveComplete[1] && lineMoveComplete[2] && lineMoveComplete[3]))
			yield return null;

		if (moveMade) 
		{
			UpdateEmptyTiles ();
			Generate ();
			
			if (!CanMove())
			{
				GameOver();
			}

		}

		State = GameState.Playing;
		StopAllCoroutines ();
	}
	public void OnAdLoaded(object sender,System.EventArgs args)
	{
		//ad.Show();
	}
}
