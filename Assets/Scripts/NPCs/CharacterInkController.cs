﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Ink.Runtime;
using UnityEngine.Events;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class CharacterInkController : MonoBehaviour {
	
	[SerializeField]
	private TextAsset inkJSONAsset;
	public Story story;	

    [SerializeField]
    private VerticalLayoutGroup buttonLayoutGroup;
    [SerializeField]
    private RectTransform npcDialogueParent;

	// UI Prefabs
	[SerializeField]
	private Text textPrefab;
	[SerializeField]
	private Button buttonPrefab;

	// Callbacks
	[SerializeField]
	private UnityEvent startStoryEvent;

    private PlayerController playerController;

    public FMODUnity.StudioEventEmitter roboVoice;

    void Awake () {
		// Remove the default message
        playerController = UnityEngine.Object.FindObjectOfType<PlayerController>();
		RemoveChildren();
	}

	// Creates a new Story object with the compiled story which we can then play!
	public void StartStory () {
		story = new Story (inkJSONAsset.text);
		RefreshView();

		if (startStoryEvent != null)
		{
			startStoryEvent.Invoke();
		}
	}
	
	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView () {
		// Remove all the UI on screen
		RemoveChildren ();
		
		// Read all the content until we can't continue any more
		ShowNextNPCDialogueLine();

		// Display all the choices, if there are any!
		if(story.currentChoices.Count > 0) {
			for (int i = 0; i < story.currentChoices.Count; i++) {
				Choice choice = story.currentChoices [i];
				Button button = CreateChoiceView (choice.text.Trim ());
				// Tell the button what to do when we press it
				button.onClick.AddListener (delegate {
					OnClickChoiceButton (choice);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else {
			Button choice = CreateChoiceView("Goodbye");
			choice.onClick.AddListener(delegate{
				RemoveChildren();
                playerController.ExitStoryMode();
			});
		}
	}

	void ShowNextNPCDialogueLine()
	{
        //stop any previous dialogue audio, then play a new one
        roboVoice.Stop();
        roboVoice.Play();
		// TODO: show each line manually, proceeding via player input
		while (story.canContinue) {
			// Continue gets the next line of the story
			string text = story.Continue ();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the text on screen!
			CreateContentView(text);
		}
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton (Choice choice) {
		story.ChooseChoiceIndex (choice.index);
		RefreshView();
	}

	// Creates a button showing the choice text
	void CreateContentView (string text) {
		Text storyText = Instantiate (textPrefab) as Text;
		storyText.text = text;
		storyText.transform.SetParent (npcDialogueParent.transform, false);
	}

	// Creates a button showing the choice text
	Button CreateChoiceView (string text) {
		// Creates the button from a prefab
		Button choice = Instantiate (buttonPrefab) as Button;
		choice.transform.SetParent (buttonLayoutGroup.transform, false);
		
		// Gets the text from the button prefab
		Text choiceText = choice.GetComponentInChildren<Text> ();
		choiceText.text = text;

		// Make the button expand to fit the text
		HorizontalLayoutGroup layoutGroup = choice.GetComponent <HorizontalLayoutGroup> ();
		layoutGroup.childForceExpandHeight = false;

		return choice;
	}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren () {
		int childCount = npcDialogueParent.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			GameObject.Destroy (npcDialogueParent.transform.GetChild (i).gameObject);
		}

        childCount = buttonLayoutGroup.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i) {
			GameObject.Destroy (buttonLayoutGroup.transform.GetChild (i).gameObject);
		}
        //stop audio at end of conversation
        roboVoice.Stop();
	}
}