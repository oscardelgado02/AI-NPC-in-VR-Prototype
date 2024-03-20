# Creative Prototyping Univrse

### Controls
- To move your position, you can push and hold the right Joystick forward, then release.
- To talk to the NPC, just aproach to it and it will start the conversation. The microphone is all the time listening, so talk without having to press any button!

### Settings interface.
You have an interface at your left when you spawn to change some settings:
- To select one item of the interface, press the "Trigger" button with one of the controllers while pointing at the item.
- You can change between microphones with a dropdown.
- Also, this interface has an "Exit" button.

### NPC Interface
The NPC has three different states:

- While it is doing the Speech To Text task, it will show an ear image above its head. While this image is visible, the NPC won't listen the user words.

![alt text](https://github.com/oscardelgado02/Creative-Prototyping-Univrse/blob/main/Creative%20Prototyping%20Univrse/Assets/Sprites/Listen.png)

- While it is generating the response (ChatGPT), it will show a cloud image above its head. While this image is visible, the NPC won't listen the user words.

![alt text](https://github.com/oscardelgado02/Creative-Prototyping-Univrse/blob/main/Creative%20Prototyping%20Univrse/Assets/Sprites/Think.png)

- While it is speaking (Text To Speech), it will show an audio source image above its head. While this image is visible, the NPC will listen the user words, and if the user talks, the NPC will be interrupted.

![alt text](https://github.com/oscardelgado02/Creative-Prototyping-Univrse/blob/main/Creative%20Prototyping%20Univrse/Assets/Sprites/Speak.png)

### NPC Speech to Answer system
The NPC has three different parts to process the audio from the user and generate an audio answer:

- Speech-To-Text: Using the OpenAI API, the audio recorded from the user is sent to the OpenAI Whisper Speech-To-Text model. This model receives as input an audio of the user saying a sentence or question, and it outputs the sentence or question of the user in text format. When a response is generated (in string format), the system send this text to the next block.
- Conversational Block: Using the OpenAI API, the system does another call, this time sends the text to OpenAI ChatGPT 3.5 conversational model. This model receives as input a text (the user sentence from the previous block) and it generates an answer in text format. When the response from ChatGPT is generated (in string format), the system send this text to the next block.
- Text-To-Speech: Finally, the system calls to the Windows Synth and sets as input the answer generated in the previous block. The Windows Synth transforms the sentence into audio and it reproduces it.

The first time the NPC interacts with the user, a prompt is sent to the Conversational Block (ChatGPT 3.5), explaining the model that it has to act as a roman and that it does not have to mention it is an AI model. The prompt is more complex and has more details, and it can be found in the SpeechToAnswerSystem.cs script.

### Notes about the behavior of the NPC
- The NPC is all the time acting as a roman from the old roman empire age. It is a citizen that talks with brief and humorous responses.
- The NPC proactively interacts with the user if the user gets close to him/her. When the user gets away from the NPC and comes back, the NPC will start talking again.
- The NPC can be interrupted when talking to it.
- The NPC only will listen you if you talk looking at it. This way, in scenarios where users might be conversing with friends rather than the NPC, this feature can prevent confusion or frustration in a public exhibition setting.
- The NPC talks in spanish.
- The NPC responses have a low latency, as one of the tasks, the Text To Speech task, is processed by the Windows Synth.

### Enable your credentials
To make work the NPC, you need your OpenAI credentials.

Follow these steps:

- Create a folder called .openai in your home directory (e.g. `C:User\UserName\` for Windows or `~\` for Linux or Mac)
- Create a file called `auth.json` in the `.openai` folder
- Add an api_key field and a organization field (optional) to the auth.json file and save it
- Here is an example of what your auth.json file should look like:

```json
{
    "api_key": "sk-...W6yi",
    "organization": "org-...L7W"
}
```
