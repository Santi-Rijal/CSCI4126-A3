using UnityEngine;
using Riptide;

public class Controls : MonoBehaviour {

    public void SpaceClicked() {
        // Send failed to connect message to server.
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
        message.Add("Space Button clicked");
        NetworkManager.Singleton.Client.Send(message);
    }

    public void ResetClicked() {
        // Send failed to connect message to server.
        Message message = Message.Create(MessageSendMode.Reliable, ClientToServerId.name);
        message.Add("Reset Button clicked");
        NetworkManager.Singleton.Client.Send(message);
    }
}
