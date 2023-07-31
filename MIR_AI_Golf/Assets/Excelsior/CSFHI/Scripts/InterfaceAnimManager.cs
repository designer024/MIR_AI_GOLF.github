using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
[ExecuteInEditMode]
#endif

//EXCELSIOR SCRIPT
[AddComponentMenu("Excelsior/CSFHI/Interface Animation Manager")] // 'Also refered as IAM'

public class InterfaceAnimManager : MonoBehaviour {

    public List<InterfaceAnmElement> elementsList = new List<InterfaceAnmElement>();

    public bool autoStart = true; // call startAppear when GameObject is enabled
    public bool invertDelays = true; //copy appear delays of elements, and use those same delay in the reverse order for the disappear delays
    public bool cloneDelays = false; //copy appear delays of elements, and use those same delay in the same order for the disappear delays
    public float timer = 0; // incremented by on delay loop, when above an dis/appear delay, triggers the corresponding element
    public int timeBetweenLoops = 3;
    public bool testLoop = false;
    public bool autoLinearAppearDelay = true;
	
    public float appearDelay_SpeedMultiplier = 1;
    public float disappearDelay_SpeedMultiplier = 1;
    public CSFHIAnimableState currentState = CSFHIAnimableState.disappeared;
	
    public bool useDelays = true;
	
    public AudioSource audioSource;
    public AudioClip openSound;
    public AudioClip closeSound;
    public List<InterfaceAnimManager> nestedIAM = new List<InterfaceAnimManager>();
    public bool waitAppear_Nested = false;
    public bool waitDisappear_Nested = false;
    private InterfaceAnmElement waitElementFullAnim;
    void Awake() {
        this.audioSource = this.GetComponent<AudioSource>();
    }

    void Start() {
        if (Application.isPlaying) {
			//in case the asset has just been instantiated
			UpdateAnimClips();			
            startDisappear(true); // to avoid auto appear anim of elements inside (when enabled)
            if (autoStart){
               startAppear();
			}
        }
    }

