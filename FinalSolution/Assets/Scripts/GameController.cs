using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour {

	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public int buildIndex =0;

	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameOverText;

	private bool gameOver;
	private bool restart;
	private int score;

	void Start(){
		gameOver = false;
		restart = false;
		restartText.text = "";
		gameOverText.text = "";
		score = 0;

		UpdateScore ();

		StartCoroutine(SpawnWaves ());
	}
	void Update(){
		// if (restart) {
		// 	if(Input.GetKeyDown(KeyCode.R)){
		// 		Destroy (GameObject.FindWithTag("MYO"));
		// 		SceneManager.LoadScene(buildIndex -1);
				//buildIndex++;
				//Application.LoadLevel(Application.loadedlevel);//code is supposedly obsolete
		// 	}
		// }
	}

	IEnumerator SpawnWaves(){
		yield return new WaitForSeconds(startWait);
		while(true){
			for (int i =0; i<hazardCount ; i++) {

				GameObject hazard = hazards [Random.Range (0, hazards.Length)];
				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				Instantiate (hazard, spawnPosition, spawnRotation);
				yield return new WaitForSeconds(spawnWait);
			}
			yield return new WaitForSeconds(waveWait);

			if(gameOver){
				restartText.text = "Game Over";
				restart = true;
				break;
			}
		}
		hazardCount++;
	}

	public void AddScore(int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}

	void UpdateScore(){
		scoreText.text = "Score: " + score;
	}
	public void GameOver(){
		gameOverText.text = "Game Over";
		gameOver = true;
	}
}
