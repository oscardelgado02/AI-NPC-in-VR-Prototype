# Creative Prototyping Univrse

### Controls
- To move your position, you can push and hold the right Joystick forward, then release.
- You have an interface at your left when you spawn to change between microphones and Text To Speech method. Also, this interface has an "Exit" button.
- To select one item of the interface, press the "Trigger" button with one of the controllers while pointing at the item.

### Notes about the behavior of the NPC
- The NPC is all the time acting as a roman from the old roman empire age. It is a citizen that talks with brief and humorous responses.
- The NPC proactively interacts with the user if the user gets close to him/her.
- The NPC can be interrupted when talking to it.
- The NPC only will listen you if you talk looking at it. This way, in scenarios where users might be conversing with friends rather than the NPC, this feature can prevent confusion or frustration in a public exhibition setting.
- The NPC talks in spanish.
- The NPC responses have a low latency, however, you can select in the interface between the two types of TTS: Windows Synth and ElevenLabs. Windows Synth can achieve a more low latency, while ElevenLabs is more realistic.

### Enable your credentials
To make work the game, you need your OpenAI and ElevenLabs credentials.

Follow these steps for OpenAI:

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

Follow these steps for ElevenLabs:

- Create a folder called .elevenlabs in your home directory (e.g. `C:User\UserName\` for Windows or `~\` for Linux or Mac)
- Create a file called `auth.json` in the `.elevenlabs` folder
- Add the api_key field to the auth.json file and save it
- Here is an example of what your auth.json file should look like:

```json
{
    "api_key": "sk....W6yi"
}
```