    void Update() {
        if (Application.isPlaying) {
            switch (currentState) {
                /// ------------------------------------APPEARING------------------------------------------------------------
                case CSFHIAnimableState.appearing:
                    if (waitAppear_Nested && waitElementFullAnim != null) {
                        //we check if the nested interface manager (if there is one) has finished appearing
                        if (waitElementFullAnim.gameObjectRef && waitElementFullAnim.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState == CSFHIAnimableState.appeared) {
                            waitElementFullAnim = null;
                        }
                    } else {
                        foreach (InterfaceAnmElement _element in elementsList) {
                            //we set back the weight values we've temporarly set to 0 on last disappear
                            Animator animator = _element.gameObjectRef.GetComponent<Animator>();
                            if (animator) {
                                for (int layerIndex = 0; layerIndex < animator.layerCount; layerIndex++) {
                                    animator.SetLayerWeight(layerIndex, 1);
                                }
                            }

                            if (!useDelays || timer > _element.timeAppear * appearDelay_SpeedMultiplier) {
                                //we don't check by isActive, because some elements can appear and disappear just for the time of the overall apparition, and aren't meant to last after that
                                if (_element.currentState == CSFHIAnimableState.disappeared) {
                                    _element.gameObjectRef.SetActive(true);
                                    // Try to find a corresponding appear anim in the animator element
                                    //even though those appear anims can be automatically started with the setActive(true),
                                    //if we startDisappear(false), then startDisappear(true), and then startAppear(false), some elements won't be shown
                                    //so here's the fix :
                                    if (_element.animator) {
                                        foreach (AnimationClip ac in _element.animClips) {
                                            if (ac.name.ToLower().Contains("appear")) {
                                                _element.animator.Play(ac.name, -1, 0);
                                            }
                                            break;
                                        }
                                    }
                                    if (_element.isNested_IAM) {
                                        if (waitAppear_Nested) {
                                            waitElementFullAnim = _element;
                                        }
                                        _element.gameObjectRef.GetComponent<InterfaceAnimManager>().startAppear();
                                    }
                                    //should auto play the 'appear' animation since it's transition is linked to the entry point of the animator
                                    _element.currentState = CSFHIAnimableState.appearing;
                                }
                            }else{
								if (useDelays){
									//Debug.Log ("timerGlobal "+timer+"_element.timeAppear "+_element.timeAppear* appearDelay_SpeedMultiplier+"("+_element.gameObjectRef.name+")");
								}
							}
                        }
                        timer += Time.deltaTime;
                        if ((testLoop && timer > appearDelay_SpeedMultiplier + timeBetweenLoops) || (!testLoop && timer > appearDelay_SpeedMultiplier + 1)) {
                            endAppear();
                        }
                    }
                    break;
                /// ------------------------------------DISAPPEARING------------------------------------------------------------
                case CSFHIAnimableState.disappearing:
                    if (waitDisappear_Nested && waitElementFullAnim != null) {
                        //we check if the nested interface manager (if there is one) has finished appearing
                        if (waitElementFullAnim.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState == CSFHIAnimableState.disappeared) {
                            waitElementFullAnim = null;
                        }
                    } else {
                        foreach (InterfaceAnmElement _element in elementsList) {
                            if (!useDelays || timer > _element.timeDisappear * disappearDelay_SpeedMultiplier) {
                                //here, we call all the childs and ask them to disappear
                                if (_element.currentState != CSFHIAnimableState.disappearing && _element.currentState != CSFHIAnimableState.disappeared) { //we don't check by isActive, because some elements can appear and disappear just for the time of the overall apparition, and aren't meant to last after that
                                    if (_element.isNested_IAM) {
                                        if (_element.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState == CSFHIAnimableState.appeared || _element.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState == CSFHIAnimableState.appearing) {
                                            _element.gameObjectRef.GetComponent<InterfaceAnimManager>().startDisappear();
                                            _element.currentState = CSFHIAnimableState.disappearing;
                                            if (waitDisappear_Nested) {
                                                waitElementFullAnim = _element;
                                            }
                                        }
                                    } else {
                                        bool hasAnm = false;
                                        // Try to find a corresponding disappear anim in the animator element
                                        if (_element.animator) {
                                            foreach (AnimationClip ac in _element.animClips) {
                                                if (ac.name.ToLower().Contains("disappear")) {
                                                    //we take all the other layers and un-weighten them in case they've got some scale / alpha animations that go against the disappear anim
                                                    if (_element.animator) {
                                                        for (int layerIndex = 0; layerIndex < _element.animator.layerCount; layerIndex++) {
                                                            _element.animator.SetLayerWeight(layerIndex, 0);
                                                            //these values will be reset on disappear ending (when all gameObjects are disabled) and/or on next appear
                                                        }
                                                        _element.currentState = CSFHIAnimableState.disappearing;
                                                        _element.animator.Play(ac.name);
                                                        hasAnm = true;
                                                    }
                                                    break;
                                                }
                                            }
                                        }
                                        if (!hasAnm) {
                                            //Debug.LogWarning("CSFHI : "+_element.gameObjectRef.name + " hasn't got a dedicated disappear animation");
                                            _element.gameObjectRef.SetActive(false);
                                            _element.currentState = CSFHIAnimableState.disappeared;
                                        }
                                    }
                                }
                                if (!_element.isNested_IAM && _element.currentState == CSFHIAnimableState.disappearing) { //has an anim, already confirmed above
                                    //Debug.Log(_element.name + " >>> " + _element.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1);
                                    //weird status of Unity here which sometimes dives multiple layers normalizedTime at once (depending on the version)
                                    //we just have to check the the current animation playing IS the disappear one
                                    string animName = "";
                                    foreach (AnimationClip ac in _element.animClips) {
                                        if (ac.name.ToLower().Contains("disappear")) {
                                            animName = ac.name;
                                            break;
                                        }
                                    }
                                    //Debug.Log("CSFHI : ac.name : " + animName);
                                    if (_element.animator.GetCurrentAnimatorStateInfo(0).IsName(animName) && _element.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
                                        _element.currentState = CSFHIAnimableState.disappeared;
                                        _element.gameObjectRef.SetActive(false);
                                        //debug
                                        /*int totalDisabled = 0;
                                        foreach (InterfaceAnmElement _element2 in elementsList) {
                                            if (_element2.currentState == CSFHIAnimableState.disappeared) {
                                                totalDisabled++;
                                            }
                                        }
                                        Debug.Log("CSFHI : totalElementDisabled =>" + totalDisabled + "/" + elementsList.Count);
                                        */
                                    }
                                }
                            }
                        }

                        bool cancel = true;
                        foreach (InterfaceAnmElement _element2 in elementsList) {
                            if (_element2.currentState != CSFHIAnimableState.disappearing) {
                                cancel = false;
                                break;
                            }
                        }

                        if (cancel) {
                            StartCoroutine(Disable_OnDisappearEnd());
                        }

                        timer += Time.deltaTime;

                        //in case if there was nested ANM to wait, we shouldn't start closing sound before their ending
                        foreach (InterfaceAnmElement _IAM in elementsList) {
                            if (_IAM.isNested_IAM && _IAM.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState != CSFHIAnimableState.disappeared) {
                                return;
                            }
                        }
                        if (audioSource && audioSource.enabled && audioSource.clip != closeSound) {
                            audioSource.clip = closeSound;
                            audioSource.Play();
                        }

                        if ((testLoop && timer >= disappearDelay_SpeedMultiplier + timeBetweenLoops) || (!testLoop && timer >= disappearDelay_SpeedMultiplier)) {
                            //we start testing that only when the minimum time has been reached
                            foreach (InterfaceAnmElement _element in elementsList) {
                                if (_element.isNested_IAM && _element.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState != CSFHIAnimableState.disappeared) {
                                    return;
                                }
                                if (!_element.isNested_IAM && _element.currentState != CSFHIAnimableState.disappeared) {
                                    return;
                                }
                            }
                            endDisappear();
                        }
                    }
                    break;

                /// ------------------------------------APPEARED/DISAPPEARED------------------------------------------------------------
                case CSFHIAnimableState.appeared:
                case CSFHIAnimableState.disappeared:
                    //auto start next state if loop mode activated
                    if (testLoop) {
                        if (currentState == CSFHIAnimableState.appeared) {
                            startDisappear();
                        }
                        if (currentState == CSFHIAnimableState.disappeared) {
                            startAppear();
                        }
                    }
                    break;
            }
        }
    }
    public IEnumerator Disable_OnDisappearEnd() {
        int longestWait = 1;
        foreach (InterfaceAnmElement _element in elementsList) {
            if (_element.gameObjectRef.activeSelf && _element.animator != null) {
                if (longestWait < _element.animator.GetCurrentAnimatorClipInfo(0).Length) {
                    longestWait = _element.animator.GetCurrentAnimatorClipInfo(0).Length;
                }
            }
        }
        yield return new WaitForSeconds(longestWait);
        int index = 0;
        foreach (InterfaceAnmElement _elementx in elementsList) {
            index++;
            if (_elementx.isNested_IAM) {
                _elementx.gameObjectRef.GetComponent<InterfaceAnimManager>().startDisappear();
                //_elementx.gameObjectRef.SetActive(false);
            }
        }
        StopCoroutine(Disable_OnDisappearEnd());
        if (!isIAM_Root() && this.gameObject.transform.parent.GetComponent<InterfaceAnimManager>()) {
            endDisappear();
        }
    }

