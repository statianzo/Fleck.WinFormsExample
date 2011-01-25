using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Fleck.WinFormsExample
{
	public partial class MainForm : Form
	{
		private readonly IList<WebSocketConnection> _allSockets = new List<WebSocketConnection>();

		public MainForm()
		{
			InitializeComponent();
			SetupServer();
		}

		private void SetupServer()
		{
			var server = new WebSocketServer("ws://localhost:54321");
			server.Start(c =>
				{
					c.OnOpen = () =>
						{
							AddItemOnUIThread("Open!");
							_allSockets.Add(c);
						};
					c.OnClose = () =>
						{
							AddItemOnUIThread("Closed!");
							_allSockets.Remove(c);
						};
					c.OnMessage = m => AddItemOnUIThread(m);
				});
		}

		private void AddItemOnUIThread(string message)
		{
			Invoke(new Action(() => lbxInput.Items.Add(message)));
		}


		private void OnSendClick(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtOutput.Text))
				return;

			foreach (WebSocketConnection socket in _allSockets)
			{
				socket.Send(txtOutput.Text);
			}
		}
	}
}