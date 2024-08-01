## $\color{cyan}{TTS\space Telegram\space Bot}$

> [!NOTE]
> This Telegram bot allows users to send text prompts which are then converted into audio files using text-to-speech technology.</br></br>
> The resulting audio files are sent back to the users.</br></br>
> Currently, this version is designed specifically for Windows because it uses the System.Speech.Synthesis library, which is only supported on the Windows platform.</br>

> [!IMPORTANT]
> List of commands</br></br>
> /voices - Displays all the voices currently installed on your machine</br>
> /setvoice <desired-voice> - Sets the desired voice you gave it</br></br>
> **NOTE:** In order for the voices to work, you need to have them installed in windows.

## 1. Setup
Create a .env file in the root folder of this app, and add the following

``` ini
BOT_TOKEN=Your-Bot-Token-From-Botfather
```

In order to get the bot token, head over to <a href="https://telegram.me/BotFather">@BotFather</a> in Telegram and create a bot and copy its token and replace 'Your-Bot-Token-From-Botfather' with your actual token </br>
> :information_source: **Telegram Bot Tokens look like this** 1234321567:RAnD0m35cHArA3teR5


## 2. Build & Run
1. **After you've done everything:**
    - Double-check that all necessary files are in place.
    - Ensure that all configurations are properly set.

2. **Run the build script:**
    ```bash
    BuildAndRun.bat
    ```
    - This script will compile and launch your application.

3. **Head over to your Telegram bot:**
    - Open the Telegram app.
    - Start interacting with your bot.

4. **Enjoy! 🎉**

## 3. Working Examples
When the user gives a prompt</br></br>
![image](https://github.com/user-attachments/assets/9b40124b-f25c-4dfa-a66a-d06878c6e7c7)

When the user sends __/voices__ command</br></br>
![image](https://github.com/user-attachments/assets/f5b0a128-c26c-488c-85c9-38ebcda61c89)

When the user sets the desired voice</br></br>
![image](https://github.com/user-attachments/assets/f9fe3061-1b7e-4fc1-b5b6-55c4d962fa63)



