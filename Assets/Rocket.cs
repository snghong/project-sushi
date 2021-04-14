using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 250f;
    [SerializeField] float mainThrust = 50f;
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip success;
    [SerializeField] AudioClip death;
    [SerializeField] float levelLoadDelay = 2f; 

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem deathParticles;

    enum State {Alive, Dying, Transcending }
    State state = State.Alive;
  

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()

    {   if (state == State.Alive)
        { 
            RespondToThrustInput();
            RespondToRotateInput();
        }
       


        
    }
    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
            
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartDeathSequence();
                break;

        }    

        }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(death);
        deathParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void StartSuccessSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(success);
        successParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay); //parametise time
    }
   

    private void LoadFirstLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        int finalSceneIndex = SceneManager.sceneCountInBuildSettings;
        if (currentSceneIndex <= 4)
            SceneManager.LoadScene(0);
        else if (currentSceneIndex == 5)
            SceneManager.LoadScene(5);
        else if (currentSceneIndex == 6)
            SceneManager.LoadScene(5);
        else if (currentSceneIndex == 7)
            SceneManager.LoadScene(5);
        else if (currentSceneIndex == 8)
            SceneManager.LoadScene(5);
        else if (currentSceneIndex == 9)
            SceneManager.LoadScene(5);
        else 
            SceneManager.LoadScene(10);

    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        int finalSceneIndex = SceneManager.sceneCountInBuildSettings;

        if (currentSceneIndex == finalSceneIndex) 
            SceneManager.LoadScene(0);
        else
            SceneManager.LoadScene(nextSceneIndex);
        //todo change from definite number to an expression so we can have more scenes
    }

    private void RespondToRotateInput()
    {
        rigidBody.freezeRotation = true; // take manual control of rotation
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.A))
        {
            
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }

        rigidBody.freezeRotation = false; // resume physics

    }

    private void RespondToThrustInput()
    {
        float thrustThisFrame = mainThrust * Time.deltaTime;
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust(thrustThisFrame);
        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust(float thrustThisFrame)
    {
        rigidBody.AddRelativeForce(Vector3.up * thrustThisFrame);
        if (audioSource.isPlaying == false)
        {
            audioSource.PlayOneShot(mainEngine);
            mainEngineParticles.Play();
        }
    }
}