    public virtual void startAppear(bool direct = false) {
        //Debug.Log("startAppear " + name + " / direct: " + direct);
        //calling appear while it's disappearing is forbidden (future update might include that it fastforward the whole current disappear)
        if (this.currentState == CSFHIAnimableState.appeared)
            return;

        if (this.currentState == CSFHIAnimableState.disappearing) {
            Debug.LogWarning("CSFHI : start appear not available if disappear in progress");
            return;
        }

        if (this.currentState != CSFHIAnimableState.appearing) {
			
            foreach (InterfaceAnmElement _element in elementsList) {
                if (_element && _element.isNested_IAM) {
                    _element.gameObjectRef.GetComponent<InterfaceAnimManager>().startDisappear(true);
                    _element.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState = CSFHIAnimableState.disappeared;
                }
            }
            this.gameObject.SetActive(true);
            if (!direct && audioSource && audioSource.enabled && (this.currentState != CSFHIAnimableState.appeared && this.currentState != CSFHIAnimableState.appearing)) {
                audioSource.clip = openSound;
                audioSource.Play();
            }
            if (direct) {
                foreach (InterfaceAnmElement _element in elementsList) {
					if (_element){
                    _element.gameObjectRef.SetActive(true);
                    if (_element.isNested_IAM) {
                        _element.gameObjectRef.GetComponent<InterfaceAnimManager>().startAppear(true);
                    }
                    _element.currentState = CSFHIAnimableState.appearing;
					}
                }
                endAppear();
            } else {
                timer = 0;
                currentState = CSFHIAnimableState.appearing;
            }
        }
			/*	Debug.Log("mainState "+currentState);
			foreach (InterfaceAnmElement _element in elementsList) {
				Debug.Log("elementState "+_element.currentState);
			}*/
    }
    private void endAppear() {
        currentState = CSFHIAnimableState.appeared;
        waitElementFullAnim = null;
    }


