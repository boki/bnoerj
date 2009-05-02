namespace Bnoerj.Storage
{
	using System;
	using Microsoft.Xna.Framework;
	using Microsoft.Xna.Framework.GamerServices;
	using Microsoft.Xna.Framework.Storage;

	/// <summary>
	/// Defines methods and properties to access StorageDevices.
	/// </summary>
	public class StorageProvider
	{
		/// <summary>
		/// The title name to use for sessions.
		/// </summary>
		string titleName;

		/// <summary>
		/// A value indicating whether the device was connected at the
		/// last check.
		/// </summary>
		bool wasConnected;

		/// <summary>
		/// A value indicating the status of the provider.
		/// </summary>
		StorageProviderStatus status;

		/// <summary>
		/// The providers device.
		/// </summary>
		StorageDevice device;

		/// <summary>
		/// The current session context.
		/// </summary>
		/// <remarks>
		/// Only one session can be active for a player/device pair.
		/// </remarks>
		IStorageSessionContext currentSessionContext;

		/// <summary>
		/// Initializes a new instance of the StorageProvider class.
		/// </summary>
		/// <param name="titleName">The title name to use for sessions.</param>
		/// <param name="requirements">The requirements.</param>
		internal StorageProvider(string titleName, StorageRequirements requirements)
		{
			this.titleName = titleName;
			this.Requirements = requirements;
		}

		/// <summary>
		/// Gets the requirements for this provider.
		/// </summary>
		public StorageRequirements Requirements
		{
			get;
			private set;
		}

		/// <summary>
		/// Gets the status of the provider.
		/// </summary>
		public StorageProviderStatus Status
		{
			get { return status; }
			private set { status = value; }
		}

		/// <summary>
		/// Opens a session.
		/// </summary>
		/// <param name="context">The context.</param>
		public void OpenSession(IStorageSessionContext context)
		{
			if (currentSessionContext != null)
			{
				throw new InvalidOperationException("A storage session is already open for this player.");
			}

			currentSessionContext = context;
			if (Status == StorageProviderStatus.Connected)
			{
				using (var container = device.OpenContainer(titleName))
				{
					var session = new StorageSession(this, container);
					context.SessionOpened(session);
				}

				currentSessionContext = null;
			}
			else
			{
				SelectDevice();
			}
		}

		/// <summary>
		/// Updates the internal state of the provider.
		/// </summary>
		internal void Update()
		{
			if (Status == StorageProviderStatus.PendingSelector)
			{
				if (Guide.IsVisible == false)
				{
					SelectDevice();
				}
			}
			else
			{
				bool isConnected = device != null && device.IsConnected;
				if (isConnected == false && wasConnected == true)
				{
					Status = StorageProviderStatus.Disconnected;
				}

				wasConnected = isConnected;
			}
		}

		/// <summary>
		/// Retrieves a value if this provider matches the specified
		/// requirements.
		/// </summary>
		/// <param name="requirements">The requirements to test.</param>
		/// <returns>true if the provider satisfies the requirements; otherwise, false.</returns>
		internal bool MatchesRequirements(StorageRequirements requirements)
		{
			return
				Requirements.Player == requirements.Player &&
				((device != null && device.FreeSpace > requirements.SizeInBytes) ||
				 (Requirements.SizeInBytes >= requirements.SizeInBytes &&
				  Requirements.DirectoryCount >= requirements.DirectoryCount));
		}

		/// <summary>
		/// Shows the storage device selector.
		/// </summary>
		void SelectDevice()
		{
			Status = StorageProviderStatus.ShowSelector;

			if (Requirements.Player.HasValue == false)
			{
				if (Requirements.SizeInBytes == 0)
				{
					Guide.BeginShowStorageDeviceSelector(
						StorageDeviceDismissed, null);
				}
				else
				{
					Guide.BeginShowStorageDeviceSelector(
						Requirements.SizeInBytes, Requirements.DirectoryCount,
						StorageDeviceDismissed, null);
				}
			}
			else
			{
				if (Requirements.SizeInBytes == 0)
				{
					Guide.BeginShowStorageDeviceSelector(
						Requirements.Player.Value,
						StorageDeviceDismissed, null);
				}
				else
				{
					Guide.BeginShowStorageDeviceSelector(
						Requirements.Player.Value,
						Requirements.SizeInBytes, Requirements.DirectoryCount,
						StorageDeviceDismissed, null);
				}
			}
		}

		/// <summary>
		/// Called when a storage device selector has been dismissed.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation.</param>
		void StorageDeviceDismissed(IAsyncResult result)
		{
			device = Guide.EndShowStorageDeviceSelector(result);
			UpdateStatusFromDevice();

			if (Status == StorageProviderStatus.Connected)
			{
				OpenSession(currentSessionContext);
			}
			else
			{
				HandleCancelledBehavior();
			}
		}

		/// <summary>
		/// Determines the action to perform if a device selector has been
		/// cancelled.
		/// </summary>
		void HandleCancelledBehavior()
		{
			var behavior = currentSessionContext.CanceledBehavior;

			// REVIEW: Wording of messages (http://msdn.microsoft.com/en-us/library/aa511263.aspx)
			// TODO: Extract message box variables
			if (behavior == StorageSessionCanceledBehavior.Warn)
			{
				Guide.BeginShowMessageBox(
					// TODO: Propagate controlling player
					PlayerIndex.One,
					titleName,
					"To save your progress it is recommended to select a storage device.",
					//"Because you did not select a storage device, your progress will not be saved.",
					new string[]
					{
						"Select a device",
						"Close"
					},
					0,
					MessageBoxIcon.Warning,
					MessageBoxDismissed,
					null);
			}
			else if (behavior == StorageSessionCanceledBehavior.Force)
			{
				Guide.BeginShowMessageBox(
					// TODO: Propagate controlling player
					PlayerIndex.One,
					titleName,
					"A storage device is required to save your progress.",
					new string[]
					{
						"Close"
					},
					0,
					MessageBoxIcon.Alert,
					MessageBoxDismissed,
					null);
			}
			else
			{
				CancelSession();
			}
		}

		/// <summary>
		/// Called when a message box has been dismissed.
		/// </summary>
		/// <param name="result">The result of the asynchronous operation.</param>
		void MessageBoxDismissed(IAsyncResult result)
		{
			int? button = Guide.EndShowMessageBox(result);
			if (button.HasValue == true && button.Value == 0)
			{
				// TODO: Delay to hide current message box
				Status = StorageProviderStatus.PendingSelector;
				//SelectDevice(context);
			}
			else
			{
				CancelSession();
			}
		}

		/// <summary>
		/// Called to cancel a session.
		/// </summary>
		void CancelSession()
		{
			currentSessionContext.SessionCanceled();
			currentSessionContext = null;

			UpdateStatusFromDevice();
		}

		/// <summary>
		/// Determines the providers status from the attached device.
		/// </summary>
		void UpdateStatusFromDevice()
		{
			if (device != null)
			{
				Status = device.IsConnected == true ?
					StorageProviderStatus.Connected :
					StorageProviderStatus.Disconnected;
			}
			else if (device == null)
			{
				Status = wasConnected == true ?
					StorageProviderStatus.Disconnected :
					StorageProviderStatus.Canceled;
			}
		}
	}
}
