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
using UnityEngine.UI;
using Photon.Pun.UtilityScripts;
using System.Collections.Generic;

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
    /// You can do this from Unity's OnEnable and OnDisable functions.
    /// </remarks>
    public class Timer : MonoBehaviourPunCallbacks
    {
        /// <summary>
        ///     OnCountdownTimerHasExpired delegate.
        /// </summary>
        public delegate void CountdownTimerHasExpired();

        public const string CountdownStartTime = "StartTime";

        [Header("Countdown time in seconds")]
        public float Countdown = 50f;

        public bool isTimerRunning;

        public int startTime;

        public Material mat;

        [Header("Reference to a Text component for visualizing the countdown")]
        public Text Text;

        public GameObject image;

        /// <summary>
        ///     Called when the timer has expired.
        /// </summary>
        public static event CountdownTimerHasExpired OnCountdownTimerHasExpired;

        int Team;

        public void Start()
        {
            
        }
        
        public override void OnEnable()
        {
            Debug.LogError("OnEnable CountdownTimer");
            base.OnEnable();

            //textObject.SetActive(true);
            //image.SetActive(true);

            //the starttime may already be in the props. look it up.
            Initialize();

            foreach (PhotonView plrMan in PhotonNetwork.PhotonViewCollection)
            {
                if (plrMan.gameObject.CompareTag("PlrMan") && plrMan.GetComponent<PhotonView>().IsMine)
                {
                    Team = plrMan.GetComponent<PlayerManager>().team;
                }
            }
        }

        public override void OnDisable()
        {
            base.OnDisable();
            Debug.Log("OnDisable CountdownTimer");
        }

        public void Update()
        {
            if (!this.isTimerRunning)
            {
                //Wait for all Players
                //if (GameObject.FindGameObjectsWithTag("Player").Length == PhotonNetwork.PlayerList.Length)
                //{
                //    Initialize();
                //}

                return;
            }

            float countdown = TimeRemaining();
            //this.Text.text = string.Format(countdown.ToString("n0"));
            image.transform.localScale = new Vector3(countdown * .1f, 1, 1);

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
            image.SetActive(false);


            //Debug.Log("Emptying info text.", this.Text);
            //this.Text.text = string.Empty;

            if (OnCountdownTimerHasExpired != null) OnCountdownTimerHasExpired();

            foreach (PhotonView plr in PhotonNetwork.PhotonViewCollection)
            {
                if (plr.gameObject.CompareTag("Player") && !plr.GetComponent<PhotonView>().IsMine && plr.GetComponent<PlayerController>().plrManager.team != 0 && Team == plr.GetComponent<PlayerController>().plrManager.team)
                {
                    plr.transform.Find("Recoil/CameraHolder/Head").gameObject.tag = "Dead";
                    plr.transform.Find("Model/Body").gameObject.tag = "Dead";

                    plr.GetComponent<TeamColor>().headOutline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
                    plr.GetComponent<TeamColor>().outline.OutlineMode = Outline.Mode.OutlineAndSilhouette;
                }
            }

            photonView.RPC("SetVisabilty", RpcTarget.All, true);
        }
        [PunRPC]
        void SetVisabilty(bool on)
        {
            //textObject.SetActive(!on);
            //image.SetActive(!on);

            foreach (PhotonView plr in PhotonNetwork.PhotonViewCollection)
            {
                if (plr.gameObject.CompareTag("Player"))
                {
                    GameObject plrObject = PhotonNetwork.GetPhotonView(plr.ViewID).gameObject;
                    Debug.LogWarning(plrObject);
                    plrObject.transform.Find("Recoil/CameraHolder/Head").gameObject.GetComponent<MeshRenderer>().enabled = on;
                    plrObject.transform.Find("Model/Body").gameObject.GetComponent<MeshRenderer>().enabled = on;
                    //plrObject.transform.Find("Model/VanishPlayer/Cube").gameObject.GetComponent<MeshRenderer>().enabled = on;
                    plrObject.transform.Find("Recoil/CameraHolder/itemContainer").gameObject.SetActive(on);
                    //mat.SetVector("MaxTime", new Vector2(1, 0));
                    //Invoke("StopDissovle", 1f);
                }
            }
        }
        void StopDissovle()
        {
            mat.SetVector("MaxTime", new Vector2(0, 0));
        }

        void EndGame()
        {
            //textObject.SetActive(false);
            image.SetActive(false);

            Debug.Log("end game");

            PhotonNetwork.LoadLevel(1);

            PhotonNetwork.CurrentRoom.IsOpen = true;
        }

        public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
        {
            Debug.Log("CountdownTimer.OnRoomPropertiesUpdate " + propertiesThatChanged.ToStringFull());
            Initialize();
        }

        private void Initialize()
        {
            //int propStartTime;
            //if (TryGetStartTime(out propStartTime))
            //{
                this.startTime = PhotonNetwork.ServerTimestamp;
                Debug.Log("Initialize sets StartTime " + this.startTime + " server time now: " + PhotonNetwork.ServerTimestamp + " remain: " + TimeRemaining());

                this.isTimerRunning = TimeRemaining() > 0;
                
                if (this.isTimerRunning)
                    OnTimerRuns();
                else
                    OnTimerEnds();
            //}
        }


        private float TimeRemaining()
        {
            int timer = PhotonNetwork.ServerTimestamp - this.startTime;
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


        public void SetVis(int plr)
        {
            //int startTime = 0;
            //bool wasSet = TryGetStartTime(out startTime);

            //Hashtable props = new Hashtable
            //{
            //    {CountdownTimer.CountdownStartTime, (int)PhotonNetwork.ServerTimestamp}
            //};
            //PhotonNetwork.CurrentRoom.SetCustomProperties(props);

            //Debug.Log("Set Custom Props for Time: " + props.ToStringFull() + " wasSet: " + wasSet);

            photonView.RPC("VisItem", RpcTarget.All, plr);
        }
        [PunRPC]
        public void VisItem(int plrID)
        {
            GameObject plr = PhotonNetwork.GetPhotonView(plrID).gameObject;

            //plr.transform.Find("Recoil/CameraHolder/Head").gameObject.GetComponent<MeshRenderer>().enabled = false;
            plr.transform.Find("Model/VanishPlayer/Cube").gameObject.GetComponent<MeshRenderer>().enabled = false;
            plr.transform.Find("Model/Body").gameObject.GetComponent<MeshRenderer>().enabled = false;
            plr.transform.Find("Recoil/CameraHolder/Head").gameObject.GetComponent<MeshRenderer>().enabled = false;
            plr.transform.Find("Recoil/CameraHolder/itemContainer").gameObject.SetActive(false);
        }
    }
}