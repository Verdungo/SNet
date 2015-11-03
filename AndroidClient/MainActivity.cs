﻿using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using SNet.Sockets;
using SNet.Messages;

namespace AndroidClient
{
    [Activity(Label = "AndroidClient", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private SNetClient _sNetClient;
        private Button _connectButton;
        private Button _sendButton;
        private EditText _message;
        private ListView _chatListView;
        //private List<string> _items;
        ArrayAdapter<string> _adapter;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            _connectButton = FindViewById<Button>(Resource.Id.ConnectButton);
            _chatListView = FindViewById<ListView>(Resource.Id.ChatListView);
            _message = FindViewById<EditText>(Resource.Id.Message);
            _sendButton = FindViewById<Button>(Resource.Id.SendButton);

            //_items = new List<string>();

            _adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.TestListItem, new List<string>());

            _chatListView.Adapter = _adapter;

            _connectButton.Click += ConnectButton_Click;
            _sendButton.Click += SendButton_Click;
        }

        private void SNetClient_OnDisconnect(object o, SocketEventArgs e)
        {
            Application.SynchronizationContext.Post((c) =>
            {
                _adapter.Add("Disconnected from server!");
                _adapter.NotifyDataSetChanged();
            }, this);

            //Application.Current.Dispatcher.Invoke((Action)(() =>
            //{
            //    ChatListView.Items.Add(string.Format("Server shuted down"));
            //    _sNetClient = null;
            //    ConnectButton.IsEnabled = true;
            //}));
        }

        private void SNetClient_OnRecieve(object o, SocketEventArgs e)
        {
            Application.SynchronizationContext.Post((c) =>
            {
                switch (e.Message.Type)
                {
                    case MessageType.MessageText:
                        _adapter.Add(String.Format("{0}", e.Message.Text));
                        _adapter.NotifyDataSetChanged();
                        //_chatListView.Text += (string.Format("{0}", e.Message.Text));
                        break;
                    default:
                        break;
                }
            }, this);
            //Application.Current.Dispatcher.Invoke(() =>
            //{
            //    switch (e.Message.Type)
            //    {
            //        case MessageType.MessageText:
            //            ChatListView.Items.Add(string.Format("{0}", e.Message.Text));
            //            break;
            //        default:
            //            break;
            //    }
            //});
        }


        private void SendButton_Click(object sender, EventArgs e)
        {
            MessageText msg = new MessageText(_message.Text);

            _sNetClient?.Send(msg.Data);
            _message.Text = "";
        }

        private void ConnectButton_Click(object sender, EventArgs e)
        {
            _sNetClient = new SNetClient();
            _sNetClient.Connect("192.168.179.101", 50001);
            //_sNetClient.OnConnect += 
            _sNetClient.OnRecieve += SNetClient_OnRecieve;
            _sNetClient.OnDisconnect += SNetClient_OnDisconnect;
            //_sNetClient.OnSend +=
        }
    }
}

