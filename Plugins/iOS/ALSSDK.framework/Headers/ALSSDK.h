//
//  ALSSDK.h
//  ALSSDK
//
//  Created by  user on 14/12/2018.
//  Copyright © 2018 cifrasoft. All rights reserved.
//

#import <UIKit/UIKit.h>

typedef void (*ALSMonoPStateChangeDelegate)(const char* error, int state);
typedef void (*ALSMonoPSearchStateDelegate)(const char* error, int state, int trackId, int offset);

//! Project version number for ALSSDK.
FOUNDATION_EXPORT double ALSSDKVersionNumber;

//! Project version string for ALSSDK.
FOUNDATION_EXPORT const unsigned char ALSSDKVersionString[];

@interface ALSSDK : NSObject
+ (instancetype) sharedALSSDK;
+ (void) RegisterPluginHandlers:(ALSMonoPStateChangeDelegate) stateChange :(ALSMonoPSearchStateDelegate) searchStateChange;
- (void) InitSDK:(NSString *) withName;
- (void) StartSDK;
- (void) StopSDK;
- (void) SingleSDK;
- (void) PauseSDK;
- (void) UpdateSDK;
@end
