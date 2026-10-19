using UnityEngine;
using UnityEngine.SceneManagement;


[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public Light lit;
    [SerializeField] public GameObject batterybar;
    [SerializeField] public Transform spawnpoint;
    [SerializeField] public GameObject pano;
    [SerializeField] public GameObject panow;
    



    [SerializeField] private LayerMask pickableLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] private LayerMask transitionLayer;
    [SerializeField] private MonoBehaviour mouseLookScript;

    [SerializeField] private GameObject pj;
    [SerializeField] private GameObject twall;
    [SerializeField] private GameObject twall1;
    [SerializeField] private GameObject twall2;
    [SerializeField] private GameObject twall3;






    public float moveSpeed = 6f;
    public float jumpHeight = 2f;
    public bool hasflashlight = true;
    public bool hasmachete = false;
    public bool onitem;
    public bool oncorn;
    public bool jacktivated = false;



    


    public float gravity = -9.81f;
    private Vector3 velocity;
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    private bool isGrounded;
    public bool lighton = true;
    
    public float flashlightbattery = 100.0f;
    public float burnrate = 0.1f;
    public float dist = 8.0f;

    public float pickupRange = 10f;
    private Renderer highlightedRenderer = null;
    private Renderer[] highlightedRenderercorns = null;

    public Color glowColor = Color.orange; 


    private CharacterController controller;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }
    void Update()
    {
        if(hasflashlight && flashlightbattery > 0){
            batterybar.SetActive(true);
            batterybar.transform.localScale = new Vector3((0.125f * (flashlightbattery/100.0f)),0.1f ,1.0f);

        }

        HandleHighlight();
        HandleCornHighlight();


        if(Input.GetMouseButtonDown(0)){
            pickup();


        }


        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(hasflashlight && lighton){
            if(lit.intensity < 1 && flashlightbattery < 0){
                lit.intensity = lit.intensity - ((100-flashlightbattery)/20000);

            }

            flashlightbattery = flashlightbattery - burnrate;
            if(flashlightbattery <= 1){
                Debug.Log("battery dead");
                lit.intensity = 0;

            }


        }


        if(Input.GetKeyDown(KeyCode.E) && !lighton && hasflashlight){
            lighton = true;
            lit.enabled = true;
            Debug.Log("lighton");

        } else if(Input.GetKeyDown(KeyCode.E) && lighton && hasflashlight){
            lighton = false;
            lit.enabled = false;
            Debug.Log("lightoff");


        }
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; 
        }
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * moveSpeed * Time.deltaTime);
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
        private void OnControllerColliderHit(ControllerColliderHit collision){
            //Debug.Log("touched sometyhing");
            if (collision.gameObject.CompareTag("transition")){
                

                Debug.Log("touched exit");

                if(SceneManager.GetActiveScene().name != "SampleScene"){
                    SceneManager.LoadScene("SampleScene");
                    Debug.Log(SceneManager.GetActiveScene().name);

                }
                else{
                    Debug.Log("touched exit");
                    Cursor.lockState = CursorLockMode.None;
                    panow.SetActive(true);
                    Time.timeScale = 0f;
                    Cursor.visible = true;
                    if (mouseLookScript != null) mouseLookScript.enabled = false;

                }

                //SceneManager.LoadScene("SampleScene");
            }
        }
    void pickup()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, dist))
        {
            if (hit.collider.CompareTag("Battery"))
            {
                Debug.Log("touched battery");
                hit.collider.gameObject.SetActive(false);
                flashlightbattery = 100.0f;
                lit.intensity = 20;
            }
            if (hit.collider.CompareTag("Machete"))
            {
                Debug.Log("machete equipped");
                hasmachete = true;
                hit.collider.gameObject.SetActive(false);
                if(!jacktivated){
                    jacktivation();
                }
            }
            if(hasmachete && hit.collider.CompareTag("Corn")){
                Debug.Log("boken");
                hit.collider.gameObject.SetActive(false);
            }
        }
    }
        void HandleHighlight() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, pickupRange, pickableLayer)){Renderer targetRenderer = hit.collider.GetComponent<Renderer>();
            if (targetRenderer != null && targetRenderer != highlightedRenderer)
            {
                ClearCurrentHighlight();
                highlightedRenderer = targetRenderer;
                highlightedRenderer.material.EnableKeyword("_EMISSION");
                highlightedRenderer.material.SetColor("_EmissionColor", glowColor);
                onitem = true;
            }
        }
        else if(!oncorn){
            onitem = true;
            ClearCurrentHighlight();
        }
    }
        void HandleCornHighlight() {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (hasmachete && Physics.Raycast(ray, out hit, pickupRange, wallLayer)){Renderer[] targetRenderer = hit.collider.GetComponentsInChildren<Renderer>();
            if (targetRenderer != null && targetRenderer != highlightedRenderercorns)
            {
                //Debug.Log("idk lolll");

                ClearCurrentHighlightCorn();

                highlightedRenderercorns = targetRenderer;
                //Material[] mats = highlightedRenderercorns.SelectMany(rend => rend.materials).ToArray();
                //highlightedRenderercorns.material.EnableKeyword("_EMISSION");
                foreach (Renderer rendy in highlightedRenderercorns)
                {
                    rendy.material.EnableKeyword("_EMISSION");
                    rendy.material.SetColor("_EmissionColor", glowColor);
                }
                //highlightedRenderercorns.material.SetColor("_EmissionColor", glowColor);
                oncorn = true;
            
            }
        }
        else if(!onitem){
            oncorn = false;


            ClearCurrentHighlightCorn();

        }
    }

    public void Die(){
        // CharacterController ccon = GetComponent<CharacterController>();
        // ccon.enabled = false;
        // transform.position = spawnpoint.position;
        // transform.rotation = spawnpoint.rotation;
        // hasmachete = false;
        // ccon.enabled = true;
        Time.timeScale = 1f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);


    }

    public void Hit(){
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        pano.SetActive(true);
        Time.timeScale = 0f;
        if (mouseLookScript != null) mouseLookScript.enabled = false;

    }

    void jacktivation(){
        pj.SetActive(true);
        twall.SetActive(true);
        twall1.SetActive(true);
        twall2.SetActive(true);
        twall3.SetActive(true);

    }

    void ClearCurrentHighlight()
    {
        if (highlightedRenderer != null)
        {
            highlightedRenderer.material.SetColor("_EmissionColor", Color.black);
            highlightedRenderer = null;
        }
    }

        void ClearCurrentHighlightCorn()
    {
        if (highlightedRenderercorns != null)
        {
            foreach (Renderer rendy in highlightedRenderercorns)
            {
                rendy.material.SetColor("_EmissionColor", Color.black);
                highlightedRenderercorns = null;

            }
            //rendy.material.SetColor("_EmissionColor", Color.black);
        }
    }
}
