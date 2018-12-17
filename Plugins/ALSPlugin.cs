using System;
using UnityEngine;
using System.Runtime.InteropServices;


public enum ALSAudioManagerState {
    Uninitialized,
    Recording,
    Paused,
    Stopped,
    Error,
    Denied
};

public enum ALSSearchManagerState {
    Uninitialized,
    Searching,
    SingleSearching,
    Paused,
    Stopped
};

public class ALSPlugin : MonoBehaviour {
    #region iOS Plugin Import

    delegate void ALSMonoPStateChangeDelegate(string error, int state);
    delegate void ALSMonoPSearchStateDelegate(string error, int state, int trackId, int offset);

    #if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void ALSSDKInit(string userName);
    [DllImport("__Internal")]
    private static extern void ALSSDKStart();
    [DllImport("__Internal")]
    private static extern void ALSSDKStop();
    [DllImport("__Internal")]
    private static extern void ALSSDKPause();
    [DllImport("__Internal")]
    private static extern void ALSSDKSingle();
    [DllImport("__Internal")]
    private static extern void ALSSDKUpdate();
    [DllImport("__Internal")]
    private static extern void ALSRegisterPluginHandlers(ALSMonoPStateChangeDelegate state, ALSMonoPSearchStateDelegate search);
    #endif

    #endregion

    #region Android Plugin Import

    #if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject PluginBridge;
    public class ALSPluginJavaMessageHandler : AndroidJavaProxy {
        public ALSPluginJavaMessageHandler() : base("com.alexanderkub.plugins.alssdk.ALSPluginJavaMessageHandler") { }

        public void OnStateChange(string message, int state) {
            ALSPlugin.OnStateChange(message, state);
        }
        public void OnSearchStateChange(string message, int state, int trackId, int offset) {
            ALSPlugin.OnSearchStateChange(message, state, trackId, offset);
        }
    }
    #endif

    #endregion

    #region ALS SDK API

    public static bool IsPluginInit { get; private set; }
    public static ALSAudioManagerState AudioManagerState { get; private set; }
    public static ALSSearchManagerState SearchManagerState { get; private set; }
    public static int LastTrackId { get; private set; }
    public static int LastTrackOffset { get; private set; }

    public static void InitSDK(string userName) {
        if (IsPluginInit) {
            Debug.LogError("ALS SDK already initialized!");
            return;
        }
        #if UNITY_EDITOR
        Debug.Log("ALSPlugin.InitSDK");
        #else
            #if UNITY_IOS
            ALSSDKInit(userName);
            #elif UNITY_ANDROID
            PluginBridge.Call("Init", userName);
            #endif
        #endif
        IsPluginInit = true;
    }
    
    public static void StartSearch() {
        if (!IsPluginInit) {
            Debug.LogError("ALS SDK not initialized!");
            return;
        }
        #if UNITY_EDITOR
        Debug.Log("ALSPlugin.StartSearch");
        #else
            #if UNITY_IOS
            ALSSDKStart();
            #elif UNITY_ANDROID
            PluginBridge.Call("StartSearch");
            #endif
        #endif
    }

    public static void StopSearch() {
        if (!IsPluginInit) {
            Debug.LogError("ALS SDK not initialized!");
            return;
        }
        #if UNITY_EDITOR
        Debug.Log("ALSPlugin.StopSearch");
        #else
            #if UNITY_IOS
            ALSSDKStop();
            #elif UNITY_ANDROID
            PluginBridge.Call("StopSearch");
            #endif
        #endif
    }

    public static void PauseSearch() {
        if (!IsPluginInit) {
            Debug.LogError("ALS SDK not initialized!");
            return;
        }
        #if UNITY_EDITOR
        Debug.Log("ALSPlugin.PauseSearch");
        #else
            #if UNITY_IOS
            ALSSDKPause();
            #elif UNITY_ANDROID
            PluginBridge.Call("StopSearch");
            #endif
        #endif
    }

    public static void SingleSearch() {
        if (!IsPluginInit) {
            Debug.LogError("ALS SDK not initialized!");
            return;
        }
        #if UNITY_EDITOR
        Debug.Log("ALSPlugin.SingleSearch");
        #else
            #if UNITY_IOS
            ALSSDKSingle();
            #elif UNITY_ANDROID
            PluginBridge.Call("SingleSearch");
            #endif
        #endif
    }
    
    public static void UpdateLocalBase() {
        if (!IsPluginInit) {
            Debug.LogError("ALS SDK not initialized!");
            return;
        }
        #if UNITY_EDITOR
        Debug.Log("ALSPlugin.UpdateLocalBase");
        #else
            #if UNITY_IOS
            ALSSDKUpdate();
            #elif UNITY_ANDROID
            PluginBridge.Call("UpdateLocalBase");
            #endif
        #endif
    }

    #endregion

    #region Native Callbacks

    public static Action<ALSAudioManagerState, string> OnStateChangeEvent;
    public static Action<ALSSearchManagerState, string, int, int> OnSearchStateChangeEvent;

    [RuntimeInitializeOnLoadMethod]
    private static void Initialize() {
        AudioManagerState = ALSAudioManagerState.Uninitialized;
        SearchManagerState = ALSSearchManagerState.Uninitialized;
        LastTrackId = -1;
        LastTrackOffset = -1;

        #if UNITY_EDITOR
        Debug.Log("ALSPlugin.Initialize");
        #else
            #if UNITY_IOS
            ALSRegisterPluginHandlers(OnStateChange, OnSearchStateChange);
            #elif UNITY_ANDROID
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject unityActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            PluginBridge = new AndroidJavaObject("com.alexanderkub.plugins.alssdk.Bridge");
            object[] parameters = new object[2];
            parameters[0] = unityActivity;
            parameters[1] = new ALSPluginJavaMessageHandler();
            PluginBridge.Call("registerPluginHandlers", parameters);
            #endif
        #endif
    }

    [AOT.MonoPInvokeCallback(typeof(ALSMonoPStateChangeDelegate))]
    private static void OnStateChange(string error, int state) {
        if (OnStateChangeEvent == null) {
            return;
        }
        if (state > -1) {
            AudioManagerState = (ALSAudioManagerState)state;
        }
        OnStateChangeEvent(AudioManagerState, error);
    }

    [AOT.MonoPInvokeCallback(typeof(ALSMonoPSearchStateDelegate))]
    private static void OnSearchStateChange(string error, int state, int trackId, int offset) {
        if (OnSearchStateChangeEvent == null) {
            return;
        }
        if (state > -1) {
            SearchManagerState = (ALSSearchManagerState)state;
        }
        if (trackId > -1) {
            LastTrackId = trackId;
        }
        if (offset > -1) {
            LastTrackOffset = offset;
        }
        OnSearchStateChangeEvent(SearchManagerState, error, trackId, offset);
    }

    void OnApplicationQuit() {
        #if !UNITY_EDITOR && UNITY_ANDROID
        PluginBridge.Call("ReleaseSDK");
        #endif
        Debug.Log("Application ending after " + Time.time + " seconds");
    }
    #endregion
}
