using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class ClientSocket : MonoBehaviour
{
    public static string communicate(string address, int port_number, string msg)
    {
        // specify server IP and port number
        IPAddress ipAddress = IPAddress.Parse(address);
        int port = port_number;
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
            string request = msg;
            byte[] requestBytes = Encoding.UTF8.GetBytes(request);
            clientSocket.Send(requestBytes);

            // receive the response from the server
            byte[] responseBytes = new byte[1024];
            int bytesRead = clientSocket.Receive(responseBytes);
            string response = Encoding.UTF8.GetString(responseBytes, 0, bytesRead);
            Debug.Log("Response from the server: " + response);

            // close the socket
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
            return response;
        }
        catch (Exception ex)
        {
            Debug.Log("Connection failed: " + ex.Message);
            return "Connection failed";
        }
    }
}
