using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ClientSocket : MonoBehaviour
{
    public string communicate(string address, int portNumber, string msg)
    {
        // specify server IP and port number
        IPAddress ipAddress = IPAddress.Parse(address);
        int port = portNumber;
        // create a socket object
        Socket clientSocket = new Socket(
            ipAddress.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp
        );
        try
        {
            // connect to the server
            clientSocket.Connect(new IPEndPoint(ipAddress, port));

            // send request to the server
            string request = msg + Globals.endOfFileSignal;
            byte[] requestBytes = Encoding.UTF8.GetBytes(request);
            clientSocket.Send(requestBytes);
            Debug.Log("message sent");

            // receive the response from the server
            StringBuilder response = new StringBuilder();
            byte[] responseBytes = new byte[4096];
            int bytesRead = 0;

            do
            {
                bytesRead = clientSocket.Receive(responseBytes);
                response.Append(Encoding.UTF8.GetString(responseBytes, 0, bytesRead));
            } while (bytesRead == responseBytes.Length); // Continue receiving data in chunks until the server sends less data than the buffer size, indicating that it has finished.

            Debug.Log("Response from the server: " + response.ToString());

            // close the socket
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            return response.ToString();
        }
        catch (Exception ex)
        {
            Debug.Log("Connection failed: " + ex.Message);
            return "Connection failed";
        }
    }
}
