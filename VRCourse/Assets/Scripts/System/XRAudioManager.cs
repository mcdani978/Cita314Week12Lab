using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class XRAudioManager : MonoBehaviour
{
    [Header("Progress Control")]

    [SerializeField]
    ProgressControl progressControl;

    [SerializeField]
    AudioSource progressSound;

    [SerializeField]
    AudioClip startGameClip;

    [SerializeField]
    AudioClip challengeCompleteClip;

    [Header("Grab Interactables")]

    [SerializeField]
    XRGrabInteractable[] grabInteractables;

    [SerializeField]
    AudioSource grabSound;

    [SerializeField]
    AudioSource activatedSound;
    
    [SerializeField]
    AudioClip grabClip;

    [SerializeField]
    AudioClip keyClip;

    [SerializeField]
    AudioClip grabActivatedClip;

    [SerializeField] 
    AudioClip wandActivatedClip;


    [Header("Drawer Interactables")]

    [SerializeField]
    DrawerInteractable drawer;

    XRPhysicsButtonInteractable drawerPhysicsButton;
    bool isDetatched;
    XRSocketInteractor drawerSocket;
    AudioSource drawerSound;
    AudioSource drawerSocketSound;
    AudioClip drawerMoveClip;
    AudioClip drawerSocketClip;


    [Header("Hinge Interactables")]

    [SerializeField]
    SimpleHingeInteractable[] cabinetDoors = new SimpleHingeInteractable[2];

    AudioSource[] cabinetDoorSound;
    AudioClip cabinetDoorMoveClip;


    [Header("Combo Lock")]

    [SerializeField]
    CombinationLock comboLock;

    AudioSource comboLockSound;
    AudioClip lockComboClip;
    AudioClip unlockComboClip;
    AudioClip comboButtonPressedClip;


    [Header("The Wall")]

    [SerializeField]
    TheWall wall;

    [SerializeField]
    AudioSource wallSound;

    XRSocketInteractor wallSocket;
    AudioSource wallSocketSound;
    AudioClip destoryWallClip;
    AudioClip wallSocketClip;


    [Header("Local Audio Settings")]

    [SerializeField]
    AudioSource backgroundMusic;
    [SerializeField]
    AudioClip backgroundMusicClip;
    [SerializeField]
    AudioClip fallBackClip;
    const string FALL_BACK_CLIP = "fallBackClip";
    bool startAudio;

    private void OnEnable()
    {
        //Check if there's a fallback clip
        if (fallBackClip == null)
        {
            fallBackClip = AudioClip.Create(FALL_BACK_CLIP, 1, 1, 1000, true);
        }

        //Progress Control
        if (progressControl != null)
        {
            progressControl.OnStartGame.AddListener(StartGame);
            progressControl.OnChallengeComplete.AddListener(ChallengeComplete);
        }

        //Grabbable Objects
        SetGrabbables();

        //Drawer
        if (drawer != null)
        {
            SetDrawerInteractable();
        }

        //Wall
        if (wall != null)
        {
            SetWall();
        }

        //Cabinet Doors
        cabinetDoorSound = new AudioSource[cabinetDoors.Length];
        for (int i = 0; i < cabinetDoors.Length; ++i)
        {
            if (cabinetDoors[i] != null)
            {
                SetCabinetDoors(i);
            }
        }

        //Combo Lock
        if (comboLock!= null)
        {
            SetComboLock();
        }

    }


    private void ChallengeComplete(string arg0)
    {
        if (progressSound != null && challengeCompleteClip != null)
        {
            progressSound.clip = challengeCompleteClip;
            progressSound.Play();
        }
    }

    private void StartGame(string arg0)
    {
        if (!startAudio)
        {
            startAudio = true;
            if (backgroundMusic != null && backgroundMusicClip != null)
            {
                backgroundMusic.clip = backgroundMusicClip;
                backgroundMusic.Play();
            }
        }
        else
        {
            if (progressSound != null && startGameClip != null)
            {
                progressSound.clip = startGameClip;
                progressSound.Play();
            }
        }
    }

    private void OnDisable()
    {
        if (progressControl != null)
        {
            progressControl.OnStartGame.RemoveListener(StartGame);
            progressControl.OnChallengeComplete.RemoveListener(ChallengeComplete);
        }

        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }

        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.RemoveListener(OnSelectEnteredGrabbable);
            grabInteractables[i].selectExited.RemoveListener(OnSelectExitedGrabbable);
            grabInteractables[i].activated.RemoveListener(OnActivatedGrabbable);
        }

        if (drawer != null)
        {
            drawer.selectEntered.RemoveListener(OnDrawerMove);
            drawer.selectExited.RemoveListener(OnDrawerStop);
            drawer.OnDrawerDetatch.RemoveListener(OnDrawerDetatch);
        }

        if (comboLock != null)
        {

            comboLock.UnlockAction -= OnComboUnlocked;
            comboLock.LockAction -= OnComboLocked;
            comboLock.ComboButtonPressed -= OnComboButtonPressed;

        }
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            cabinetDoors[i].OnHingeSelected.RemoveListener(OnDoorMove);
            cabinetDoors[i].selectExited.RemoveListener(OnDoorStop);
        }

        if (wall != null)
        {
            wall.OnDestroy.RemoveListener(OnDestroyWall);
        }

        if (wallSocket != null)
        {
            wallSocket.selectEntered.RemoveListener(OnWallSocketed);
        }
    }

    void SetCabinetDoors(int index)
    {
        cabinetDoorSound[index] = cabinetDoors[index].transform.AddComponent<AudioSource>();
        cabinetDoorMoveClip = cabinetDoors[index].GetHingeMoveClip;
        CheckIfClipIsNull(ref cabinetDoorMoveClip);
        cabinetDoorSound[index].clip = cabinetDoorMoveClip;

        cabinetDoors[index].OnHingeSelected.AddListener(OnDoorMove);
        cabinetDoors[index].selectExited.AddListener(OnDoorStop);
    }

    void SetComboLock()
    {
        comboLockSound = comboLock.transform.AddComponent<AudioSource>();
        lockComboClip = comboLock.GetLockClip;
        CheckIfClipIsNull (ref lockComboClip);
        unlockComboClip = comboLock.GetUnlockClip;
        CheckIfClipIsNull (ref unlockComboClip);
        comboButtonPressedClip = comboLock.GetComboPressedClip;
        CheckIfClipIsNull(ref  comboButtonPressedClip);

        comboLock.UnlockAction += OnComboUnlocked;
        comboLock.LockAction += OnComboLocked;
        comboLock.ComboButtonPressed += OnComboButtonPressed;
    }

    private void OnComboButtonPressed()
    {
        comboLockSound.clip = comboButtonPressedClip;
        comboLockSound.Play();
    }

    private void OnComboLocked()
    {
        comboLockSound.clip = lockComboClip;
        comboLockSound.Play();
    }

    private void OnComboUnlocked()
    {
        comboLockSound.clip = unlockComboClip;
        comboLockSound.Play();
    }

    private void OnDoorStop(SelectExitEventArgs arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0.interactableObject == cabinetDoors[i])
            {
                cabinetDoorSound[i].Stop();
            }
        }
    }

    private void OnDoorMove(SimpleHingeInteractable arg0)
    {
        for (int i = 0; i < cabinetDoors.Length; i++)
        {
            if (arg0 == cabinetDoors[i])
            {
                cabinetDoorSound[i].Play();
            }
        }
    }

    private void OnSelectEnteredGrabbable(SelectEnterEventArgs arg0)
    {
        if (arg0.interactableObject.transform.CompareTag("Key"))
        {
            PlayGrabSound(keyClip);
        }
        else
        {
            PlayGrabSound(grabClip);
        }

        grabSound.Play();
    }

    private void OnSelectExitedGrabbable(SelectExitEventArgs arg0)
    {
        PlayGrabSound(grabClip);
    }

    private void OnActivatedGrabbable(ActivateEventArgs arg0)
    {
        GameObject tempGameObj = arg0.interactableObject.transform.gameObject;

        if (tempGameObj.GetComponent<WandControl>() != null)
        {
            activatedSound.clip = wandActivatedClip;
        }
        else
        {
            activatedSound.clip = grabActivatedClip;
        }

        activatedSound.Play();
    }

    private void OnDestroyWall()
    {
        //Play audio source
        if (wallSound != null)
        {
            wallSound.Play();
        }
    }

    private void OnDrawerStop(SelectExitEventArgs arg0)
    {
        drawerSound.Stop();
    }

    private void OnDrawerMove(SelectEnterEventArgs arg0)
    {
        //if(drawerRb.velocity.magnitude > 0 {drawerSound.Play();})
        //else {drawerSound.Stop();}
        if (isDetatched)
        {
            PlayGrabSound(grabClip);
        }
        else
        {
            drawerSound.Play();
        }
    }
    void SetGrabbables()
    {
        //Find the grabbable objects
        grabInteractables = FindObjectsByType<XRGrabInteractable>(FindObjectsSortMode.None);

        //Loop through each grabbable object & add set listeners
        for (int i = 0; i < grabInteractables.Length; i++)
        {
            grabInteractables[i].selectEntered.AddListener(OnSelectEnteredGrabbable);
            grabInteractables[i].selectExited.AddListener(OnSelectExitedGrabbable);
            grabInteractables[i].activated.AddListener(OnActivatedGrabbable);
        }
    }

    private void SetWall()
    {
        destoryWallClip = wall.GetDestroyClip;
        CheckIfClipIsNull(ref destoryWallClip);
        wallSound = wall.transform.AddComponent<AudioSource>();
        wallSound.clip = destoryWallClip;
        wall.OnDestroy.AddListener(OnDestroyWall);

        wallSocket = wall.GetWallSocket;
        if (wallSocket != null)
        {
            wallSocketSound = wallSocket.transform.AddComponent<AudioSource>();
            wallSocketClip = wall.GetSocketClip;
            CheckIfClipIsNull(ref wallSocketClip);
            wallSocketSound.clip = wallSocketClip;
            wallSocket.selectEntered.AddListener(OnWallSocketed);
        }
    }

    private void OnWallSocketed(SelectEnterEventArgs arg0)
    {
        wallSocketSound.Play();
    }

    private void SetDrawerInteractable()
    {
        //Set up audio source
        drawerSound = drawer.transform.AddComponent<AudioSource>();
        drawerMoveClip = drawer.GetMoveClip;
        CheckIfClipIsNull(ref drawerMoveClip);

        drawerSound.clip = drawerMoveClip;
        drawerSound.loop = true;

        //When the drawer is grabbed, play the sound. When the drawer is not grabbed, stop playing the sound
        drawer.selectEntered.AddListener(OnDrawerMove);
        drawer.selectExited.AddListener(OnDrawerStop);

        drawer.OnDrawerDetatch.AddListener(OnDrawerDetatch);

        drawerSocket = drawer.GetSocketIntractor;
        if (drawerSocket != null)
        {

            drawerSocketSound = drawerSocket.transform.AddComponent<AudioSource>();
            drawerSocket.selectEntered.AddListener(OnDrawerSocketed);
            drawerSocketClip = drawer.GetSocketedClip;
            CheckIfClipIsNull(ref drawerSocketClip);
            drawerSocketSound.clip = drawerSocketClip;

        }
        drawerPhysicsButton = drawer.GetPhysicsButtonInteractable;
        if (drawerPhysicsButton != null)
        {
            drawerPhysicsButton.OnBaseEnter.AddListener(OnPhysicsButtonEnter);
            drawerPhysicsButton.OnBaseExit.AddListener(OnPhysicsButtonExit);
        }
    }

    private void OnPhysicsButtonExit()
    {
        PlayGrabSound(keyClip);
    }

    private void OnPhysicsButtonEnter()
    {
        PlayGrabSound(keyClip);
    }

    private void OnDrawerDetatch()
    {
        isDetatched = true;
        drawerSound.Stop();
    }

    private void OnDrawerSocketed(SelectEnterEventArgs arg0)
    {
        drawerSocketSound.Play();
    }

    /*
     * Using ref in the parameter passes the exact reference of the given argument. Without it,
     * changing the reference inside the function will not chage the reference outside the function.
     * The parameter is referencing the same object in memory as the passed argument reference, but they
     * are different references. Using ref means passing the exact reference to the function, meaning
     * changes to the parameter reference will change the argument's reference too.
     * 
     * This also means that when you call the function, you need to add ref before passing the argument
     * (CheckIfClipIsNull(ref myAudioClip))
     */
    void PlayGrabSound(AudioClip clip)
    {
        grabSound.clip = clip;
        grabSound.Play();
    }

    void CheckIfClipIsNull(ref AudioClip clip)
    {
        if (clip == null)
        {
            clip = fallBackClip;
        }
    }
}
