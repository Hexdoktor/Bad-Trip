using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] PlayerMovement playerMovementScript;
    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ActiveGliding()
    {
        if (playerMovementScript.glide)
        {
          
            animator.Play("fly", 0, 0.86f);
           

        }
       

    }
}
