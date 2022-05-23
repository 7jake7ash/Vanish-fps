// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CountdownTimer.cs" company="Exit Games GmbH">
//   Part of: Photon Unity Utilities,
// </copyright>
// <summary>
// This is a basic CountdownTimer. In order to start the timer, the MasterClient can add a certain entry to the Custom Room Properties,
// which contains the property's name 'StartTime' and the actual start time describing the moment, the timer has been started.
// To have a synchronized timer, the best practice is to use PhotonNetwork.Time.
// In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
// from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;.
// You can do this from Unity's OnDisable function for example.
// </summary>
// <author>developer@exitgames.com</author>
// --------------------------------------------------------------------------------------------------------------------

using ExitGames.Client.Photon;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

namespace Photon.Pun.UtilityScripts
{
    /// <summary>This is a basic, network-synced CountdownTimer based on properties.</summary>
    /// <remarks>
    /// In order to start the timer, the MasterClient can call SetStartTime() to set the timestamp for the start.
    /// The property 'StartTime' then contains the server timestamp when the timer has been started.
    /// 
    /// In order to subscribe to the CountdownTimerHasExpired event you can call CountdownTimer.OnCountdownTimerHasExpired
    /// += OnCountdownTimerIsExpired;
    /// from Unity's OnEnable function for example. For unsubscribing simply call CountdownTimer.OnCountdownTimerHasExpired
    /// -= OnCountdownTimerIsExpired;.
    /// 
    
    /// </remarks>
    public class CountdownTimer : MonoBehaviourPunCallbacks
    {
        /// <summary>
        ///     OnCountdownTimerHasExpired delegate.
        /// </summary>
        public delegate void CountdownTimerHasExpired();
        public int index;
        public Text Text;
        public const string CountdownStartTime = "StartTime";

        [Header("Countdown time in seconds")] 
        public float Countdown = 5.0f;
        private bool isTimerRunning;
        private int startTime;
        public GameObject panel;

        [Header("Reference to a Text component for visualizing the countdown")]
        public static int Index;
        /// <summary>
        ///     Called when the timer has expired.
        /// </summary>
        public static event CountdownTimerHasExpired OnCountdownTimerHasExpired;

        void Start()
        {

        }

        public override void OnEnable()
        {
            Debug.Log("OnEnable CountdownTimer");
            base.OnEnable();
            PhotonNetwork.CurrentRoom.IsOpen = false;
            //Countdown = 5f;
            photonView.RPC("UI", RpcTarget.All);

            Initialize();
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Debug.Log("OnDisable CountdownTimer");
        }


        public void Update()
        {
            if (!this.isTimerRunning) return;

            float countdown = TimeRemaining();
            this.Text.text = string.Format(countdown.ToString("n0"));

            if (countdown > 0.0f) return;

            countdown = Countdown;

            OnTimerEnds();
        }

        private void OnTimerRuns()
        {
            this.isTimerRunning = true;
            this.enabled = true;
        }

        private void OnTimerEnds()
        {
            this.isTimerRunning = false;
            this.enabled = false;

            Debug.Log("Emptying info text.", this.Text);
            this.Text.text = string.Empty;

            if (OnCountdownTimerHasExpired != null) OnCountdownTimerHasExpired();

            //Start Game
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogError("load scene" + index);
                PhotonNetwork.LoadLevel(index);
            }
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
            Initialize();
        }

        private void Initialize()
        {
            int propStartTime;
            if (TryGetStartTime(out propStartTime))
            {
                this.startTime = PhotonNetwork.ServerTimestamp;
                Debug.Log("Initialize sets StartTime " + this.startTime + " server time now: " + PhotonNetwork.ServerTimestamp + " remain: " + TimeRemaining());
                
                this.isTimerRunning = TimeRemaining() > 0;
                Debug.LogError(isTimerRunning);
                if (this.isTimerRunning)
                    OnTimerRuns();
                else
                    OnTimerEnds();
            }
        }

        private float TimeRemaining()
        {
            int timer = PhotonNetwork.ServerTimestamp - this.startTime;// Fix This!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            return this.Countdown - timer / 1000f;
        }

        public static bool TryGetStartTime(out int startTimestamp)
        {
            startTimestamp = PhotonNetwork.ServerTimestamp;

            object startTimeFromProps;
            if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(CountdownStartTime, out startTimeFromProps))
            {
                startTimestamp = (int)startTimeFromProps;
                return true;
            }

            return false;
        }
        
        public void SetStartTime()
        {
            int startTime = 0;
            bool wasSet = TryGetStartTime(out startTime);
            
            //photonView.RPC("UI", RpcTarget.All);/////////////////////////////////////////////////////////////////////////////// Turns on UI
            
            Hashtable props = new Hashtable
            {
                {CountdownTimer.CountdownStartTime, (int)PhotonNetwork.ServerTimestamp}
            };
            PhotonNetwork.CurrentRoom.SetCustomProperties(props);


            Debug.Log("Set Custom Props for Time: "+ props.ToStringFull() + " wasSet: "+wasSet);
        }
        
        [PunRPC]
        public void UI()
        {
            panel.SetActive(true);
        }
    }
}