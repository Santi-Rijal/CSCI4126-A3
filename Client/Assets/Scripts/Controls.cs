using UnityEngine;
using Riptide;

/**
 * Class to handle controls of the player.
 */
public class Controls : MonoBehaviour {

    // Button click to move game objects.
    public void SpaceClicked() {
        // Send a message indicating which button was pressed to the server.
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
        message.Add("Space Button clicked");
        NetworkManager.Singleton.Client.Send(message);
    }

    // Button click to reset the game.
    public void ResetClicked() {
        // Send a message indicating which button was pressed to the server.
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
        message.Add("Reset Button clicked");
        NetworkManager.Singleton.Client.Send(message);
    }
}
