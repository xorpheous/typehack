/******************************************************************************
 * AnimateUISprite.cs does what Unity should be able to do on it's own,
 * animate a sprite on a UI canvas.  There are rather clunky ways of forcing
 * the Unity Animator to do this for a UI.Image game object, but this will 
 * work just the same but less grief and hassle.
 * 
 * The designer indicates the number of frames there are in the animation and
 * assigns a sprite to each frame in the order they're to be played.  Then
 * the designer defines the total runtime for the animation in seconds.  This
 * time is divided by the total number of frames to determine how long to 
 * wait before swapping sprites for the next frame.  
 * 
 * Lastly, there is a public method that allows the animation to be enabled
 * or disabled either through direct scripting or through a UI event such as
 * a button click.
 * 
 * J. Douglas Patterson
 * Johnson County Community College
 * dpatter@jccc.edu
 * 
 * v1.0, 01-APR-2021
 * 
 * ***************************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class AnimateUISprite : MonoBehaviour
{
    /**************************************************************************
     * * * *                  VARIABLE DECLARATIONS                    * * * */
    [SerializeField]
    [Tooltip("Ordered sprites of the animation sequence.")]
    Sprite [] animationFrames;

    [SerializeField]
    [Tooltip("Duration of the animation in seconds")]
    float animationPeriod = 1.0f;

    [SerializeField]
    [Tooltip("Defines the initial state of the animation.  Animation only plays if this value is true")]
    bool animationEnabled = true;

    float timePerFrame;     //time between frames in seconds
    float timeRemaining;    //time left in the current animation frame

    int frameIndex;         //current sprite array index

    Image thisImage;        //image component of the game object to which this script is attached.


    /**************************************************************************
     * * * *                 PARAMETER INITIALIZATIONS                 * * * */
    private void Start()
    {
        thisImage = GetComponent<Image>();                                  //Find and define the image component
        frameIndex = 0;                                                     //Set the frame index to point to the first sprite in the array

        if (animationFrames.Length > 0)                                     //Avoid divide-by-zero error
        {
            thisImage.sprite = animationFrames[frameIndex];                 //Set the initial sprite
            timePerFrame = animationPeriod / (float)animationFrames.Length; //Calculate the time between sprite swaps (frame rate)
        }
        else
        {
            thisImage.sprite = null;                                        //If the array is empty, set the sprite to null,
            timePerFrame = 6400.0f;                                         //and the per frame time to a couple of hours.
        }

        timeRemaining = timePerFrame;                                       //Initialize the countdown timer to the full time per frame value.
    }


    /**************************************************************************
     * * * *                     PERFORM ANIMATION                     * * * */
    private void FixedUpdate()
    {
        if (animationEnabled)                                               //Only loop through the sprites if the animation is enabled
        {
            timeRemaining -= Time.fixedDeltaTime;                           //Decrement the time remaining in the current animation frame
            if (timeRemaining < 0.0f)                                       //Check to see if the timer has reached zero
            {
                frameIndex += 1;                                            //Increment the sprite array index by one
                if (frameIndex == animationFrames.Length) frameIndex = 0;   //Loop back to the start if we've played the last frame in the sequence.
                thisImage.sprite = animationFrames[frameIndex];             //Load the next sprite to be displayed in the animation sequence.
                timeRemaining = timePerFrame;                               //Reset the time remaining in the frame to the full time per frame
            }
        }
    }

    /**************************************************************************
     * * * *                 ENABLE/DISABLE ANIMATION                  * * * */
    public void ToggleAnimation()
    {
        animationEnabled = !animationEnabled;                               //simply toggle the animation on and off.
    }

}
