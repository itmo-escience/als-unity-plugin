#import <ALSSDK/ALSSDK.h>

extern "C" {
    void ALSSDKInit(char *userName) {
        [[ALSSDK sharedALSSDK] InitSDK:[NSString stringWithUTF8String: userName]];
    }
    
    void ALSSDKStart() {
        [[ALSSDK sharedALSSDK] StartSDK];
    }
    
    void ALSSDKStop() {
        [[ALSSDK sharedALSSDK] StopSDK];
    }
    
    void ALSSDKPause() {
        [[ALSSDK sharedALSSDK] PauseSDK];
    }
    
    void ALSSDKSingle() {
        [[ALSSDK sharedALSSDK] SingleSDK];
    }
    
    void ALSSDKUpdate() {
        [[ALSSDK sharedALSSDK] UpdateSDK];
    }
    
    void ALSRegisterPluginHandlers(ALSMonoPStateChangeDelegate changeStateDelegate,
                                   ALSMonoPSearchStateDelegate changeSearchStateDelegate) {
        [ALSSDK RegisterPluginHandlers:changeStateDelegate :changeSearchStateDelegate];
    }
}
