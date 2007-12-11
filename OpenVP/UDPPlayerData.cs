// UDPPlayerData.cs
//
//  Copyright (C) 2007 [name of author]
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
//

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace OpenVP {
	/// <summary>
	/// Reads player data from a listening UDP socket.
	/// </summary>
	public class UDPPlayerData : PlayerData {
		private enum MessageType : int {
			PositionUpdate = 1,
			TitleUpdate = 2,
			
			PCMUpdate = 3,
			SpectrumUpdate = 4,
			
			SliceComplete = 5,
		}
		
		private UdpClient mClient;
		
		/// <summary>
		/// Creates a new UDP-based player data source.
		/// </summary>
		/// <remarks>
		/// This constructor uses the default port, 40507.
		/// </remarks>
		public UDPPlayerData() : this(40507) {
		}
		
		/// <summary>
		/// Creates a new UDP-based player data source.
		/// </summary>
		/// <param name="port">
		/// The port number to listen on.
		/// </param>
		public UDPPlayerData(int port) {
			try {
				this.mClient = new UdpClient(port, AddressFamily.InterNetworkV6);
			} catch {
				this.mClient = new UdpClient(port, AddressFamily.InterNetwork);
			}
		}
		
		private bool mIsBeat = false;
		
		/// <value>
		/// True if this slice of data is on a beat.
		/// </value>
		public override bool IsBeat {
			get {
				return this.mIsBeat;
			}
			set {
				this.mIsBeat = true;
			}
		}
		
		private double mSongPosition = 0;
		
		/// <value>
		/// The current song's position in fractional seconds.
		/// </value>
		public override double SongPosition {
			get {
				return this.mSongPosition;
			}
		}
		
		private string mSongTitle = null;
		
		/// <value>
		/// The current song's title.
		/// </value>
		public override string SongTitle {
			get {
				return this.mSongTitle;
			}
		}
		
		private static float[] mZeroArray = new float[] { 0 };
		
		private bool mDataInitialized = false;
		
		private float[][] mPCMData = new float[][] { mZeroArray, mZeroArray };
		
		private float[][] mSpecData = new float[][] { mZeroArray, mZeroArray };
		
		private Queue<byte[]> mLazyUpdateQueue = new Queue<byte[]>();
		
		private bool GetMessageType(byte[] data, out MessageType type) {
			if (data.Length < 4) {
				type = (MessageType) 0;
				
				return false;
			}
			
			type = (MessageType) BitConverter.ToInt32(data, 0);
			
			return true;
		}
		
		private void ProcessUpdate(byte[] data) {
			MessageType type;
			
			if (!this.GetMessageType(data, out type))
				return;
			
			switch (type) {
			case MessageType.PositionUpdate:
				if (data.Length < 12)
					break;
				
				this.mSongPosition = BitConverter.ToDouble(data, 4);
				break;
				
			case MessageType.TitleUpdate:
				this.mSongTitle = Encoding.ASCII.GetString(data, 4,
				                                           data.Length - 4);
				break;
				
			case MessageType.PCMUpdate:
				this.FillData(ref this.mPCMData, data);
				break;
				
			case MessageType.SpectrumUpdate:
				this.FillData(ref this.mSpecData, data);
				break;
			}
		}
		
		private void FillData(ref float[][] data, byte[] message) {
			// Divide by 2 since we have a left and right channel, and divide by
			// 4 since we are taking floats.
			int bytelength = (message.Length - 4) / 2;
			int length = bytelength / 4;
			
			if (!this.mDataInitialized || data[0].Length != length) {
				data = new float[][] { new float[length], new float[length] };
				this.mDataInitialized = true;
			}
			
			Buffer.BlockCopy(message, 4,              data[0], 0, bytelength);
			Buffer.BlockCopy(message, 4 + bytelength, data[1], 0, bytelength);
		}
		
		/// <summary>
		/// Attempts to update the player data.
		/// </summary>
		/// <returns>
		/// True if and only if the data was updated.
		/// </returns>
		/// <remarks>
		/// If this method returns false, it is guaranteed that no data on the
		/// object will have changed.
		/// </remarks>
		public override bool Update() {
			while (this.mClient.Available != 0) {
				IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
				
				byte[] data = this.mClient.Receive(ref remote);
				
				MessageType type;
				if (this.GetMessageType(data, out type)) {
					if (type == MessageType.SliceComplete) {
						this.ProcessLazyUpdateQueue();
						return true;
					}
					
					this.mLazyUpdateQueue.Enqueue(data);
				}
			}
			
			if (this.mLazyUpdateQueue.Count != 0)
				Console.WriteLine("Bailing with {0} messages in the queue.",
				                  this.mLazyUpdateQueue.Count);
			
			return false;
		}
		
		private void ProcessLazyUpdateQueue() {
			while (this.mLazyUpdateQueue.Count != 0)
				this.ProcessUpdate(this.mLazyUpdateQueue.Dequeue());
		}
		
		/// <summary>
		/// Updates the player data, blocking until successful.
		/// </summary>
		public override void UpdateWait() {
			this.ProcessLazyUpdateQueue();
			
			byte[] data;
			
			MessageType type;
			
			for (;;) {
				IPEndPoint remote = new IPEndPoint(IPAddress.Any, 0);
				
				data = this.mClient.Receive(ref remote);
				
				if (this.GetMessageType(data, out type)) {
					if (type == MessageType.SliceComplete)
						break;
					
					this.ProcessUpdate(data);
				}
			};
		}
		
		private void GetCenter(float[][] data, float[] output) {
			if (output == null)
				return;
			
			float[] center = new float[data[0].Length];
			
			for (int i = 0; i < center.Length; i++)
				center[i] = data[0][i] / 2 + data[1][i] / 2;
			
			PlayerData.Interpolate(center, output);
		}
		
		private void GetChannels(float[][] data, float[][] channels) {
			if (channels.Length == 0)
				return;
			
			if (channels.Length == 1) {
				this.GetCenter(data, channels[0]);
				return;
			}
			
			if (channels[0] != null)
				PlayerData.Interpolate(data[0], channels[0]);
			
			if (channels[1] != null)
				PlayerData.Interpolate(data[1], channels[1]);
			
			for (int i = 2; i < channels.Length; i++)
				if (channels[i] != null)
					Array.Clear(channels[i], 0, channels[i].Length);
		}
		
		/// <summary>
		/// Returns PCM data.
		/// </summary>
		/// <param name="channels">
		/// An array of arrays to fill.
		/// </param>
		public override void GetPCM(float[][] channels) {
			this.GetChannels(this.mPCMData, channels);
		}
		
		/// <summary>
		/// Returns spectrum analyzer data.
		/// </summary>
		/// <param name="channels">
		/// An array of arrays to fill.
		/// </param>
		public override void GetSpectrum(float[][] channels) {
			this.GetChannels(this.mSpecData, channels);
		}
		
		public override int NativePCMLength {
			get {
				return this.mPCMData[0].Length;
			}
		}
		
		public override int NativeSpectrumLength {
			get {
				return this.mSpecData[0].Length;
			}
		}
	}
}
