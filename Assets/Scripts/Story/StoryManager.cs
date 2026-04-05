using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Behavior;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class StoryManager : MonoBehaviour
{
    #region UTILITIES AND FUNCTIONS

    public static StoryManager Instance;

    public bool debugMode = false;

    [Header("Story")]
    public int objectiveStage = -1;
    public List<Objective> objectives;
    public Objective activeObj => objectives[objectiveStage];


    public string spawnPointID;
    public Animator transitionAnimator;

    private HashSet<StoryFlag> activeFlags = new HashSet<StoryFlag>();
    



    // CHARACTER CONFIGURATION
    // You, the main character
    public Character mainCharacter = new Character() { name = "You", speechColor = Color.lightBlue };
        // Friend Michael
    public Character michael = new Character() { name = "Michael", speechColor = Color.yellowNice };
    // killer
    public Character killer = new Character() { name = "???", speechColor = Color.softRed };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Check single instance and destroy if exists already
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);


        CreateObjectives();

        StartCoroutine(Storyline());
    }

    public void advanceTask(string taskID)
    {
        Task task = activeObj.tasks.FirstOrDefault(t => t.id == taskID);
        if (task == null)
        {
            Debug.LogError("ERROR: Couldn't find Task to complete in advanceTask();");
        }
        task.complete = true;
        ObjectiveManager.Instance.completeTask(task.text);
    }

    public void advanceObjective()
    {
        objectiveStage++;

        Objective obj = activeObj;
        Task[] tasks = activeObj.tasks.ToArray();

        ObjectiveManager.Instance.setObjective(obj.title, tasks);
    }

    public bool checkTaskCompletion()
    {
        bool tasksCompleted = false;
        foreach (Task t in activeObj.tasks)
        {
            tasksCompleted = t.complete;
            if (!tasksCompleted) break;
        }

        return tasksCompleted;
    }

    public bool HasFlag(StoryFlag flag)
    {
        return activeFlags.Contains(flag);
    }

    public void AddFlag(StoryFlag flag)
    {
        if (activeFlags.Add(flag))
            Debug.Log($"Added flag {flag}");
    }

    public IEnumerator TransitionLoadScene(string sceneName, string spawnID)
    {
        PlayerController.Instance.freeze = true;
        transitionAnimator.SetBool("loading", true);

        yield return new WaitForSeconds(1f);

        spawnPointID = spawnID;
        yield return SceneManager.LoadSceneAsync(sceneName);

        PlayerController.Instance.freeze = false;
        transitionAnimator.SetBool("loading", false);
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator MovePlayerToPosition(Vector3 targetPos, Quaternion targetRot, Quaternion targetCamRot, float duration)
    {
        Vector3 startPos = PlayerController.Instance.transform.position;
        Quaternion startRot = PlayerController.Instance.transform.localRotation;
        Quaternion startCamRot = PlayerController.Instance.playerCamera.transform.localRotation;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);

            PlayerController.Instance.transform.position = Vector3.Lerp(startPos, targetPos, t);
            PlayerController.Instance.transform.localRotation = Quaternion.Slerp(startRot, targetRot, t);
            PlayerController.Instance.playerCamera.transform.localRotation = Quaternion.Slerp(startCamRot, targetCamRot, t);

            yield return null;
        }


        PlayerController.Instance.transform.position = targetPos;
        PlayerController.Instance.transform.localRotation = targetRot;
        PlayerController.Instance.playerCamera.transform.localRotation = targetCamRot;
    }

    #endregion

    #region OBJECTIVES

    private void CreateObjectives()
    {
        objectives.Add(new Objective
        {
            stageID = 0,
            title = "Reheat dinner",
            tasks = new Task[]
            {
                new Task {id = "fridge", text = "Open the fridge"},
                new Task {id = "food", text = "Pick up the leftovers"},
                new Task {id = "heat", text = "Heat up the leftovers"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 1,
            title = "Late night call",
            tasks = new Task[]
            {
                new Task {id = "phone", text = "Answer the phone"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 2,
            title = "Get Ready",
            tasks = new Task[]
            {
                new Task {id = "flashlight", text = "Pack Flashlight in Trunk"},
                new Task {id = "axe", text = "Pack Axe in Trunk"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 3,
            title = "Drive to the arcade",
            tasks = new Task[]
            {
                new Task {id = "leave", text = "Get in the car"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 4,
            title = "Inspect the area",
            tasks = new Task[]
            {
                new Task {id = "arclook", text = "Look around the arcade"},
                new Task {id = "carinspect", text = "Inspect the car"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 5,
            title = "Try to get in",
            tasks = new Task[]
            {
                new Task {id = "inspdoor", text = "Inspect the front door"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 6,
            title = "Prepare to enter",
            tasks = new Task[]
            {
                new Task {id = "getlight", text = "Grab flashlight"},
                new Task {id = "getaxe", text = "Grab axe"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 7,
            title = "Enter the arcade",
            tasks = new Task[]
            {
                new Task {id = "breakchain", text = "Break the chain"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 8,
            title = "Explore the arcade",
            tasks = new Task[] {}
        });
        objectives.Add(new Objective
        {
            stageID = 9,
            title = "Mysterious Room",
            tasks = new Task[]
            {
                new Task {id = "MRgetin", text = "Find a way in"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 10,
            title = "Staff Room",
            tasks = new Task[]
            {
                new Task {id = "staffInspect", text = "Inspect the room"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 11,
            title = "Mysterious Room",
            tasks = new Task[]
            {
                new Task {id = "MRkey", text = "Try the key"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 12,
            title = "Unknown Scream",
            tasks = new Task[]
            {
                new Task {id = "smic", text = "Save Michael"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 13,
            title = "Maintenance Room - Investigate",
            tasks = new Task[]
            {
                new Task {id = "mrinv", text = "Search the room"}
            }
        });
        objectives.Add(new Objective
        {
            stageID = 14,
            title = "Find the source of the scream",
            tasks = new Task[]
            {
                new Task {id = "ckdoor", text = "Check the door at the back"}
            }
        });
    }

    #endregion


    #region STORYLINE
    // THE STORYLINE CONTROLS THE FLOW OF THE STORY AND ORDER OF SEQUENCES 
    private IEnumerator Storyline()
    {
        /*
         * ====================================================================
         * ====================================================================
         *                      HOME SCENE
         * ====================================================================
         * ====================================================================
         */



        // INTRO SEQUENCE START

        // 5 Second delay for player to prepare
        if (!debugMode) yield return new WaitForSeconds(5f);
        // DEBUG MODE 1 SECOND
        else yield return new WaitForSeconds(1f);

        if (!debugMode)
        {

            // Intro monologue, food objective
            yield return IntroSequence();

            ObjectiveManager.Instance.hideObjective();

            // Wait 30 seconds while the food is heating up
            if (!debugMode) yield return new WaitForSeconds(30f);
            // DEBUG MODE 3 SECONDS
            else yield return new WaitForSeconds(3f);

            // INTRO SEQUENCE END



            // PHONE SEQUENCE START

            // Phone starts ringing
            yield return PhoneRing();

            ObjectiveManager.Instance.hideObjective();

            // Phone dialogue sequence
            yield return PhoneSequence();

            // PHONE SEQUENCE END



            // PREPERATION SEQUENCE STARTED
            // WAITING FOR PLAYER TO PREPARE ALL ITEMS AND GET IN THE CAR AND LEAVE!
            yield return new WaitUntil(() => checkTaskCompletion());

            advanceObjective();
            // Await for player to get in the car

            yield return new WaitUntil(() => checkTaskCompletion());
            ObjectiveManager.Instance.hideObjective();

            // SCREEN FADES OUT AND PLAYS AUDIO OF DOOR CLOSE AND CAR LEAVING
        }
        PlayerController.Instance.freeze = true;

        transitionAnimator.SetBool("loading", true);
        yield return new WaitForSeconds(1f);

        GameObject.Find("Car_Leave").GetComponent<ObjectAudio>().PlaySound();

        if (!debugMode) yield return new WaitForSeconds(15f);

        /* ====================================================================
         *                      HOME SCENE OVER
           ====================================================================*/

        spawnPointID = "car";
        yield return SceneManager.LoadSceneAsync("Drive");


        /*
         * ====================================================================
         * ====================================================================
         *                      DRIVING SCENE
         * ====================================================================
         * ====================================================================
         */

        PlayerController.Instance.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        PlayerController.Instance.transform.SetParent(GameObject.Find("PlayerLock").transform, true);

        transitionAnimator.SetBool("loading", false);
        yield return new WaitForSeconds(1f);

        if(!debugMode) yield return DriveSequence();

        transitionAnimator.SetBool("loading", true);
        yield return new WaitForSeconds(1f);
        PlayerController.Instance.transform.SetParent(null);
        DontDestroyOnLoad(PlayerController.Instance);

        /* ====================================================================
                                  DRIVING SCENE OVER
           ====================================================================*/

        spawnPointID = "arcade_arrive";
        yield return SceneManager.LoadSceneAsync("Arcade_Exterior");
        // testing
        if (debugMode) objectiveStage = 3;
        
        PlayerController.Instance.freeze = false;

        /*
         * ====================================================================
         * ====================================================================
         *                      ARCADE EXTERIOR SCENE
         * ====================================================================
         * ====================================================================
         */

        transitionAnimator.SetBool("loading", false);
        yield return new WaitForSeconds(1f);
        // Finished loading


        yield return new WaitForSeconds(2f);
        if (!debugMode) yield return ExteriorSequence();

        ObjectiveManager.Instance.hideObjective();

        transitionAnimator.SetBool("loading", true);
        yield return new WaitForSeconds(1f);

        /* ====================================================================
                                  ARCADE EXTERIOR SCENE OVER
           ====================================================================*/

        spawnPointID = "arcade";
        yield return SceneManager.LoadSceneAsync("Arcade_Interior");

        PlayerController.Instance.freeze = false;

        /*
         * ====================================================================
         * ====================================================================
         *                      ARCADE INTERIOR SCENE
         * ====================================================================
         * ====================================================================
         */

        transitionAnimator.SetBool("loading", false);
        yield return new WaitForSeconds(1f);
        // Finished loading

        objectiveStage = 7;

        yield return InteriorSequence();


    }

    #endregion

    #region STORY SEQUENCES & DIALOGUES/MONOLOGUES

    // START OF GAME - PLAYER ARRIVED HOME
    IEnumerator IntroSequence()
    {
        // Play the monologue, player has arrived home, hungry
        yield return PlayIntroMonologue();

        // Advance objective, starts the obhjective
        advanceObjective();

        // Await till the objective has been completed
        yield return new WaitUntil(() => checkTaskCompletion());

        // Start microwave sound
        GameObject.Find("Microwave").GetComponent<ObjectAudio>().PlaySound();
    }

    // Player intro monologue
    IEnumerator PlayIntroMonologue()
    {

        Speech introMono = new Speech()
        {
            title = mainCharacter.name,
            color = mainCharacter.speechColor,
            speeches = new string[] { "Finaly home...", "Today was such a loooong day.", "Man, I hate work, it's so boring.", "Well, at least I make a decent living.", "I am sooo hungry, I think there are some leftovers in the fridge.", "I should heat them up." }
        };

        Speech[] mono = new Speech[] { introMono };

        // skip monologue in debug
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono);
    }

    IEnumerator PhoneRing()
    {
        GameObject.Find("Telephone").GetComponent<ObjectAudio>().PlayLoop();

        yield return new WaitForSeconds(3f);

        advanceObjective();

        yield return new WaitUntil(() => checkTaskCompletion());

        GameObject.Find("Telephone").GetComponent<ObjectAudio>().StopSound();
    }

    IEnumerator PhoneSequence()
    {
        Speech[] dialog = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "Hello?", "Who's there?" }},
            new Speech() {title = "???", color = michael.speechColor, speeches = new string[] { "Hey there...", "We haven't spoken in a long time." }},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "Excuse me?", "Who is this?" }},
            new Speech() {title = "???", color = michael.speechColor, speeches = new string[] { "It's Michael! Don't you remember my voice?" }},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "No, I don't recognise it.", "Are you Michael back from high school?" }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "Of course...", "How many Michaels do you know?" }},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "Well actually-" }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "Whatever.", "Anyways...", "You might be wondering why I'm calling you in these late hours." }},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "Yes I am. I'm starving..." }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "Okay, well, hear me out...", "Do you remember the times during high school how we used to go exploring abandoned buildings around town?" }},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "Yeah, I remember.", "One time we got attacked by a drunk hobo in that old post office... That wasn't very fun." }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "I know... It wasn't the safest activity we could have done...", "But hear me out-" }},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "Don't tell me you want to..." }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "Listen... this might sound crazy... but...", "There's this arcade on the edge of town by the forest...", "You remember it? We used to spend a ton of time there!"}},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "Yes... We also spent a fortune there. My parents didn't really like it." }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "Look, that doesn't matter right now.", "The thing is, the arcade has been closed since the 90's...", "I went around there recently and it looks in pretty good shape.", "The door had a chain on it, so I assume the interior could be intact.", "Imagine how cool it would be to explore the place and see all the games we used to play!", "Some of them might still work... I wonder if my Rocketron record still prevails..."}},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "I don't know if this is a good idea.", "It sounds cool, but if the place is locked up, don't you think there might be a security guard? Or cameras?" }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "Maybe. But we can handle a guard. And I don't think there were cameras back when we used to go there.", "And remembering the rumours about financial issues the arcade had towards it's end, I doubt they had the cash to set them up.", "Let's hope there won't be any killer robots, like there were at that one pizza place we heard of."}},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "Don't even remind me of that." }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "Anyways, it's getting late now. Either you're coming with now, or I'm going alone."}},
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] {"You want to go right now?", "We should think this through more..." }},
            new Speech() {title = michael.name, color = michael.speechColor, speeches = new string[] { "I'm gonna grab some stuff and head over there soon.", "You remember the spot? Don't be late or I'm going in without you.", "Don't let me down man... Please..." }},
        };

        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(dialog);

        GameObject.Find("TelEnd").GetComponent<ObjectAudio>().PlaySound();

        Speech[] mono1 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "Wait! Hello?",
                "Could have at least said bye."
            }},
        };
        yield return DialogueManager.Instance.PlayDialogue(mono1);

        GameObject.Find("TelEnd").GetComponent<ObjectAudio>().StopSound();
        GameObject.Find("TelHang").GetComponent<ObjectAudio>().PlaySound();



        Speech[] mono2 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "It's been so long since I've done something like this.",
                "I kinda regreted doing it back then... I have a job now and don't really want to get into any trouble.",
                "But on the other hand, my life has been very boring since then.",
                "This probably isn't very safe, but I miss the adrenaline of exploring abandoned buildings.",
                "A little bit of nostalgia won't hurt... and if Mike is already going, I might aswell tag along.",
                "If anything goes south, I'll just leave him there...",
                "I should throw some tools in the car before leaving.",
                "I don't have a crowbar, but there should be an axe laying around here somewhere...",
                "Oh, and can't forget a flashlight!"
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono2);

        advanceObjective();
    }

    IEnumerator DriveSequence()
    {
        Speech[] mono = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "Why am I doing this?",
                "Am I going to regret this?",
                "Michael hasn’t changed.",
                "Still impulsive. Still dragging me into stupid ideas.",
                "Thought I'd never see him again... funny how one phone call can undo years of being responsible.",
                "The arcade...",
                "Haven’t thought about that place in ages.",
                "That constant buzzing from the machines, the flashing neon lights... I'll never forget it.",
                "It felt like a second home back then.",
                "I miss the times when nothing mattered.",
                "If the place is truly abandoned, why hasn't anyone stripped it yet?",
                "Old machines, old wiring… someone would’ve taken it all by now.",
                "Unless there’s a reason they didn’t.",
                "I keep telling myself this is just nostalgia talking.",
                "That this is no different from any of those buildings we explored back then.",
                "Still… something feels off.",
                "My hands are tighter on the steering wheel than they should be...",
                "Whatever.",
                "It’s probably just the stress of not doing this in so long.",
                "Hopefully Michael’s already there. Waiting for me.",
                "And yet…",
                "I can’t quite overcome this feeling of...",
                "Fear..."

            }},
        };
        yield return DialogueManager.Instance.PlayDialogue(mono);
    }

    IEnumerator ExteriorSequence()
    {
        Speech[] mono = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "That trip felt longer than it should have...",
                "Where's Michael? I was expecting him to be here already.",
                "I should look around a bit.",
                "There's a car over there. Maybe he's waiting inside."
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono);

        advanceObjective();

        if (!debugMode) yield return new WaitUntil(() => checkTaskCompletion());

        Speech[] mono2 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "I'll try the door, maybe he's inside already."
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono2);

        advanceObjective();

        yield return new WaitUntil(() => checkTaskCompletion());

        Speech[] mono3 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "That's odd... the door is still chained up.",
                "Michael was always a creative guy, he probably found a different way in.",
                "Unless he wasn't here at all...",
                "That's stupid. Of course he's inside already, probably trying to get the machines running.",
                "I'll try bust through the door, maybe I'll scare him. Heh."
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono3);

        advanceObjective();

        yield return new WaitUntil(() => checkTaskCompletion());

        advanceObjective();

        yield return new WaitUntil(() => checkTaskCompletion());
    }


    // Interior sequence
    IEnumerator InteriorSequence()
    {
        yield return new WaitForSeconds(1f);

        Speech[] mono = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "Wow, I haven't been here in so long... It has barely changed.",
                "I remember how you could redeem your tickets for prizes at the desk here.",
                "Looks like the power still works in some spots. Most of the wiring has probably burnt out by now.",
                "Time to look around..."
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono);

        advanceObjective();

        yield return new WaitUntil(() =>
            HasFlag(StoryFlag.exploreArcade) &&
            HasFlag(StoryFlag.talkMachines) &&
            HasFlag(StoryFlag.talkMR)
        );

        advanceObjective();

        yield return new WaitUntil(() => HasFlag(StoryFlag.enterStaff));

        Speech[] mono2 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "A staff room... so this is why the employees spent so much time in here.",
                "Maybe I can find something useful in here."
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono2);
        AddFlag(StoryFlag.talkStaff);

        advanceObjective();

        yield return new WaitUntil(() => HasFlag(StoryFlag.readPaper));

        Speech[] mono5 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "Could this be why the arcade shut down?"
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono5);

        yield return new WaitUntil(() => checkTaskCompletion());
        ObjectiveManager.Instance.hideObjective();

        Speech[] mono3 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "Maybe this key belongs to the room..."
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono3);

        advanceObjective();

        yield return new WaitUntil(() => HasFlag(StoryFlag.screamWalk));

        GameObject.Find("Scream").GetComponent<ObjectAudio>().PlaySound();

        yield return new WaitForSeconds(2.5f);

        Speech[] mono4 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "W-was that Michael?",
                "Oh god... I need to go help him."
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono4);

        // save michael task 12
        advanceObjective();

        // player enters the maintenance room
        yield return new WaitUntil(() => checkTaskCompletion());
        ObjectiveManager.Instance.hideObjective();

        Speech[] mono6 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "This must be the maintenance room... tools and machines everywhere. Looks like it hasn't been touched in years.",
                "That scream... it came from somewhere close. Michael, where are you?"
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono6);

        // investigate mr task 13
        advanceObjective();

        // wait for player to read notebook
        yield return new WaitUntil(() => HasFlag(StoryFlag.readNotebook));

        Speech[] mono7 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "Someone worked here... December 1986. They were scared of the owner.",
                "A blood stain on a machine... a weird smell from the locked room at the back...",
                "'I've never been back there, nor have I seen anyone go in there... there's always this weird warmth...'",
                "That's... unsettling. Whatever was happening here, it wasn't normal."
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono7);

        yield return new WaitForSeconds(2f);

        yield return new WaitUntil(() => HasFlag(StoryFlag.exploreMR));

        Speech[] mono8 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "Wait... The smell and the locked room. The scream... It must have come from...",
                "that room...",
                "...",
                "I need to get in there and help him!"
            }},
        };
        if (!debugMode) yield return DialogueManager.Instance.PlayDialogue(mono8);

        // advance to task 14 door
        advanceObjective();

        yield return new WaitUntil(() => checkTaskCompletion());
        ObjectiveManager.Instance.hideObjective();
        PlayerController.Instance.freeze = true;

        Speech[] mono9 = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[]
            {
                "The door... it was unlocked. There's a staircase going down.",
                "It's completely dark down there.",
                "Michael!",
                "...",
                "...no response.",
                "I have to go down."
            }},
        };
        yield return DialogueManager.Instance.PlayDialogue(mono9);

        PlayerController.Instance.sprintingEnabled = false;
        PlayerController.Instance.walkSpeed = PlayerController.Instance.walkSpeed * 2/3;
        PlayerController.Instance.freeze = false;

        yield return new WaitUntil(() => HasFlag(StoryFlag.basement));


        PlayerController.Instance.freeze = true;
        // moves player to a position and waits till its done so we can start dialogue and then the chase sequence
        yield return MovePlayerToPosition(
            new Vector3(-33.89117f, -2.359681f, 11.98751f),
            Quaternion.Euler(0f, 145.75f, 0f),
            Quaternion.Euler(3.75f, 0f, 0f),
            2f
        );



        Speech[] dialogue = new Speech[]
        {
            new Speech() {title = mainCharacter.name, color = mainCharacter.speechColor, speeches = new string[] { "What the hell...", "Who is that?", "What is he doing?"}}
        };
        yield return DialogueManager.Instance.PlayDialogue(dialogue);

        Killer killerObject = GameObject.Find("Enemy").GetComponent<Killer>();

        yield return killerObject.NoticePlayer(2f);

        Speech[] dialogue2 = new Speech[]
        {
            new Speech() {title = "???", color = killer.speechColor, speeches = new string[] { "Well, well, well...", "I've been waiting for you."}},
            new Speech() {title = "", color = michael.speechColor, speeches = new string[] { "RUN!"} }
        };
        yield return DialogueManager.Instance.PlayDialogue(dialogue2);

        // start chase scene
        PlayerController.Instance.sprintingEnabled = true;
        PlayerController.Instance.walkSpeed = PlayerController.Instance.walkSpeed * 3 / 2;
        PlayerController.Instance.freeze = false;

        killerObject.agent.BlackboardReference.SetVariableValue("chase", true);
        killerObject.agent.BlackboardReference.SetVariableValue("Target", GameObject.Find("Player"));
    }




    #endregion
}

#region CLASSES

[System.Serializable]
public class Task
{
    public string id;
    public string text;
    public bool complete = false;
}

[System.Serializable]
public class Objective
{
    public int stageID;
    public string title;
    public Task[] tasks;
}

[System.Serializable]
public class Character
{
    public string name;
    public Color speechColor;
    public Texture2D icon;
}

#endregion