using agsXMPP;
using agsXMPP.protocol.client;
using agsXMPP.Xml.Dom;
using System;
using System.Threading;

namespace SendNotification
{
    internal class Program
    {
        private static volatile XmppClientConnection xmpp;

        private static void Main(string[] args)
        {
            xmpp = new XmppClientConnection("ec2-107-22-92-51.compute-1.amazonaws.com", 5222);
            Console.WriteLine("Writing...");
            xmpp.UseSSL = false;
            xmpp.AutoAgents = false;
            xmpp.AutoPresence = false;
            xmpp.AutoRoster = false;
            xmpp.Username = "user";
            xmpp.Server = "localhost";
            xmpp.ConnectServer = "ec2-107-22-92-51.compute-1.amazonaws.com";
            xmpp.UseCompression = false;
            xmpp.AutoResolveConnectServer = false;
            xmpp.UseStartTLS = false;
            xmpp.OnError += XmppOnErrorException;
            xmpp.OnSocketError += XmppOnErrorException;
            xmpp.OnAuthError += AuthXmppOnErrorElement;
            xmpp.OnRegisterError += RegisterXmppOnErrorElement;
            xmpp.OnStreamError += StreamXmppOnErrorElement;
            xmpp.OnClose += XmppOnOnClose;
            xmpp.OnAgentEnd += XmppOnOnAgentEnd;
            xmpp.OnWriteXml += XmppOnOnWriteXml;
            xmpp.OnReadXml += XmppOnOnReadXml;
            xmpp.OnLogin += XmppOnOnLogin;
            xmpp.Open("user", "user");
            Thread.Sleep(3000);
            Console.WriteLine("xmpp Connection State {0}", xmpp.XmppConnectionState);
            Console.WriteLine("xmpp Authenticated? {0}", xmpp.Authenticated);
            xmpp.Close();
            Thread.Sleep(1000);
            Console.WriteLine("Notification sent.");
        }

        private static void XmppOnOnReadXml(object sender, string xml)
        {
            Console.WriteLine("Receiving: " + xml);
        }

        private static void XmppOnOnWriteXml(object sender, string xml)
        {
            Console.WriteLine("Sending: " + xml);
        }

        private static void XmppOnOnAgentEnd(object sender)
        {
            Console.WriteLine("Agent closed.");
        }

        private static void XmppOnOnClose(object sender)
        {
            Console.WriteLine("Connection closed.");
        }

        private static void XmppOnOnLogin(object sender)
        {
            Console.WriteLine("Attempt to send.");
            var recepient = new Jid("admin", "localhost", null);
            Console.WriteLine("Sending to: " + recepient);
            var message = new Message(recepient, MessageType.chat, @"ClientUpdated3");
            message.From = "user@localhost";
            message.Language = "en";
            xmpp.Send(message);
            Console.WriteLine("Did send.");
        }

        private static void AuthXmppOnErrorElement(object sender, Element element)
        {
            Console.WriteLine("Auth Error:");
            Console.WriteLine(element.ToString());
        }

        private static void RegisterXmppOnErrorElement(object sender, Element element)
        {
            Console.WriteLine("Register Error:");
            Console.WriteLine(element.ToString());
        }

        private static void StreamXmppOnErrorElement(object sender, Element element)
        {
            Console.WriteLine("Stream Error:");
            Console.WriteLine(element.ToString());
        }

        private static void XmppOnErrorException(object sender, Exception exception)
        {
            Console.WriteLine("OnExceptionError:");
            Console.WriteLine(exception.Message);
        }
    }
}