using System.Net;
using System.Net.Sockets;
using System.Net.NetworkInformation;
using UnityEngine;

public class WakeOnLan : MonoBehaviour
{
    public string macAddress = "2C-F0-5D-AD-54-A2";
    private byte[] m_MacAddress;

    public void TurnOnPC()
    {
        m_MacAddress = PhysicalAddress.Parse(macAddress).GetAddressBytes();
        Debug.Log("wakeOn");

        UdpClient client = new UdpClient();
        client.Connect(IPAddress.Broadcast, 40000);

        byte[] packet = new byte[17 * 6];

        for (int i = 0; i < 6; i++)
        {
            packet[i] = 0xFF;
        }

        for (int i = 1; i <= 16; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                packet[i * 6 + j] = m_MacAddress[j];
            }
        }
        int d = client.Send(packet, packet.Length);
    }
}