    public virtual void startDisappear(bool direct = false) {
        //Debug.Log("CSFHI : startDisappear " + name + " / direct: " + direct);
        if (this.currentState == CSFHIAnimableState.appeared) {
            timer = 0;
        }
        if (this.currentState == CSFHIAnimableState.appearing) {
            timer = disappearDelay_SpeedMultiplier - timer;
        }
		
        if (direct || this.currentState != CSFHIAnimableState.disappearing) {
            //if we call a directDisappear OVER a notDirect Disappear, the directDisappear does override
            //if we call a disappear over an appear, the disappear does override
            if (direct) {
                foreach (InterfaceAnmElement _element in elementsList) {
					if (_element.isNested_IAM) {
						_element.gameObjectRef.GetComponent<InterfaceAnimManager>().startDisappear(true);
					}
					_element.gameObjectRef.SetActive(false);
					_element.currentState = CSFHIAnimableState.disappearing;
                }
                endDisappear();
            } else {
                if (waitAppear_Nested) {
                    //if their was appear, we remove the latest waitElementFullAnim
                    waitElementFullAnim = null;
                }
                currentState = CSFHIAnimableState.disappearing;
            }
        }
    }
    private void endDisappear() {
        //Debug.Log("CSFHI : endDisappear " + name);
        foreach (InterfaceAnmElement _element in elementsList) {
            _element.currentState = CSFHIAnimableState.disappeared;
            if (_element.isNested_IAM) {
                _element.gameObjectRef.GetComponent<InterfaceAnimManager>().currentState = CSFHIAnimableState.disappeared;
            }
            _element.gameObjectRef.SetActive(false);
        }
        waitElementFullAnim = null;
        StopCoroutine(Disable_OnDisappearEnd());
        currentState = CSFHIAnimableState.disappeared;
        if (!isIAM_Root()) { // if is nested IAM
            this.gameObject.SetActive(false);

            //+ if this nested IAM is the latest element to disappear of the ROOT, we use it to declare the endDisappear of the parent
            //because that element will usually be at delay 0, we need to launch endDisappear even after the original timer
            InterfaceAnimManager _parentIAM = this.transform.parent.GetComponent<InterfaceAnimManager>();
            if (_parentIAM.currentState==CSFHIAnimableState.disappearing){

                //we check that the parent is the root
                if (_parentIAM.transform.parent) {
                    if (_parentIAM.transform.parent.GetComponent<InterfaceAnimManager>()) {
                        return;
                    }
                }

                int index = 0;
                foreach (InterfaceAnmElement _element in _parentIAM.elementsList) {
                    if (_element.gameObjectRef == this.gameObject) {
                        if (index == 0) {
                            _parentIAM.endDisappear();
                        }
                        return;
                    }
                    index++;
                }
            }
        }
    }
    public void UpdateAnimClips() {
       // Debug.Log("data recomputed");
        bool addElement = true;
        int newSortID = 0;
        foreach (Transform child in this.gameObject.transform) {
            //only the childs of this parent (not the nested AIM)
            if (child.parent == this.gameObject.transform) {
                addElement = true;
                //foreach loop won't work since we remove elements, so we go trough a inverted for loop
                for (int a = this.elementsList.Count - 1; a >= 0; a--) {
                    //Debug.Log(_target.gameObject.transform.name);
                    //Debug.Log(_target.elementsList[a].gameObjectRef.transform.parent.name);
                    if (this.elementsList[a] == null || this.elementsList[a].gameObjectRef == null || this.elementsList[a].gameObjectRef.transform.parent != this.gameObject.transform) {
                        //the element has been removed or moved to an other interface, we remove it from the list
                        if (this.elementsList[a] != null) {
                            this.elementsList[a].Delete();
                        }
                        this.elementsList.Remove(this.elementsList[a]);
                    } else {
                        //if the element is in the list, we create an interfaceAnmElement so that the manager can handle it
                        if (this.elementsList[a].gameObjectRef == child.gameObject) {
                            addElement = false;
                            this.elementsList[a].sortID = newSortID;
                            break;
                        }
                    }
                }
                if (addElement) {
                    InterfaceAnmElement ia;
                    if (child.gameObject.GetComponent<InterfaceAnimManager>()) {
                        ia = InterfaceAnmElement.Create(child.gameObject, newSortID, true);
                    } else {
                        ia = InterfaceAnmElement.Create(child.gameObject, newSortID, false);
                    }
                    this.elementsList.Add(ia);
                }
                newSortID++;
            }
        }
        //update order if an element position has changed in hierarchy
        this.elementsList = this.elementsList.OrderBy(x => x.sortID).ToList();
        if (!isIAM_Root()) {
            foreach (InterfaceAnimManager _IAM in this.nestedIAM) {
                _IAM.autoStart = this.autoStart;
                _IAM.useDelays = this.useDelays;
                _IAM.timeBetweenLoops = this.timeBetweenLoops;
                _IAM.waitAppear_Nested = this.waitAppear_Nested;
                _IAM.waitDisappear_Nested = this.waitDisappear_Nested;
            }
        } else {
            //force on all the children directly :
            this.nestedIAM = this.GetComponentsInChildren<InterfaceAnimManager>(true).ToList<InterfaceAnimManager>();
            this.nestedIAM.Remove(this);
            foreach (InterfaceAnimManager _IAM in this.nestedIAM) {
                _IAM.autoStart = this.autoStart;
                _IAM.useDelays = this.useDelays;
                _IAM.timeBetweenLoops = this.timeBetweenLoops;
                _IAM.waitAppear_Nested = this.waitAppear_Nested;
                _IAM.waitDisappear_Nested = this.waitDisappear_Nested;
            }
        }

        foreach (InterfaceAnmElement _element in this.elementsList) {
            if (_element) {//in case it has been destroyed in the meantime
                _element.UpdateProperties();
            }
        }

        if (isIAM_Root()) {
            int index = 0;
            //we need all the IAM to have a different name to avoid errors
            foreach (InterfaceAnimManager _IAM in this.nestedIAM) {
                if (_IAM.gameObject.GetComponent<InterfaceAnimManager>()) {
                    _IAM.gameObject.name = "Nested_IAM[" + index + "]";
                }
                index++;
            }
            foreach (InterfaceAnimManager _IAM in this.nestedIAM) {
                _IAM.UpdateAnimClips();
            }
        }
    }
    public bool isIAM_Root(){
        if (this.transform.parent && this.transform.parent.GetComponent<InterfaceAnimManager>()) {
            return false;
        }else{
            return true;
        }
    }
}
