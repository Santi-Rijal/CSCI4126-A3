using UnityEngine;
using Riptide;

/**
 * Controls.cs
 * 
 * Handles player control inputs for a Unity game, sending corresponding messages to the server when the 'Space' button is Clicked or released.
 * 
 * Authors: Santi Rijal, Adam Sarty
 * Course: CSCI4126
 * Assignment: 3
 */

public class Controls : MonoBehaviour {

    // Called when the 'Space' button is clicked
    public void Thump() {
        Debug.Log("Thump clicked.");
        SendButtonMessage(ClientToServerId.player, "Thump called.");
    }

    // Called when the 'Swing Hammer' button is clicked
    public void SwingHammer() {
        Debug.Log("Hammer Swing clicked.");
        SendButtonMessage(ClientToServerId.player, "Hammer Swing called.");
    }

     // Called when the 'Reset' button is clicked
    public void Reset() {
        Debug.Log("Reset clicked.");
        SendButtonMessage(ClientToServerId.player, "Reset called.");
    }

    // Sends a message to the server indicating which button was interacted with
    private void SendButtonMessage(ClientToServerId id, string messageText) {
        Message message = Message.Create(MessageSendMode.Reliable, id);
        message.Add(messageText);
        NetworkManager.Singleton.Client.Send(message);
    }
}
